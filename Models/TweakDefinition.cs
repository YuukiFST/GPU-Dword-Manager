using System.Collections.Generic;

namespace AMD_DWORD_Viewer.Models
{
    /// <summary>
    /// Represents a single DWORD change within a tweak
    /// </summary>
    public class TweakChange
    {
        public string KeyName { get; set; } = string.Empty;
        public uint TargetValue { get; set; }
        public uint? OriginalValue { get; set; }  // null if didn't exist before
        public bool ExistedBefore { get; set; }
    }

    /// <summary>
    /// Represents a complete tweak with all its DWORD changes
    /// </summary>
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
