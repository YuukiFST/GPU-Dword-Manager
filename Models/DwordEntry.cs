namespace AMD_DWORD_Viewer.Models
{
    /// <summary>
    /// Represents a single AMD GPU DWORD registry entry
    /// </summary>
    public class DwordEntry
    {
        /// <summary>
        /// Full registry path (e.g., "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{...}\0000")
        /// </summary>
        public string RegistryPath { get; set; } = string.Empty;

        /// <summary>
        /// Registry key/value name (e.g., "KMD_Oca_ErrorInjection")
        /// </summary>
        public string KeyName { get; set; } = string.Empty;

        /// <summary>
        /// Whether the DWORD exists in the system registry
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// The DWORD value if it exists (null if not found)
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Formatted display value combining hex and decimal
        /// </summary>
        public string DisplayValue
        {
            get
            {
                if (!Exists || Value == null)
                    return "N/A";

                if (Value is int intValue)
                    return $"0x{intValue:X8} ({intValue})";
                if (Value is uint uintValue)
                    return $"0x{uintValue:X8} ({uintValue})";
                if (Value is long longValue)
                    return $"0x{longValue:X16} ({longValue})";

                return Value.ToString() ?? "N/A";
            }
        }

        /// <summary>
        /// Hexadecimal representation of the value
        /// </summary>
        public string HexValue
        {
            get
            {
                if (!Exists || Value == null)
                    return "N/A";

                if (Value is int intValue)
                    return $"0x{intValue:X8}";
                if (Value is uint uintValue)
                    return $"0x{uintValue:X8}";
                if (Value is long longValue)
                    return $"0x{longValue:X16}";

                return "N/A";
            }
        }

        /// <summary>
        /// Decimal representation of the value
        /// </summary>
        public string DecimalValue
        {
            get
            {
                if (!Exists || Value == null)
                    return "N/A";

                return Value.ToString() ?? "N/A";
            }
        }

        /// <summary>
        /// Status text (Present/Missing)
        /// </summary>
        public string Status => Exists ? "Present" : "Missing";

        /// <summary>
        /// Original line from the AMD EXPORT.txt file
        /// </summary>
        public string OriginalLine { get; set; } = string.Empty;
    }
}
