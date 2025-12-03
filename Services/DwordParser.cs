using System;
using System.IO;
using System.Collections.Generic;
using AMD_DWORD_Viewer.Models;

namespace AMD_DWORD_Viewer.Services
{
    /// <summary>
    /// Service to parse AMD EXPORT.txt file and extract DWORD entries
    /// </summary>
    public class DwordParser
    {
        /// <summary>
        /// Parse the AMD EXPORT.txt file and return a list of DWORD entries
        /// </summary>
        /// <param name="filePath">Path to AMD EXPORT.txt</param>
        /// <returns>List of parsed DWORD entries</returns>
        public List<DwordEntry> ParseFile(string filePath)
        {
            var entries = new List<DwordEntry>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"AMD EXPORT.txt not found at: {filePath}");
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Parse line format: "Registry\Machine\path : KeyName"
                var parts = line.Split(new[] { " : " }, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;

                var registryPath = parts[0].Trim();
                var keyName = parts[1].Trim();

                // Convert from "Registry\Machine\" format to "HKEY_LOCAL_MACHINE\" format
                var convertedPath = ConvertRegistryPath(registryPath);

                entries.Add(new DwordEntry
                {
                    RegistryPath = convertedPath,
                    KeyName = keyName,
                    OriginalLine = line,
                    Exists = false, // Will be determined by RegistryReader
                    Value = null
                });
            }

            return entries;
        }

        /// <summary>
        /// Convert from "Registry\Machine\" format to "HKEY_LOCAL_MACHINE\" format
        /// </summary>
        private string ConvertRegistryPath(string registryPath)
        {
            // Registry\Machine\system\... -> HKEY_LOCAL_MACHINE\SYSTEM\...
            if (registryPath.StartsWith("Registry\\Machine\\", StringComparison.OrdinalIgnoreCase))
            {
                var remainder = registryPath.Substring("Registry\\Machine\\".Length);
                return $"HKEY_LOCAL_MACHINE\\{remainder}";
            }

            // Registry\User\... -> HKEY_CURRENT_USER\...
            if (registryPath.StartsWith("Registry\\User\\", StringComparison.OrdinalIgnoreCase))
            {
                var remainder = registryPath.Substring("Registry\\User\\".Length);
                return $"HKEY_CURRENT_USER\\{remainder}";
            }

            // If already in HKEY format, return as-is
            if (registryPath.StartsWith("HKEY_", StringComparison.OrdinalIgnoreCase))
            {
                return registryPath;
            }

            // Fallback: assume local machine
            return $"HKEY_LOCAL_MACHINE\\{registryPath}";
        }
    }
}
