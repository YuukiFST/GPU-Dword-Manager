using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AMD_DWORD_Viewer.Models;
using AMD_DWORD_Viewer.Services;

namespace AMD_DWORD_Viewer
{
    public class HistoryDialog : Form
    {
        private ListView listViewHistory;
        private Button btnRevert;
        private Button btnClearHistory;
        private Button btnClose;
        private Label lblTitle;

        private List<ChangeEntry> changeHistory;
        private RegistryWriter registryWriter;
        private Action refreshCallback;

        public HistoryDialog(List<ChangeEntry> history, RegistryWriter writer, Action onRefresh)
        {
            changeHistory = history;
            registryWriter = writer;
            refreshCallback = onRefresh;
            
            InitializeComponent();
            LoadHistory();
        }

        private void InitializeComponent()
        {
            this.listViewHistory = new ListView();
            this.btnRevert = new Button();
            this.btnClearHistory = new Button();
            this.btnClose = new Button();
            this.lblTitle = new Label();
            
            this.SuspendLayout();
            

            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(900, 500);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Change History";
            this.Font = new Font("Segoe UI", 9F);
            

            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.ForeColor = Color.Red;
            this.lblTitle.Location = new Point(20, 15);
            this.lblTitle.Size = new Size(860, 25);
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.Text = "Registry Change History";
            

            this.listViewHistory.BackColor = Color.FromArgb(30, 30, 30);
            this.listViewHistory.ForeColor = Color.White;
            this.listViewHistory.FullRowSelect = true;
            this.listViewHistory.GridLines = true;
            this.listViewHistory.Location = new Point(20, 50);
            this.listViewHistory.Size = new Size(860, 380);
            this.listViewHistory.TabIndex = 0;
            this.listViewHistory.View = View.Details;
            this.listViewHistory.BorderStyle = BorderStyle.FixedSingle;
            this.listViewHistory.SelectedIndexChanged += ListView_SelectedIndexChanged;
            

            this.listViewHistory.Columns.Add("Timestamp", 150);
            this.listViewHistory.Columns.Add("Action", 80);
            this.listViewHistory.Columns.Add("Key Name", 250);
            this.listViewHistory.Columns.Add("Old Value", 150);
            this.listViewHistory.Columns.Add("New Value", 150);
            

            this.btnRevert.BackColor = Color.FromArgb(60, 60, 60);
            this.btnRevert.FlatStyle = FlatStyle.Flat;
            this.btnRevert.ForeColor = Color.White;
            this.btnRevert.Location = new Point(20, 445);
            this.btnRevert.Size = new Size(120, 30);
            this.btnRevert.Text = "Revert Selected";
            this.btnRevert.Enabled = false;
            this.btnRevert.Click += BtnRevert_Click;
            

            this.btnClearHistory.BackColor = Color.FromArgb(60, 60, 60);
            this.btnClearHistory.FlatStyle = FlatStyle.Flat;
            this.btnClearHistory.ForeColor = Color.White;
            this.btnClearHistory.Location = new Point(150, 445);
            this.btnClearHistory.Size = new Size(120, 30);
            this.btnClearHistory.Text = "Clear History";
            this.btnClearHistory.Click += BtnClearHistory_Click;
            

            this.btnClose.BackColor = Color.FromArgb(60, 60, 60);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(760, 445);
            this.btnClose.Size = new Size(120, 30);
            this.btnClose.Text = "Close";
            this.btnClose.DialogResult = DialogResult.OK;
            

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.listViewHistory);
            this.Controls.Add(this.btnRevert);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnClose);
            
            this.AcceptButton = this.btnClose;
            
            this.ResumeLayout(false);
        }

        private void LoadHistory()
        {
            listViewHistory.Items.Clear();
            

            foreach (var change in changeHistory.OrderByDescending(c => c.Timestamp))
            {
                var item = new ListViewItem(new[]
                {
                    change.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    change.ActionDescription,
                    change.KeyName,
                    change.OldValueDisplay,
                    change.NewValueDisplay
                });
                
                item.Tag = change;
                listViewHistory.Items.Add(item);
            }
            
            btnClearHistory.Enabled = changeHistory.Count > 0;
        }

        private void ListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            btnRevert.Enabled = listViewHistory.SelectedItems.Count > 0;
        }

        private void BtnRevert_Click(object? sender, EventArgs e)
        {
            if (listViewHistory.SelectedItems.Count == 0)
                return;
            
            var item = listViewHistory.SelectedItems[0];
            var change = item.Tag as ChangeEntry;
            
            if (change == null)
                return;
            

            var message = change.Type switch
            {
                ChangeType.Add => $"This will DELETE the registry value you added:\n\n{change.KeyName}\nValue: {change.NewValueDisplay}\n\nContinue?",
                ChangeType.Delete => $"This will RESTORE the original registry value you deleted:\n\n{change.KeyName}\nOriginal Value: {change.OldValueDisplay}\n\nContinue?",
                ChangeType.Edit => $"This will RESTORE the original registry value:\n\n{change.KeyName}\nCurrent: {change.NewValueDisplay}\n→ Restore to: {change.OldValueDisplay}\n\nContinue?",
                _ => "Revert this change?"
            };
            
            var result = MessageBox.Show(message, "Confirm Revert", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes)
                return;
            
            try
            {

                var entry = new DwordEntry
                {
                    KeyName = change.KeyName,
                    RegistryPath = change.RegistryPath
                };
                
                bool success = false;
                

                switch (change.Type)
                {
                    case ChangeType.Add:

                        success = registryWriter.DeleteDwordValue(entry);
                        break;
                    
                    case ChangeType.Delete:

                        if (change.OldValue.HasValue)
                        {
                            success = registryWriter.WriteDwordValue(entry, change.OldValue.Value);
                        }
                        break;
                    
                    case ChangeType.Edit:

                        if (change.OldValue.HasValue)
                        {
                            success = registryWriter.WriteDwordValue(entry, change.OldValue.Value);
                        }
                        break;
                }
                
                if (success)
                {
                    MessageBox.Show("Change reverted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    

                    changeHistory.Remove(change);
                    

                    LoadHistory();
                    

                    refreshCallback?.Invoke();
                }
                else
                {
                    MessageBox.Show("Failed to revert change.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reverting change:\\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearHistory_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all history?\\n\\nThis action cannot be undone.",
                "Confirm Clear History",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                changeHistory.Clear();
                LoadHistory();
                MessageBox.Show("History cleared.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
