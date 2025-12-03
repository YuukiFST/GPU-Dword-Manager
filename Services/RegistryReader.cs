using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMD_DWORD_Viewer.Models;
using Microsoft.Win32;

namespace AMD_DWORD_Viewer.Services
{
    /// <summary>
    /// Service to read DWORD values from Windows registry
    /// </summary>
    public class RegistryReader
    {
        /// <summary>
        /// Read registry values for a list of DWORD entries
        /// </summary>
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
                    // Log error but continue processing other entries
                    System.Diagnostics.Debug.WriteLine($"Error reading registry for {entry.KeyName}: {ex.Message}");
                    entry.Exists = false;
                    entry.Value = null;
                }
            }
        }

        /// <summary>
        /// Read registry value for a single DWORD entry
        /// </summary>
        private void ReadRegistryValue(DwordEntry entry)
        {
            // Parse the registry path
            var (hive, subKey) = ParseRegistryPath(entry.RegistryPath);
            
            if (hive == null || string.IsNullOrEmpty(subKey))
            {
                entry.Exists = false;
                return;
            }

            // Open the registry key
            using var key = hive.OpenSubKey(subKey, writable: false);
            
            if (key == null)
            {
                entry.Exists = false;
                return;
            }

            // Try to read the value
            var value = key.GetValue(entry.KeyName);
            
            if (value == null)
            {
                entry.Exists = false;
                return;
            }

            entry.Exists = true;
            entry.Value = value;
        }

        /// <summary>
        /// Parse registry path into hive and subkey
        /// </summary>
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

        /// <summary>
        /// Read registry values asynchronously
        /// </summary>
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
