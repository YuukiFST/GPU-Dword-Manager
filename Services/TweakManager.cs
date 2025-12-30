using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GPU_Dword_Manager_Avalonia.Models;
using GPU_Dword_Manager_Avalonia.Services.Strategies;
using Microsoft.Win32;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public class TweakManager : ITweakService
    {
        private readonly IRegistryService _registryService;
        private readonly IEnumerable<ITweakActionHandler> _actionHandlers;
        private readonly string _stateFilePath;
        private readonly GpuVendor _vendor;
        private string _gpuRegistryPath = string.Empty;

        public TweakManager(IRegistryService registryService, IEnumerable<ITweakActionHandler> actionHandlers, GpuVendor selectedVendor = GpuVendor.AMD)
        {
            _registryService = registryService;
            _actionHandlers = actionHandlers;
            _vendor = selectedVendor;
            
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appData, "GPU_Dword_Manager_Avalonia");
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            
            _stateFilePath = Path.Combine(appFolder, "tweaks_state.json");
            _gpuRegistryPath = GetGpuRegistryPath();
        }

        private string GetGpuRegistryPath()
        {
            try
            {
                using (var classKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}"))
                {
                    if (classKey != null)
                    {
                        foreach (var subKeyName in classKey.GetSubKeyNames())
                        {
                            using (var subKey = classKey.OpenSubKey(subKeyName))
                            {
                                if (subKey != null)
                                {
                                    var desc = subKey.GetValue("DriverDesc") as string;
                                    if (desc != null)
                                    {
                                        bool isMatch = _vendor == GpuVendor.Nvidia
                                            ? (desc.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) || desc.Contains("GeForce", StringComparison.OrdinalIgnoreCase))
                                            : desc.Contains("Radeon", StringComparison.OrdinalIgnoreCase);
                                        
                                        if (isMatch)
                                        {
                                            return $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e968-e325-11ce-bfc1-08002be10318}}\\{subKeyName}";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting GPU registry path: {ex.Message}");
            }
            
            return string.Empty;
        }

        public void ApplyTweak(TweakDefinition tweak, List<DwordEntry> allEntries)
        {
            if (string.IsNullOrEmpty(_gpuRegistryPath))
            {
                throw new Exception($"Could not find {_vendor} GPU registry path");
            }

            foreach (var change in tweak.Changes)
            {
                var handler = _actionHandlers.FirstOrDefault(h => h.CanHandle(change.ActionType));
                handler?.Apply(change, allEntries, _gpuRegistryPath, _registryService);
            }

            tweak.IsApplied = true;
            SaveState(new List<TweakDefinition> { tweak });
        }

        public void RevertTweak(TweakDefinition tweak)
        {
            if (string.IsNullOrEmpty(_gpuRegistryPath))
            {
                throw new Exception($"Could not find {_vendor} GPU registry path");
            }

            foreach (var change in tweak.Changes)
            {
                var handler = _actionHandlers.FirstOrDefault(h => h.CanHandle(change.ActionType));
                handler?.Revert(change, _gpuRegistryPath, _registryService);
            }

            tweak.IsApplied = false;
            SaveState(new List<TweakDefinition> { tweak });
        }

        private void SaveState(List<TweakDefinition> tweaks)
        {
            try
            {
                var stateDict = new Dictionary<string, TweakState>();
                
                if (File.Exists(_stateFilePath))
                {
                    var json = File.ReadAllText(_stateFilePath);
                    stateDict = JsonSerializer.Deserialize<Dictionary<string, TweakState>>(json) 
                        ?? new Dictionary<string, TweakState>();
                }

                foreach (var tweak in tweaks)
                {
                    stateDict[tweak.Name] = new TweakState
                    {
                        IsApplied = tweak.IsApplied,
                        Changes = tweak.Changes.Select(c => new TweakChangeState
                        {
                            KeyName = c.KeyName,
                            TargetValue = c.TargetValue,
                            OriginalValue = c.OriginalValue,
                            ExistedBefore = c.ExistedBefore,
                            ActionType = (int)c.ActionType,
                            FilePath = c.FilePath,
                            RegistryPath = c.RegistryPath
                        }).ToList()
                    };
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonOutput = JsonSerializer.Serialize(stateDict, options);
                File.WriteAllText(_stateFilePath, jsonOutput);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving tweak state: {ex.Message}");
            }
        }

        public void LoadState(List<TweakDefinition> tweaks)
        {
            try
            {
                if (!File.Exists(_stateFilePath))
                    return;

                var json = File.ReadAllText(_stateFilePath);
                var stateDict = JsonSerializer.Deserialize<Dictionary<string, TweakState>>(json);

                if (stateDict != null)
                {
                    foreach (var tweak in tweaks)
                    {
                        if (stateDict.TryGetValue(tweak.Name, out var state))
                        {
                            tweak.IsApplied = state.IsApplied;
                            
                            foreach (var change in tweak.Changes)
                            {
                                var savedChange = state.Changes.FirstOrDefault(c => c.KeyName == change.KeyName);
                                if (savedChange != null)
                                {
                                    change.OriginalValue = savedChange.OriginalValue;
                                    change.ExistedBefore = savedChange.ExistedBefore;
                                    change.ActionType = (TweakActionType)savedChange.ActionType;
                                    change.FilePath = savedChange.FilePath;
                                    change.RegistryPath = savedChange.RegistryPath;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tweak state: {ex.Message}");
            }
        }
    }
}


