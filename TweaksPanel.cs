using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AMD_DWORD_Viewer.Models;
using AMD_DWORD_Viewer.Services;

namespace AMD_DWORD_Viewer
{
    public class TweaksPanel : Panel
    {
        private Label lblTitle;
        private List<TweakRow> tweakRows = new List<TweakRow>();
        private TweakManager tweakManager;
        private List<DwordEntry> allEntries;
        private Action refreshCallback;

        public TweaksPanel(TweakManager manager, List<DwordEntry> entries, Action onRefresh)
        {
            tweakManager = manager;
            allEntries = entries;
            refreshCallback = onRefresh;
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.Black;
            this.Dock = DockStyle.Right;
            this.Width = 320;
            this.AutoScroll = true;
            this.Padding = new Padding(10);

            // Title
            lblTitle = new Label
            {
                Text = "Quick Tweaks",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.Red,
                AutoSize = false,
                Size = new Size(300, 30),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblTitle);
        }

        public void LoadTweaks(List<TweakDefinition> tweaks)
        {
            int yPos = 50;

            foreach (var tweak in tweaks)
            {
                var row = new TweakRow(tweak, tweakManager, allEntries, refreshCallback);
                row.Location = new Point(10, yPos);
                row.Width = 300;
                
                this.Controls.Add(row);
                tweakRows.Add(row);
                
                yPos += row.Height + 10;
            }
        }

        public void UpdateTweakStates()
        {
            foreach (var row in tweakRows)
            {
                row.UpdateStatus();
            }
        }
    }

    /// <summary>
    /// Represents a single tweak row with apply/revert buttons
    /// </summary>
    public class TweakRow : Panel
    {
        private Label lblName;
        private Label lblStatus;
        private Button btnApply;
        private Button btnRevert;
        private TweakDefinition tweak;
        private TweakManager manager;
        private List<DwordEntry> allEntries;
        private Action refreshCallback;

        public TweakRow(TweakDefinition tweakDef, TweakManager tweakManager, List<DwordEntry> entries, Action onRefresh)
        {
            tweak = tweakDef;
            manager = tweakManager;
            allEntries = entries;
            refreshCallback = onRefresh;
            
            InitializeComponent();
            UpdateStatus();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Height = 80;

            // Name label
            lblName = new Label
            {
                Text = tweak.Name,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(280, 22),
                Location = new Point(10, 8),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblName);

            // Status label
            lblStatus = new Label
            {
                Font = new Font("Segoe UI", 8F),
                AutoSize = false,
                Size = new Size(280, 18),
                Location = new Point(10, 28),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblStatus);

            // Apply button
            btnApply = new Button
            {
                Text = "Apply",
                Size = new Size(85, 26),
                Location = new Point(10, 48),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F)
            };
            btnApply.FlatAppearance.BorderColor = Color.Red;
            btnApply.Click += BtnApply_Click;
            this.Controls.Add(btnApply);

            // Revert button
            btnRevert = new Button
            {
                Text = "Revert",
                Size = new Size(85, 26),
                Location = new Point(105, 48),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F)
            };
            btnRevert.FlatAppearance.BorderColor = Color.Red;
            btnRevert.Click += BtnRevert_Click;
            this.Controls.Add(btnRevert);
        }

        public void UpdateStatus()
        {
            if (tweak.IsApplied)
            {
                lblStatus.Text = $"✓ Applied | {tweak.Changes.Count} DWORDs";
                lblStatus.ForeColor = Color.LimeGreen;
                btnApply.Enabled = false;
                btnRevert.Enabled = true;
            }
            else
            {
                lblStatus.Text = $"Not Applied | {tweak.Changes.Count} DWORDs";
                lblStatus.ForeColor = Color.Gray;
                btnApply.Enabled = true;
                btnRevert.Enabled = false;
            }
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Apply {tweak.Name}?\n\n" +
                    $"This will modify {tweak.Changes.Count} DWORD values.\n" +
                    $"Original values will be backed up and can be reverted.\n\n" +
                    $"Continue?",
                    "Confirm Apply Tweak",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    manager.ApplyTweak(tweak, allEntries);
                    UpdateStatus();
                    refreshCallback?.Invoke();
                    
                    MessageBox.Show(
                        $"{tweak.Name} applied successfully!\n\n" +
                        $"{tweak.Changes.Count} DWORDs modified.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error applying tweak:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnRevert_Click(object? sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Revert {tweak.Name}?\n\n" +
                    $"This will restore original values for {tweak.Changes.Count} DWORDs.\n" +
                    $"DWORDs that didn't exist before will be deleted.\n\n" +
                    $"Continue?",
                    "Confirm Revert Tweak",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    manager.RevertTweak(tweak);
                    UpdateStatus();
                    refreshCallback?.Invoke();
                    
                    MessageBox.Show(
                        $"{tweak.Name} reverted successfully!\n\n" +
                        $"Original values restored.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error reverting tweak:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
