using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AMD_DWORD_Viewer.Controls
{



    public class DarkListView : ListView
    {

        private static readonly Color BackgroundColor = Color.FromArgb(30, 30, 30); // #1E1E1E
        private static readonly Color ForegroundColor = Color.White;

        public DarkListView()
        {
            try
            {

                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                
                BackColor = BackgroundColor;
                ForeColor = ForegroundColor;
                BorderStyle = BorderStyle.FixedSingle;
                FullRowSelect = true;
                View = View.Details;
                GridLines = true;
                

                OwnerDraw = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DarkListView constructor: {ex.Message}");
            }
        }


        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0x000B;

        public void BeginUpdate()
        {
            try
            {
                if (IsHandleCreated)
                {
                    SendMessage(Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch { }
        }

        public void EndUpdate()
        {
            try
            {
                if (IsHandleCreated)
                {
                    SendMessage(Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                    Invalidate();
                }
            }
            catch { }
        }
    }
}
