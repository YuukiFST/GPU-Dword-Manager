using System;

namespace GPU_Dword_Manager_Avalonia.Models
{
    public enum ChangeType
    {
        Add,
        Edit,
        Delete
    }

    public class ChangeEntry
    {
        public DateTime Timestamp { get; set; }
        public string KeyName { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public ChangeType Type { get; set; }
        public uint? OldValue { get; set; }
        public uint? NewValue { get; set; }

        public string ActionDescription
        {
            get
            {
                return Type switch
                {
                    ChangeType.Add => "Added",
                    ChangeType.Edit => "Edited",
                    ChangeType.Delete => "Deleted",
                    _ => "Unknown"
                };
            }
        }

        public string OldValueDisplay
        {
            get
            {
                if (OldValue.HasValue)
                    return $"{OldValue.Value} (0x{OldValue.Value:X8})";
                return "N/A";
            }
        }

        public string NewValueDisplay
        {
            get
            {
                if (NewValue.HasValue)
                    return $"{NewValue.Value} (0x{NewValue.Value:X8})";
                return "N/A";
            }
        }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {ActionDescription}: {KeyName}";
        }
    }
}

