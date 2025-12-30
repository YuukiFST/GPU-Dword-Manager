using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public class RegistryService : IRegistryService
    {
        private readonly RegistryReader _reader;
        private readonly RegistryWriter _writer;

        public RegistryService()
        {
            _reader = new RegistryReader();
            _writer = new RegistryWriter();
        }

        public async Task ReadRegistryValuesAsync(List<DwordEntry> entries, IProgress<int> progress)
        {
            await _reader.ReadRegistryValuesAsync(entries, progress);
        }

        public object? ReadDwordValue(DwordEntry entry)
        {
            return _reader.ReadDwordValue(entry);
        }

        public bool WriteDwordValue(DwordEntry entry, uint value)
        {
            return _writer.WriteDwordValue(entry, value);
        }

        public bool DeleteDwordValue(DwordEntry entry)
        {
            return _writer.DeleteDwordValue(entry);
        }
    }
}

