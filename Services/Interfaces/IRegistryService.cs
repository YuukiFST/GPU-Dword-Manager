using System.Collections.Generic;
using System.Threading.Tasks;
using GPU_Dword_Manager_Avalonia.Models;
using System;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public interface IRegistryService
    {
        Task ReadRegistryValuesAsync(List<DwordEntry> entries, IProgress<int> progress);
        object? ReadDwordValue(DwordEntry entry);
        bool WriteDwordValue(DwordEntry entry, uint value);
        bool DeleteDwordValue(DwordEntry entry);
    }
}

