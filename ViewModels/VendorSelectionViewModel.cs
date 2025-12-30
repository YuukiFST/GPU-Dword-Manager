using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.ViewModels
{
    public partial class VendorSelectionViewModel : ObservableObject
    {
        public GpuVendor SelectedVendor { get; private set; } = GpuVendor.AMD;
        public bool IsConfirmed { get; private set; }

        [RelayCommand]
        private void SelectAmd()
        {
            SelectedVendor = GpuVendor.AMD;
            IsConfirmed = true;
            OnSelectionConfirmed();
        }

        [RelayCommand]
        private void SelectNvidia()
        {
            SelectedVendor = GpuVendor.Nvidia;
            IsConfirmed = true;
            OnSelectionConfirmed();
        }

        public event System.Action? SelectionConfirmed;

        private void OnSelectionConfirmed()
        {
            SelectionConfirmed?.Invoke();
        }
    }
}

