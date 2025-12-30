namespace GPU_Dword_Manager_Avalonia.Models
{
    public class DwordEntry
    {
        public string RegistryPath { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public bool Exists { get; set; }
        public object? Value { get; set; }

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

        public string DecimalValue
        {
            get
            {
                if (!Exists || Value == null)
                    return "N/A";

                return Value.ToString() ?? "N/A";
            }
        }

        public string Status => Exists ? "Present" : "Missing";
        public string OriginalLine { get; set; } = string.Empty;
    }
}

