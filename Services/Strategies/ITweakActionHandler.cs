using GPU_Dword_Manager_Avalonia.Models;
using System.Collections.Generic;

namespace GPU_Dword_Manager_Avalonia.Services.Strategies
{
    public interface ITweakActionHandler
    {
        bool CanHandle(TweakActionType actionType);
        void Apply(TweakChange change, List<DwordEntry> allEntries, string gpuRegistryPath, IRegistryService registryService);
        void Revert(TweakChange change, string gpuRegistryPath, IRegistryService registryService);
    }
}

