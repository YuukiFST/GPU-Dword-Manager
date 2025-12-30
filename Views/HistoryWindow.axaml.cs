using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GPU_Dword_Manager_Avalonia.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

