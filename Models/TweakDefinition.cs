using System.Collections.Generic;

namespace GPU_Dword_Manager_Avalonia.Models
{
    public enum TweakActionType
    {
        Dword,
        Command,
        RegistryImport
    }

    public class TweakChange
    {
        public string KeyName { get; set; } = string.Empty;
        public uint TargetValue { get; set; }
        public uint? OriginalValue { get; set; }
        public bool ExistedBefore { get; set; }
        
        public TweakActionType ActionType { get; set; } = TweakActionType.Dword;
        public string FilePath { get; set; } = string.Empty;
        public string? RegistryPath { get; set; }
    }

    public class TweakDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TweakChange> Changes { get; set; } = new List<TweakChange>();
        public bool IsApplied { get; set; }
        
        public string DisplayName => Name;
        public string StatusText => IsApplied ? "Applied" : "Not Applied";
    }
}

