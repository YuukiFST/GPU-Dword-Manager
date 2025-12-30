using System;
using Avalonia.Controls;
using Avalonia.Platform;
using GPU_Dword_Manager_Avalonia.Models;
using GPU_Dword_Manager_Avalonia.ViewModels;

namespace GPU_Dword_Manager_Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel vm)
        {
            try
            {
                InitializeComponent();
                DataContext = vm;

                var vendor = vm.Vendor;
                var iconName = vendor == GpuVendor.AMD ? "amd.ico" : "nvidia.ico";
                try
                {
                    var uri = new Uri($"avares://GPU_Dword_Manager_Avalonia/{iconName}");
                    using var stream = AssetLoader.Open(uri);
                    Icon = new WindowIcon(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load icon: {ex.Message}");
                }
                
                vm.RequestEdit += OnRequestEdit;
                vm.RequestAdd += OnRequestAdd;
                vm.RequestDelete += OnRequestDelete;
                vm.RequestShowHistory += OnRequestShowHistory;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRASH in MainWindow: {ex}");
                System.Console.WriteLine($"CRASH in MainWindow: {ex}");
                throw;
            }
        }

        private async void OnRequestEdit(DwordEntry entry)
        {
            var currentVal = entry.Value != null ? System.Convert.ToUInt32(entry.Value) : 0;
            var vm = new EditValueViewModel(entry.KeyName, currentVal, false);
            var dialog = new EditValueWindow { DataContext = vm };
            
            vm.RequestClose += () => dialog.Close();
            
            await dialog.ShowDialog(this);
            
            if (vm.IsConfirmed)
            {
                (DataContext as MainViewModel)?.UpdateEntry(entry, vm.Value);
            }
        }

        private async void OnRequestAdd(DwordEntry entry)
        {
            var vm = new EditValueViewModel(entry.KeyName, 0, true);
            var dialog = new EditValueWindow { DataContext = vm };
            
            vm.RequestClose += () => dialog.Close();
            
            await dialog.ShowDialog(this);
            
            if (vm.IsConfirmed)
            {
                (DataContext as MainViewModel)?.UpdateEntry(entry, vm.Value);
            }
        }

        private void OnRequestDelete(DwordEntry entry)
        {

            (DataContext as MainViewModel)?.DeleteEntry(entry);
        }

        private void OnRequestShowHistory()
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;
            
            var historyVm = new HistoryViewModel(vm.History);
            var dialog = new HistoryWindow { DataContext = historyVm };
            dialog.ShowDialog(this);
        }
    }
}
