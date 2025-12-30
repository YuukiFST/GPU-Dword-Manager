using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPU_Dword_Manager_Avalonia.Models;
using Microsoft.Win32;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public class RegistryReader
    {
        public object? ReadDwordValue(DwordEntry entry)
        {
            ReadRegistryValue(entry);
            return entry.Value;
        }

        public void ReadRegistryValues(List<DwordEntry> entries)
        {
            foreach (var entry in entries)
            {
                try
                {
                    ReadRegistryValue(entry);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading registry for {entry.KeyName}: {ex.Message}");
                    entry.Exists = false;
                    entry.Value = null;
                }
            }
        }

        private void ReadRegistryValue(DwordEntry entry)
        {
            var (hive, subKey) = ParseRegistryPath(entry.RegistryPath);
            
            if (hive == null || string.IsNullOrEmpty(subKey))
            {
                entry.Exists = false;
                return;
            }

            using var key = hive.OpenSubKey(subKey, writable: false);
            
            if (key == null)
            {
                entry.Exists = false;
                return;
            }

            var value = key.GetValue(entry.KeyName);
            
            if (value == null)
            {
                entry.Exists = false;
                return;
            }

            entry.Exists = true;
            entry.Value = value;
        }

        private (RegistryKey? hive, string subKey) ParseRegistryPath(string fullPath)
        {
            if (fullPath.StartsWith("HKEY_LOCAL_MACHINE\\", StringComparison.OrdinalIgnoreCase))
            {
                var subKey = fullPath.Substring("HKEY_LOCAL_MACHINE\\".Length);
                return (Registry.LocalMachine, subKey);
            }
            else if (fullPath.StartsWith("HKEY_CURRENT_USER\\", StringComparison.OrdinalIgnoreCase))
            {
                var subKey = fullPath.Substring("HKEY_CURRENT_USER\\".Length);
                return (Registry.CurrentUser, subKey);
            }
            else if (fullPath.StartsWith("HKEY_CLASSES_ROOT\\", StringComparison.OrdinalIgnoreCase))
            {
                var subKey = fullPath.Substring("HKEY_CLASSES_ROOT\\".Length);
                return (Registry.ClassesRoot, subKey);
            }
            else if (fullPath.StartsWith("HKEY_USERS\\", StringComparison.OrdinalIgnoreCase))
            {
                var subKey = fullPath.Substring("HKEY_USERS\\".Length);
                return (Registry.Users, subKey);
            }
            else if (fullPath.StartsWith("HKEY_CURRENT_CONFIG\\", StringComparison.OrdinalIgnoreCase))
            {
                var subKey = fullPath.Substring("HKEY_CURRENT_CONFIG\\".Length);
                return (Registry.CurrentConfig, subKey);
            }

            return (null, string.Empty);
        }

        public Task ReadRegistryValuesAsync(List<DwordEntry> entries, IProgress<int>? progress = null)
        {
            return Task.Run(() =>
            {
                int processed = 0;
                foreach (var entry in entries)
                {
                    try
                    {
                        ReadRegistryValue(entry);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error reading registry for {entry.KeyName}: {ex.Message}");
                        entry.Exists = false;
                        entry.Value = null;
                    }

                    processed++;
                    progress?.Report(processed);
                }
            });
        }
    }
}

