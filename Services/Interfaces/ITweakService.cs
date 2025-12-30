using System.Collections.Generic;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public interface ITweakService
    {
        void ApplyTweak(TweakDefinition tweak, List<DwordEntry> allEntries);
        void RevertTweak(TweakDefinition tweak);
        void LoadState(List<TweakDefinition> tweaks);
    }
}

