using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GPU_Dword_Manager_Avalonia.ViewModels;

namespace GPU_Dword_Manager_Avalonia.Views
{
    public partial class VendorSelectionWindow : Window
    {
        public VendorSelectionWindow(VendorSelectionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            
            vm.SelectionConfirmed += () =>
            {
                try
                {
                    var app = (App)Avalonia.Application.Current!;
                    var mainVm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<MainViewModel>(app.Services!, vm.SelectedVendor);
                    
                    var mainWindow = new MainWindow(mainVm);
                    mainWindow.Show();
                    
                    if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = mainWindow;
                    }
                    
                    this.Close();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CRASH: {ex}");
                    System.Console.WriteLine($"CRASH: {ex}");
                }
            };
        }

        public VendorSelectionWindow()
        {
            InitializeComponent();
        }
    }
}

