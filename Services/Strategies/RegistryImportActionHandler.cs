using GPU_Dword_Manager_Avalonia.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GPU_Dword_Manager_Avalonia.Services.Strategies
{
    public class RegistryImportActionHandler : ITweakActionHandler
    {
        public bool CanHandle(TweakActionType actionType)
        {
            return actionType == TweakActionType.RegistryImport;
        }

        public void Apply(TweakChange change, List<DwordEntry> allEntries, string gpuRegistryPath, IRegistryService registryService)
        {
            try
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, change.FilePath);
                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = $"import \"{fullPath}\"",
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing registry: {ex.Message}");
            }
        }

        public void Revert(TweakChange change, string gpuRegistryPath, IRegistryService registryService)
        {
        }
    }
}

