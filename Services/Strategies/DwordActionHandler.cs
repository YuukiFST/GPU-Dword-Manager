using GPU_Dword_Manager_Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GPU_Dword_Manager_Avalonia.Services.Strategies
{
    public class DwordActionHandler : ITweakActionHandler
    {
        public bool CanHandle(TweakActionType actionType)
        {
            return actionType == TweakActionType.Dword;
        }

        public void Apply(TweakChange change, List<DwordEntry> allEntries, string gpuRegistryPath, IRegistryService registryService)
        {
            string targetPath = change.RegistryPath ?? gpuRegistryPath;
            
            var existingEntry = allEntries.FirstOrDefault(e => 
                e.KeyName.Equals(change.KeyName, StringComparison.OrdinalIgnoreCase) &&
                e.RegistryPath.Equals(targetPath, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null && existingEntry.Exists)
            {
                change.ExistedBefore = true;
                try
                {
                    change.OriginalValue = existingEntry.Value != null ? Convert.ToUInt32(existingEntry.Value) : 0;
                }
                catch (FormatException)
                {
                    change.ExistedBefore = false;
                    change.OriginalValue = null;
                }
                catch (InvalidCastException)
                {
                    change.ExistedBefore = false;
                    change.OriginalValue = null;
                }
            }
            else
            {
                var entryForCheck = new DwordEntry { KeyName = change.KeyName, RegistryPath = targetPath };
                var currentValue = registryService.ReadDwordValue(entryForCheck);
                
                if (currentValue != null)
                {
                    change.ExistedBefore = true;
                    change.OriginalValue = Convert.ToUInt32(currentValue);
                }
                else
                {
                    change.ExistedBefore = false;
                    change.OriginalValue = null;
                }
            }

            var entry = new DwordEntry
            {
                KeyName = change.KeyName,
                RegistryPath = targetPath
            };

            registryService.WriteDwordValue(entry, change.TargetValue);
        }

        public void Revert(TweakChange change, string gpuRegistryPath, IRegistryService registryService)
        {
            string targetPath = change.RegistryPath ?? gpuRegistryPath;
            
            var entry = new DwordEntry
            {
                KeyName = change.KeyName,
                RegistryPath = targetPath
            };

            if (change.ExistedBefore && change.OriginalValue.HasValue)
            {
                registryService.WriteDwordValue(entry, change.OriginalValue.Value);
            }
            else if (!change.ExistedBefore)
            {
                registryService.DeleteDwordValue(entry);
            }
        }
    }
}

