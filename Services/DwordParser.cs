using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using AMD_DWORD_Viewer.Models;

namespace AMD_DWORD_Viewer.Services
{
    public class DwordParser
    {
        public List<DwordEntry> ParseFile()
        {
            var entries = new List<DwordEntry>();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AMD_DWORD_Viewer.AMD_EXPORT.txt";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(new[] { " : " }, StringSplitOptions.None);
                        if (parts.Length != 2)
                            continue;

                        var registryPath = parts[0].Trim();
                        var keyName = parts[1].Trim();

                        var convertedPath = ConvertRegistryPath(registryPath);

                        entries.Add(new DwordEntry
                        {
                            RegistryPath = convertedPath,
                            KeyName = keyName,
                            OriginalLine = line,
                            Exists = false,
                            Value = null
                        });
                    }
                }
            }

            return entries;
        }

        private string ConvertRegistryPath(string registryPath)
        {
            if (registryPath.StartsWith("Registry\\Machine\\", StringComparison.OrdinalIgnoreCase))
            {
                var remainder = registryPath.Substring("Registry\\Machine\\".Length);
                return $"HKEY_LOCAL_MACHINE\\{remainder}";
            }

            if (registryPath.StartsWith("Registry\\User\\", StringComparison.OrdinalIgnoreCase))
            {
                var remainder = registryPath.Substring("Registry\\User\\".Length);
                return $"HKEY_CURRENT_USER\\{remainder}";
            }

            if (registryPath.StartsWith("HKEY_", StringComparison.OrdinalIgnoreCase))
            {
                return registryPath;
            }

            return $"HKEY_LOCAL_MACHINE\\{registryPath}";
        }
    }
}
