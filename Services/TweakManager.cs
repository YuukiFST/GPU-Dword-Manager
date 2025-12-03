using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AMD_DWORD_Viewer.Models;
using Microsoft.Win32;

namespace AMD_DWORD_Viewer.Services
{
    /// <summary>
    /// Manages tweak application and state persistence
    /// </summary>
    public class TweakManager
    {
        private readonly RegistryWriter registryWriter;
        private readonly RegistryReader registryReader;
        private readonly string stateFilePath;
        private string gpuRegistryPath = string.Empty;

        public TweakManager(RegistryWriter writer, RegistryReader reader)
        {
            registryWriter = writer;
            registryReader = reader;
            stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tweaks_state.json");
            
            // Get GPU registry path
            gpuRegistryPath = GetGpuRegistryPath();
        }

        private string GetGpuRegistryPath()
        {
            // Find AMD GPU registry key
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
                                    if (desc != null && desc.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
                                    {
                                        return $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e968-e325-11ce-bfc1-08002be10318}}\\{subKeyName}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            
            return string.Empty;
        }

        public void ApplyTweak(TweakDefinition tweak, List<DwordEntry> allEntries)
        {
            if (string.IsNullOrEmpty(gpuRegistryPath))
            {
                throw new Exception("Could not find AMD GPU registry path");
            }

            // Backup current values
            foreach (var change in tweak.Changes)
            {
                // Find if this DWORD exists in current entries
                var existingEntry = allEntries.FirstOrDefault(e => 
                    e.KeyName.Equals(change.KeyName, StringComparison.OrdinalIgnoreCase));

                if (existingEntry != null && existingEntry.Exists)
                {
                    change.ExistedBefore = true;
                    change.OriginalValue = existingEntry.Value != null ? Convert.ToUInt32(existingEntry.Value) : 0;
                }
                else
                {
                    change.ExistedBefore = false;
                    change.OriginalValue = null;
                }

                // Apply the change
                var entry = new DwordEntry
                {
                    KeyName = change.KeyName,
                    RegistryPath = gpuRegistryPath
                };

                registryWriter.WriteDwordValue(entry, change.TargetValue);
            }

            tweak.IsApplied = true;
            SaveState(new List<TweakDefinition> { tweak });
        }

        public void RevertTweak(TweakDefinition tweak)
        {
            if (string.IsNullOrEmpty(gpuRegistryPath))
            {
                throw new Exception("Could not find AMD GPU registry path");
            }

            foreach (var change in tweak.Changes)
            {
                var entry = new DwordEntry
                {
                    KeyName = change.KeyName,
                    RegistryPath = gpuRegistryPath
                };

                if (change.ExistedBefore && change.OriginalValue.HasValue)
                {
                    // Restore original value
                    registryWriter.WriteDwordValue(entry, change.OriginalValue.Value);
                }
                else if (!change.ExistedBefore)
                {
                    // Delete the value since it didn't exist before
                    registryWriter.DeleteDwordValue(entry);
                }
            }

            tweak.IsApplied = false;
            SaveState(new List<TweakDefinition> { tweak });
        }

        private void SaveState(List<TweakDefinition> tweaks)
        {
            try
            {
                // Load existing state
                var stateDict = new Dictionary<string, TweakState>();
                
                if (File.Exists(stateFilePath))
                {
                    var json = File.ReadAllText(stateFilePath);
                    stateDict = JsonSerializer.Deserialize<Dictionary<string, TweakState>>(json) 
                        ?? new Dictionary<string, TweakState>();
                }

                // Update states
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
                            ExistedBefore = c.ExistedBefore
                        }).ToList()
                    };
                }

                // Save
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonOutput = JsonSerializer.Serialize(stateDict, options);
                File.WriteAllText(stateFilePath, jsonOutput);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tweak state: {ex.Message}");
            }
        }

        public void LoadState(List<TweakDefinition> tweaks)
        {
            try
            {
                if (!File.Exists(stateFilePath))
                    return;

                var json = File.ReadAllText(stateFilePath);
                var stateDict = JsonSerializer.Deserialize<Dictionary<string, TweakState>>(json);

                if (stateDict != null)
                {
                    foreach (var tweak in tweaks)
                    {
                        if (stateDict.TryGetValue(tweak.Name, out var state))
                        {
                            tweak.IsApplied = state.IsApplied;
                            
                            // Restore change states
                            foreach (var change in tweak.Changes)
                            {
                                var savedChange = state.Changes.FirstOrDefault(c => c.KeyName == change.KeyName);
                                if (savedChange != null)
                                {
                                    change.OriginalValue = savedChange.OriginalValue;
                                    change.ExistedBefore = savedChange.ExistedBefore;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tweak state: {ex.Message}");
            }
        }
    }

    // State persistence classes
    public class TweakState
    {
        public bool IsApplied { get; set; }
        public List<TweakChangeState> Changes { get; set; } = new List<TweakChangeState>();
    }

    public class TweakChangeState
    {
        public string KeyName { get; set; } = string.Empty;
        public uint TargetValue { get; set; }
        public uint? OriginalValue { get; set; }
        public bool ExistedBefore { get; set; }
    }
}
