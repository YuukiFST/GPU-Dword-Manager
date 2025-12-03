using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using AMD_DWORD_Viewer.Models;
using AMD_DWORD_Viewer.Services;

using System.Runtime.InteropServices;

namespace AMD_DWORD_Viewer
{
    public partial class MainForm : Form
    {

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private List<DwordEntry> allEntries = new List<DwordEntry>();
        private List<DwordEntry> filteredEntries = new List<DwordEntry>();
        private readonly DwordParser parser = new DwordParser();
        private readonly RegistryReader registryReader = new RegistryReader();
        private readonly RegistryWriter registryWriter = new RegistryWriter();
        private readonly List<Models.ChangeEntry> changeHistory = new List<Models.ChangeEntry>();
        private readonly TweakParser tweakParser = new TweakParser();
        private TweakManager? tweakManager;
        private TweaksPanel? tweaksPanel;
        private List<TweakDefinition> tweaks = new List<TweakDefinition>();
        private bool isSearchPlaceholder = true;
        private bool showRegistryPath = true; 
        private bool sortAscending = true; 

        public MainForm()
        {
            InitializeComponent();
            

            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "amd.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            {
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                int value = 1;
                DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
            }
            catch { }
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Loading DWORDS...";
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                allEntries = await Task.Run(() => parser.ParseFile());

                lblStatus.Text = $"Reading registry values for {allEntries.Count} DWORDS...";
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = allEntries.Count;
                progressBar.Value = 0;


                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                    if (value % 100 == 0 || value == allEntries.Count)
                    {
                        lblStatus.Text = $"Reading registry values... {value} of {allEntries.Count}";
                    }
                });

                await registryReader.ReadRegistryValuesAsync(allEntries, progress);


                filteredEntries = new List<DwordEntry>(allEntries);


                listViewDwords.VirtualListSize = filteredEntries.Count;


                var foundCount = allEntries.Count(e => e.Exists);
                lblStatus.Text = $"{foundCount} of {allEntries.Count} DWORDS found in registry";
                progressBar.Visible = false;


                listViewDwords.Invalidate();
                

                InitializeTweaks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading DWORDS: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error loading DWORDS";
                progressBar.Visible = false;
            }
        }

        private void listViewDwords_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= 0 && e.ItemIndex < filteredEntries.Count)
            {
                var entry = filteredEntries[e.ItemIndex];
                

                var item = new ListViewItem(new[]
                {
                    entry.KeyName,
                    entry.HexValue,
                    entry.DecimalValue,
                    entry.Status,
                    entry.RegistryPath
                });


                if (!entry.Exists)
                {
                    item.ForeColor = Color.Gray;
                }

                e.Item = item;
            }
        }

        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (isSearchPlaceholder)
                return;

            ApplyFilter();
        }

        private void txtSearch_Enter(object? sender, EventArgs e)
        {
            if (isSearchPlaceholder)
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.White;
                isSearchPlaceholder = false;
            }
        }

        private void txtSearch_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search DWORDS...";
                txtSearch.ForeColor = Color.Gray;
                isSearchPlaceholder = true;
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {

            if (allEntries == null || allEntries.Count == 0)
                return;


            if (listViewDwords == null || lblStatus == null)
                return;

            try
            {
                var searchText = isSearchPlaceholder ? "" : txtSearch?.Text?.Trim() ?? "";
                var presenceFilter = cboFilter?.SelectedIndex ?? 0;


                var entries = allEntries.AsEnumerable();


                if (presenceFilter == 1) // Present Only
                {
                    entries = entries.Where(e => e.Exists);
                }
                else if (presenceFilter == 2) // Missing Only
                {
                    entries = entries.Where(e => !e.Exists);
                }


                if (!string.IsNullOrEmpty(searchText))
                {
                    entries = entries.Where(e =>
                        e.KeyName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        e.RegistryPath.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    );
                }

                filteredEntries = entries.ToList();


                listViewDwords.BeginUpdate();
                listViewDwords.VirtualListSize = filteredEntries.Count;
                listViewDwords.EndUpdate();


                var foundCount = filteredEntries.Count(e => e.Exists);
                var filterText = presenceFilter == 0 ? "" : presenceFilter == 1 ? " (showing present only)" : " (showing missing only)";
                lblStatus.Text = $"{foundCount} of {filteredEntries.Count} DWORDS found in registry{filterText}" +
                    (string.IsNullOrEmpty(searchText) ? "" : $" - filtered from {allEntries.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ApplyFilter: {ex.Message}");
            }
        }

        private void cboFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {

            if (allEntries != null && allEntries.Count > 0)
            {
                ApplyFilter();
            }
        }

        private void btnTogglePath_Click(object? sender, EventArgs e)
        {
            showRegistryPath = !showRegistryPath;
            

            if (showRegistryPath)
            {
                listViewDwords.Columns[4].Width = 500;
                btnTogglePath.Text = "Hide Registry Path";
            }
            else
            {
                listViewDwords.Columns[4].Width = 0;
                btnTogglePath.Text = "Show Registry Path";
            }
        }

        private void listViewDwords_ColumnClick(object? sender, ColumnClickEventArgs e)
        {

            if (e.Column == 0)
            {

                sortAscending = !sortAscending;
                

                if (sortAscending)
                {
                    filteredEntries = filteredEntries.OrderBy(entry => entry.KeyName).ToList();
                }
                else
                {
                    filteredEntries = filteredEntries.OrderByDescending(entry => entry.KeyName).ToList();
                }
                

                listViewDwords.BeginUpdate();
                listViewDwords.VirtualListSize = filteredEntries.Count;
                listViewDwords.EndUpdate();
            }
        }

        private void listViewDwords_DoubleClick(object? sender, EventArgs e)
        {
            if (listViewDwords.SelectedIndices.Count > 0)
            {
                var index = listViewDwords.SelectedIndices[0];
                if (index >= 0 && index < filteredEntries.Count)
                {
                    var entry = filteredEntries[index];
                    

                    if (entry.Exists && entry.Value != null)
                    {
                        var copyText = $"{entry.KeyName} = {entry.DisplayValue}";
                        Clipboard.SetText(copyText);
                        

                        var originalText = lblStatus.Text;
                        lblStatus.ForeColor = ColorTranslator.FromHtml("#FF8C00");
                        lblStatus.Text = $"Copied: {copyText}";
                        
                        var timer = new System.Windows.Forms.Timer();
                        timer.Interval = 2000;
                        timer.Tick += (s, args) =>
                        {
                            lblStatus.Text = originalText;
                            lblStatus.ForeColor = Color.White;
                            timer.Stop();
                            timer.Dispose();
                        };
                        timer.Start();
                    }
                }
            }
        }

        private void contextMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
        {

            if (listViewDwords.SelectedIndices.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            var index = listViewDwords.SelectedIndices[0];
            if (index < 0 || index >= filteredEntries.Count)
            {
                e.Cancel = true;
                return;
            }

            var entry = filteredEntries[index];


            menuItemEdit.Visible = entry.Exists;
            menuItemDelete.Visible = entry.Exists;
            menuItemAdd.Visible = !entry.Exists;
        }

        private void menuItemEdit_Click(object? sender, EventArgs e)
        {
            if (listViewDwords.SelectedIndices.Count == 0)
                return;

            var index = listViewDwords.SelectedIndices[0];
            if (index < 0 || index >= filteredEntries.Count)
                return;

            var entry = filteredEntries[index];

            if (!entry.Exists)
            {
                MessageBox.Show("Cannot edit a missing value. Use 'Add Missing Value' instead.",
                    "Cannot Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            uint currentValue = entry.Value != null ? Convert.ToUInt32(entry.Value) : 0;

            using (var dialog = new EditValueDialog(entry.KeyName, currentValue, false))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {

                        if (registryWriter.WriteDwordValue(entry, dialog.Value))
                        {

                            var change = new Models.ChangeEntry
                            {
                                Timestamp = DateTime.Now,
                                KeyName = entry.KeyName,
                                RegistryPath = entry.RegistryPath,
                                Type = Models.ChangeType.Edit,
                                OldValue = currentValue, // Value BEFORE the edit
                                NewValue = dialog.Value  // Value AFTER the edit
                            };
                            changeHistory.Add(change);
                            UpdateUndoButton();


                            entry.Value = dialog.Value;
                            entry.Exists = true;


                            listViewDwords.Invalidate();

                            MessageBox.Show(
                                $"Successfully updated {entry.KeyName} to {dialog.Value}\n\n" +
                                $"Change has been recorded in history.\n" +
                                $"Total changes tracked: {changeHistory.Count}",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to write registry value.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Administrator privileges required to write to registry.\n\nPlease run this application as Administrator.",
                            "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error writing registry value:\n{ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void menuItemDelete_Click(object? sender, EventArgs e)
        {
            if (listViewDwords.SelectedIndices.Count == 0)
                return;

            var index = listViewDwords.SelectedIndices[0];
            if (index < 0 || index >= filteredEntries.Count)
                return;

            var entry = filteredEntries[index];

            if (!entry.Exists)
            {
                MessageBox.Show("This value doesn't exist in the registry.",
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            var result = MessageBox.Show(
                $"Are you sure you want to delete this DWORD value?\n\n" +
                $"Key: {entry.KeyName}\n" +
                $"Path: {entry.RegistryPath}\n" +
                $"Current Value: {entry.DisplayValue}",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (registryWriter.DeleteDwordValue(entry))
                    {


                        uint? oldValue = entry.Value != null ? Convert.ToUInt32(entry.Value) : null;
                        var change = new Models.ChangeEntry
                        {
                            Timestamp = DateTime.Now,
                            KeyName = entry.KeyName,
                            RegistryPath = entry.RegistryPath,
                            Type = Models.ChangeType.Delete,
                            OldValue = oldValue,  // Value BEFORE deletion (will be restored on revert)
                            NewValue = null       // No value after deletion
                        };
                        changeHistory.Add(change);
                        UpdateUndoButton();


                        entry.Value = null;
                        entry.Exists = false;


                        listViewDwords.Invalidate();

                        MessageBox.Show($"Successfully deleted {entry.KeyName}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete registry value.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Administrator privileges required to delete registry values.\n\nPlease run this application as Administrator.",
                        "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting registry value:\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuItemAdd_Click(object? sender, EventArgs e)
        {
            if (listViewDwords.SelectedIndices.Count == 0)
                return;

            var index = listViewDwords.SelectedIndices[0];
            if (index < 0 || index >= filteredEntries.Count)
                return;

            var entry = filteredEntries[index];

            if (entry.Exists)
            {
                MessageBox.Show("This value already exists. Use 'Edit Value' instead.",
                    "Cannot Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new EditValueDialog(entry.KeyName, 0, true))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {

                        if (registryWriter.WriteDwordValue(entry, dialog.Value))
                        {

                            var change = new Models.ChangeEntry
                            {
                                Timestamp = DateTime.Now,
                                KeyName = entry.KeyName,
                                RegistryPath = entry.RegistryPath,
                                Type = Models.ChangeType.Add,
                                OldValue = null,
                                NewValue = dialog.Value
                            };
                            changeHistory.Add(change);
                            UpdateUndoButton();


                            entry.Value = dialog.Value;
                            entry.Exists = true;


                            listViewDwords.Invalidate();

                            MessageBox.Show($"Successfully added {entry.KeyName} with value {dialog.Value}",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to write registry value.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Administrator privileges required to write to registry.\n\nPlease run this application as Administrator.",
                            "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error writing registry value:\n{ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateUndoButton()
        {
            btnUndo.Enabled = changeHistory.Count > 0;
            btnHistory.Text = changeHistory.Count > 0 
                ? $"View History ({changeHistory.Count})"
                : "View History";
        }

        private void btnUndo_Click(object? sender, EventArgs e)
        {
            if (changeHistory.Count == 0)
                return;


            var lastChange = changeHistory[changeHistory.Count - 1];


            var message = lastChange.Type switch
            {
                Models.ChangeType.Add => $"Undo adding this value?\n\nThis will DELETE the value from the registry:\n\n{lastChange.KeyName}\nValue: {lastChange.NewValueDisplay}",
                Models.ChangeType.Delete => $"Undo deleting this value?\n\nThis will RESTORE the original value:\n\n{lastChange.KeyName}\nOriginal Value: {lastChange.OldValueDisplay}",
                Models.ChangeType.Edit => $"Undo editing this value?\n\nThis will RESTORE the original value:\n\n{lastChange.KeyName}\nCurrent Value: {lastChange.NewValueDisplay}\n→ Restore to: {lastChange.OldValueDisplay}",
                _ => "Undo this change?"
            };

            var result = MessageBox.Show(message, "Confirm Undo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {

                var entry = allEntries.FirstOrDefault(e => 
                    e.KeyName == lastChange.KeyName && 
                    e.RegistryPath == lastChange.RegistryPath);

                if (entry == null)
                {
                    MessageBox.Show("Could not find the entry to undo.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool success = false;


                switch (lastChange.Type)
                {
                    case Models.ChangeType.Add:

                        success = registryWriter.DeleteDwordValue(entry);
                        if (success)
                        {
                            entry.Value = null;
                            entry.Exists = false;
                        }
                        break;

                    case Models.ChangeType.Delete:

                        if (lastChange.OldValue.HasValue)
                        {
                            success = registryWriter.WriteDwordValue(entry, lastChange.OldValue.Value);
                            if (success)
                            {
                                entry.Value = lastChange.OldValue.Value;
                                entry.Exists = true;
                            }
                        }
                        break;

                    case Models.ChangeType.Edit:

                        if (lastChange.OldValue.HasValue)
                        {
                            success = registryWriter.WriteDwordValue(entry, lastChange.OldValue.Value);
                            if (success)
                            {
                                entry.Value = lastChange.OldValue.Value;
                                entry.Exists = true;
                            }
                        }
                        break;
                }

                if (success)
                {
                    MessageBox.Show("Change undone successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);


                    changeHistory.Remove(lastChange);
                    UpdateUndoButton();


                    listViewDwords.Invalidate();
                }
                else
                {
                    MessageBox.Show("Failed to undo change.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error undoing change:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHistory_Click(object? sender, EventArgs e)
        {
            using (var dialog = new HistoryDialog(changeHistory, registryWriter, RefreshListView))
            {
                dialog.ShowDialog(this);
            }
        }

        private void RefreshListView()
        {

            Task.Run(async () =>
            {
                var progress = new Progress<int>();
                await registryReader.ReadRegistryValuesAsync(allEntries, progress);


                this.Invoke((Action)(() =>
                {
                    ApplyFilter();
                    listViewDwords.Invalidate();
                }));
            });
        }

        private void InitializeTweaks()
        {
            try
            {
                tweaks = tweakParser.LoadTweaks();
                
                if (tweaks.Count == 0)
                {
                    lblStatus.Text += " | No tweaks loaded.";
                    return;
                }

                tweakManager = new TweakManager(registryWriter, registryReader);
                tweakManager.LoadState(tweaks);

                tweaksPanel = new TweaksPanel(tweakManager, allEntries, RefreshListView);
                tweaksPanel.LoadTweaks(tweaks);
                
                this.Controls.Add(tweaksPanel);
                
                listViewDwords.Width = this.ClientSize.Width - tweaksPanel.Width;
            }
            catch (Exception ex)
            {
                lblStatus.Text += $" | Error loading tweaks: {ex.Message}";
            }
        }
    }
}
