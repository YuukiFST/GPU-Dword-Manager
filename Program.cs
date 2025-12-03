using System;
using System.Windows.Forms;

namespace AMD_DWORD_Viewer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Enable visual styles for modern appearance
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Set high DPI awareness
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                
                // Note: Admin check removed - will show warning in app if needed
                
                // Run the application
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fatal error starting application:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
