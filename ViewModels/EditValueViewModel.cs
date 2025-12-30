using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GPU_Dword_Manager_Avalonia.ViewModels
{
    public partial class EditValueViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _keyName = "";

        [ObservableProperty]
        private uint _value;

        [ObservableProperty]
        private bool _isAddMode;

        public bool IsConfirmed { get; private set; }

        public string Title => IsAddMode ? "Add Missing Value" : "Edit Value";

        public EditValueViewModel(string keyName, uint value, bool isAddMode)
        {
            KeyName = keyName;
            Value = value;
            IsAddMode = isAddMode;
        }

        public EditValueViewModel() { }

        [RelayCommand]
        private void Save()
        {
            IsConfirmed = true;
            OnRequestClose();
        }

        [RelayCommand]
        private void Cancel()
        {
            IsConfirmed = false;
            OnRequestClose();
        }

        public event System.Action? RequestClose;

        private void OnRequestClose()
        {
            RequestClose?.Invoke();
        }
    }
}

