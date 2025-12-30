using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPU_Dword_Manager_Avalonia.Models;
using System;

namespace GPU_Dword_Manager_Avalonia.ViewModels
{
    public partial class TweakViewModel : ObservableObject
    {
        public TweakDefinition Definition { get; }

        [ObservableProperty]
        private bool _isApplied;

        public string Name => Definition.Name;
        public string Description => Definition.Description;

        private readonly Action<TweakViewModel> _toggleAction;

        public TweakViewModel(TweakDefinition definition, Action<TweakViewModel> toggleAction)
        {
            Definition = definition;
            _toggleAction = toggleAction;
            IsApplied = definition.IsApplied;
        }

        [RelayCommand]
        private void Toggle()
        {
            _toggleAction(this);
        }

        partial void OnIsAppliedChanged(bool value)
        {
            OnPropertyChanged(nameof(ButtonText));
        }

        public string ButtonText => IsApplied ? "Revert" : "Apply";
    }
}

