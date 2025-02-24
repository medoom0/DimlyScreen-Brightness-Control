using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Dimly
{
    public partial class Form1 : Form
    {
        private Form overlay;
        private Timer clickThroughTimer;
        private ContextMenuStrip trayMenu;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        public Form1()
        {
            InitializeComponent();
            CreateOverlay();
            InitializeTimer();
            SetupTrayMenu();
        }

        private void CreateOverlay()
        {
            overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.Black,
                Opacity = 0.3,
                ShowInTaskbar = false,
                TopMost = true,
                WindowState = FormWindowState.Maximized
            };
            overlay.Show();
            MakeOverlayClickThrough();
        }

        private void InitializeTimer()
        {
            clickThroughTimer = new Timer();
            clickThroughTimer.Interval = 300;
            clickThroughTimer.Tick += (sender, e) =>
            {
                MakeOverlayClickThrough();
                clickThroughTimer.Stop();
            };
        }

        private void SetupTrayMenu()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show", null, ShowFormFromTray);
            trayMenu.Items.Add("Exit", null, exitToolStripMenuItem_Click);
            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowFormFromTray(sender, e);
            }
        }

        private void MakeOverlayClickThrough()
        {
            int style = GetWindowLong(overlay.Handle, GWL_EXSTYLE);
            SetWindowLong(overlay.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        private void brightnessSlider_Scroll(object sender, EventArgs e)
        {
            overlay.Opacity = brightnessSlider.Value / 100.0;
            brightnessLabel.Text = $"Brightness: {brightnessSlider.Value}%";
            clickThroughTimer.Stop();
            clickThroughTimer.Start();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            brightnessSlider.Value = 30;
            brightnessLabel.Text = "Brightness: 30%";
            overlay.Opacity = 0.3;
            MakeOverlayClickThrough();
        }

        private void MinToTray_Click(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon.Visible = true;
        }

        private void ShowFormFromTray(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
