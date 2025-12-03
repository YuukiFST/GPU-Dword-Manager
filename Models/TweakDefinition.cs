using System.Collections.Generic;

namespace AMD_DWORD_Viewer.Models
{
    public class TweakChange
    {
        public string KeyName { get; set; } = string.Empty;
        public uint TargetValue { get; set; }
        public uint? OriginalValue { get; set; }
        public bool ExistedBefore { get; set; }
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
