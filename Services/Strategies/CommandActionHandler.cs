using GPU_Dword_Manager_Avalonia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GPU_Dword_Manager_Avalonia.Services.Strategies
{
    public class CommandActionHandler : ITweakActionHandler
    {
        public bool CanHandle(TweakActionType actionType)
        {
            return actionType == TweakActionType.Command;
        }

        public void Apply(TweakChange change, List<DwordEntry> allEntries, string gpuRegistryPath, IRegistryService registryService)
        {
            try
            {
                var assembly = typeof(CommandActionHandler).Assembly;
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(change.FilePath.Replace(" ", "_").Replace("\\", "."), StringComparison.OrdinalIgnoreCase))
                    ?? assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(change.FilePath.Replace("\\", "."), StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Resource not found for path: {change.FilePath}");
                    return;
                }

                var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(change.FilePath));
                
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return;
                    using (var fileStream = File.Create(tempPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true,
                    Verb = "runas"
                })?.WaitForExit();
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch {  }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error executing command: {ex.Message}");
            }
        }

        public void Revert(TweakChange change, string gpuRegistryPath, IRegistryService registryService)
        {
        }
    }
}

