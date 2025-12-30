using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPU_Dword_Manager_Avalonia.Models;
using GPU_Dword_Manager_Avalonia.Services;
using Avalonia.Threading;

namespace GPU_Dword_Manager_Avalonia.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDwordParser _parser;
        private readonly IRegistryService _registryService;
        private readonly ITweakParser _tweakParser;
        private readonly ITweakService _tweakManager;
        private readonly GpuVendor _vendor;
        public GpuVendor Vendor => _vendor;
        
        [ObservableProperty]
        private ObservableCollection<DwordEntry> _filteredEntries = new();
        
        private List<DwordEntry> _allEntries = new();

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        private int _selectedFilterIndex = 0;

        [ObservableProperty]
        private bool _isRegistryPathVisible = true;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private double _progressValue;

        [ObservableProperty]
        private bool _isProgressVisible;

        [ObservableProperty]
        private ObservableCollection<TweakViewModel> _tweaks = new();

        public MainViewModel(
            GpuVendor vendor,
            Func<GpuVendor, IDwordParser> dwordParserFactory,
            IRegistryService registryService,
            Func<GpuVendor, ITweakParser> tweakParserFactory,
            Func<GpuVendor, ITweakService> tweakServiceFactory)
        {
            _vendor = vendor;
            _parser = dwordParserFactory(vendor);
            _registryService = registryService;
            _tweakParser = tweakParserFactory(vendor);
            _tweakManager = tweakServiceFactory(vendor);
            
            LoadDataCommand.Execute(null);
            LoadTweaks();
        }

        private void LoadTweaks()
        {
            var definitions = _tweakParser.LoadTweaks();
            _tweakManager.LoadState(definitions);

            foreach (var def in definitions)
            {
                Tweaks.Add(new TweakViewModel(def, OnTweakToggled));
            }
        }

        private async void OnTweakToggled(TweakViewModel tweak)
        {
            try
            {
                if (tweak.IsApplied)
                {

                    await Task.Run(() => _tweakManager.RevertTweak(tweak.Definition));
                    tweak.IsApplied = false;
                }
                else
                {

                    await Task.Run(() => _tweakManager.ApplyTweak(tweak.Definition, _allEntries));
                    tweak.IsApplied = true;
                }
                

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                StatusText = $"Error applying tweak: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;
            IsProgressVisible = true;
            StatusText = "Loading DWORDS...";

            try
            {
                _allEntries = await Task.Run(() => _parser.ParseFile());

                StatusText = $"Reading registry values for {_allEntries.Count} DWORDS...";
                
                var progress = new Progress<int>(value =>
                {
                    ProgressValue = (double)value / _allEntries.Count * 100;
                    if (value % 100 == 0 || value == _allEntries.Count)
                    {
                        StatusText = $"Reading registry values... {value} of {_allEntries.Count}";
                    }
                });

                await _registryService.ReadRegistryValuesAsync(_allEntries, progress);

                ApplyFilter();
                
                var foundCount = _allEntries.Count(e => e.Exists);
                StatusText = $"{foundCount} of {_allEntries.Count} DWORDS found in registry";
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                IsProgressVisible = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedFilterIndexChanged(int value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allEntries == null) return;

            var query = _allEntries.AsEnumerable();

            if (SelectedFilterIndex == 1)
            {
                query = query.Where(e => e.Exists);
            }
            else if (SelectedFilterIndex == 2)
            {
                query = query.Where(e => !e.Exists);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(e => 
                    e.KeyName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.RegistryPath.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            FilteredEntries = new ObservableCollection<DwordEntry>(query);
        }

        [RelayCommand]
        private void TogglePath()
        {
            IsRegistryPathVisible = !IsRegistryPathVisible;
            OnPropertyChanged(nameof(RegistryPathButtonText));
        }

        public string RegistryPathButtonText => IsRegistryPathVisible ? "Hide Registry Path" : "Show Registry Path";

        [RelayCommand]
        private void Edit(DwordEntry? entry)
        {
            if (entry == null || !entry.Exists) return;
            RequestEdit?.Invoke(entry);
        }

        [RelayCommand]
        private void Add(DwordEntry? entry)
        {
            if (entry == null || entry.Exists) return;
            RequestAdd?.Invoke(entry);
        }

        [RelayCommand]
        private void Delete(DwordEntry? entry)
        {
            if (entry == null || !entry.Exists) return;
            

            RequestDelete?.Invoke(entry);
        }

        [ObservableProperty]
        private ObservableCollection<ChangeEntry> _history = new();

        [RelayCommand]
        private void ShowHistory()
        {
            RequestShowHistory?.Invoke();
        }

        [RelayCommand]
        private void Undo()
        {
            if (History.Count == 0) return;

            var lastChange = History.Last();
            

            bool success = false;
            var entry = _allEntries.FirstOrDefault(e => e.KeyName == lastChange.KeyName && e.RegistryPath == lastChange.RegistryPath);
            
            if (entry == null) return;

            switch (lastChange.Type)
            {
                case ChangeType.Add:
                    success = _registryService.DeleteDwordValue(entry);
                    if (success)
                    {
                        entry.Value = null;
                        entry.Exists = false;
                    }
                    break;
                case ChangeType.Delete:
                case ChangeType.Edit:
                    if (lastChange.OldValue.HasValue)
                    {
                        success = _registryService.WriteDwordValue(entry, lastChange.OldValue.Value);
                        if (success)
                        {
                            entry.Value = lastChange.OldValue.Value;
                            entry.Exists = true;
                        }
                    }
                    break;
            }

            if (success)
            {
                History.Remove(lastChange);

                var index = FilteredEntries.IndexOf(entry);
                if (index >= 0)
                {
                    FilteredEntries[index] = entry;
                }
            }
        }

        public void UpdateEntry(DwordEntry entry, uint newValue)
        {
            var oldValue = entry.Value != null ? Convert.ToUInt32(entry.Value) : (uint?)null;
            var type = entry.Exists ? ChangeType.Edit : ChangeType.Add;

            if (_registryService.WriteDwordValue(entry, newValue))
            {
                entry.Value = newValue;
                entry.Exists = true;
                
                History.Add(new ChangeEntry
                {
                    Timestamp = DateTime.Now,
                    KeyName = entry.KeyName,
                    RegistryPath = entry.RegistryPath,
                    Type = type,
                    OldValue = oldValue,
                    NewValue = newValue
                });

                var index = FilteredEntries.IndexOf(entry);
                if (index >= 0)
                {
                    FilteredEntries[index] = entry; 
                }
            }
        }

        public void DeleteEntry(DwordEntry entry)
        {
            var oldValue = entry.Value != null ? Convert.ToUInt32(entry.Value) : (uint?)null;

            if (_registryService.DeleteDwordValue(entry))
            {
                entry.Value = null;
                entry.Exists = false;
                
                History.Add(new ChangeEntry
                {
                    Timestamp = DateTime.Now,
                    KeyName = entry.KeyName,
                    RegistryPath = entry.RegistryPath,
                    Type = ChangeType.Delete,
                    OldValue = oldValue,
                    NewValue = null
                });

                var index = FilteredEntries.IndexOf(entry);
                if (index >= 0)
                {
                    FilteredEntries[index] = entry; 
                }
            }
        }

        public event Action<DwordEntry>? RequestEdit;
        public event Action<DwordEntry>? RequestAdd;
        public event Action<DwordEntry>? RequestDelete;
        public event Action? RequestShowHistory;
    }
}

