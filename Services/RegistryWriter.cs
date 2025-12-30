using System;
using Microsoft.Win32;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public class RegistryWriter
    {
        public bool WriteDwordValue(Models.DwordEntry entry, uint newValue)
        {
            try
            {
                var parts = entry.RegistryPath.Split(new[] { '\\' }, 2);
                if (parts.Length != 2)
                    return false;

                var hiveStr = parts[0];
                var subKeyPath = parts[1];

                RegistryKey? hive = GetRegistryHive(hiveStr);
                if (hive == null)
                    return false;

                using (var key = hive.OpenSubKey(subKeyPath, writable: true))
                {
                    if (key == null)
                    {
                        using (var newKey = hive.CreateSubKey(subKeyPath, writable: true))
                        {
                            if (newKey == null)
                                return false;

                            newKey.SetValue(entry.KeyName, newValue, RegistryValueKind.DWord);
                        }
                    }
                    else
                    {
                        key.SetValue(entry.KeyName, newValue, RegistryValueKind.DWord);
                    }
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Administrator privileges required to write to registry.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write registry value: {ex.Message}", ex);
            }
        }

        public bool DeleteDwordValue(Models.DwordEntry entry)
        {
            try
            {
                var parts = entry.RegistryPath.Split(new[] { '\\' }, 2);
                if (parts.Length != 2)
                    return false;

                var hiveStr = parts[0];
                var subKeyPath = parts[1];

                RegistryKey? hive = GetRegistryHive(hiveStr);
                if (hive == null)
                    return false;

                using (var key = hive.OpenSubKey(subKeyPath, writable: true))
                {
                    if (key == null)
                        return false;

                    if (key.GetValue(entry.KeyName) == null)
                        return false;

                    key.DeleteValue(entry.KeyName);
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Administrator privileges required to delete registry value.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete registry value: {ex.Message}", ex);
            }
        }

        private RegistryKey? GetRegistryHive(string hiveName)
        {
            return hiveName.ToUpper() switch
            {
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => null
            };
        }
    }
}

