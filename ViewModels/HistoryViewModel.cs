using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ChangeEntry> _changes;

        public HistoryViewModel(ObservableCollection<ChangeEntry> changes)
        {
            _changes = changes;
        }

        public HistoryViewModel()
        {
            _changes = new ObservableCollection<ChangeEntry>();
        }
    }
}

