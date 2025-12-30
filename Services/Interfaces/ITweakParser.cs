using System.Collections.Generic;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public interface ITweakParser
    {
        List<TweakDefinition> LoadTweaks();
    }
}

