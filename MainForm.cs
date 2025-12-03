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
        // Windows API for dark title bar
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
        private bool showRegistryPath = true; // Changed to true since it's visible by default now
        private bool sortAscending = true; // Track sort direction for Key Name column

        public MainForm()
        {
            InitializeComponent();
            
            // Load the icon
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
                // Icon loading failed, use default
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
                // Show loading state
                lblStatus.Text = "Loading DWORDS from file...";
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                // Get path to AMD EXPORT.txt
                var exePath = AppDomain.CurrentDomain.BaseDirectory;
                var amdExportPath = Path.Combine(exePath, "AMD EXPORT.txt");

                // Parse the file
                await Task.Run(() =>
                {
                    allEntries = parser.ParseFile(amdExportPath);
                });

                lblStatus.Text = $"Reading registry values for {allEntries.Count} DWORDS...";
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = allEntries.Count;
                progressBar.Value = 0;

                // Read registry values with progress
                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                    if (value % 100 == 0 || value == allEntries.Count)
                    {
                        lblStatus.Text = $"Reading registry values... {value} of {allEntries.Count}";
                    }
                });

                await registryReader.ReadRegistryValuesAsync(allEntries, progress);

                // Update filtered list
                filteredEntries = new List<DwordEntry>(allEntries);

                // Update ListView
                listViewDwords.VirtualListSize = filteredEntries.Count;

                // Update status
                var foundCount = allEntries.Count(e => e.Exists);
                lblStatus.Text = $"{foundCount} of {allEntries.Count} DWORDS found in registry";
                progressBar.Visible = false;

                // Refresh the ListView
                listViewDwords.Invalidate();
                
                // Initialize tweaks
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
                
                // Column order: Key Name, Hex Value, Decimal Value, Status, Registry Path
                var item = new ListViewItem(new[]
                {
                    entry.KeyName,
                    entry.HexValue,
                    entry.DecimalValue,
                    entry.Status,
                    entry.RegistryPath
                });

                // Apply different styling for missing entries
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
            // Safety check: don't apply filter if data hasn't been loaded yet
            if (allEntries == null || allEntries.Count == 0)
                return;

            // Safety check: ensure UI is ready
            if (listViewDwords == null || lblStatus == null)
                return;

            try
            {
                var searchText = isSearchPlaceholder ? "" : txtSearch?.Text?.Trim() ?? "";
                var presenceFilter = cboFilter?.SelectedIndex ?? 0;

                // Start with all entries
                var entries = allEntries.AsEnumerable();

                // Apply presence filter
                if (presenceFilter == 1) // Present Only
                {
                    entries = entries.Where(e => e.Exists);
                }
                else if (presenceFilter == 2) // Missing Only
                {
                    entries = entries.Where(e => !e.Exists);
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    entries = entries.Where(e =>
                        e.KeyName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        e.RegistryPath.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    );
                }

                filteredEntries = entries.ToList();

                // Update ListView
                listViewDwords.BeginUpdate();
                listViewDwords.VirtualListSize = filteredEntries.Count;
                listViewDwords.EndUpdate();

                // Update status
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
            // Only apply filter if data is loaded
            if (allEntries != null && allEntries.Count > 0)
            {
                ApplyFilter();
            }
        }

        private void btnTogglePath_Click(object? sender, EventArgs e)
        {
            showRegistryPath = !showRegistryPath;
            
            // Toggle the Registry Path column width (now column index 4)
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
            // Only sort when Key Name column (index 0) is clicked
            if (e.Column == 0)
            {
                // Toggle sort direction
                sortAscending = !sortAscending;
                
                // Sort the filtered entries
                if (sortAscending)
                {
                    filteredEntries = filteredEntries.OrderBy(entry => entry.KeyName).ToList();
                }
                else
                {
                    filteredEntries = filteredEntries.OrderByDescending(entry => entry.KeyName).ToList();
                }
                
                // Refresh the ListView
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
                    
                    // Copy value to clipboard
                    if (entry.Exists && entry.Value != null)
                    {
                        var copyText = $"{entry.KeyName} = {entry.DisplayValue}";
                        Clipboard.SetText(copyText);
                        
                        // Flash the status to show it was copied
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
            // Only show context menu if an item is selected
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

            // Show/hide menu items based on whether the value exists
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

            // Get current value as uint - this is the OLD value before editing
            uint currentValue = entry.Value != null ? Convert.ToUInt32(entry.Value) : 0;

            using (var dialog = new EditValueDialog(entry.KeyName, currentValue, false))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // Write the new value
                        if (registryWriter.WriteDwordValue(entry, dialog.Value))
                        {
                            // Track the change - IMPORTANT: OldValue is what was in registry before this edit
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

                            // Update the entry
                            entry.Value = dialog.Value;
                            entry.Exists = true;

                            // Refresh the ListView
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

            // Confirm deletion
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
                        // Track the change - IMPORTANT: OldValue is what was in registry before deletion
                        // This allows us to restore it when reverting
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

                        // Update the entry
                        entry.Value = null;
                        entry.Exists = false;

                        // Refresh the ListView
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
                        // Write the new value
                        if (registryWriter.WriteDwordValue(entry, dialog.Value))
                        {
                            // Track the change
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

                            // Update the entry
                            entry.Value = dialog.Value;
                            entry.Exists = true;

                            // Refresh the ListView
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

            // Get the last change
            var lastChange = changeHistory[changeHistory.Count - 1];

            // Confirm undo
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
                // Find the entry in allEntries
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

                // Revert based on change type
                switch (lastChange.Type)
                {
                    case Models.ChangeType.Add:
                        // Delete the value that was added
                        success = registryWriter.DeleteDwordValue(entry);
                        if (success)
                        {
                            entry.Value = null;
                            entry.Exists = false;
                        }
                        break;

                    case Models.ChangeType.Delete:
                        // Re-add the value that was deleted
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
                        // Restore the old value
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

                    // Remove from history
                    changeHistory.Remove(lastChange);
                    UpdateUndoButton();

                    // Refresh the ListView
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
            // Re-read registry values for all entries
            Task.Run(async () =>
            {
                var progress = new Progress<int>();
                await registryReader.ReadRegistryValuesAsync(allEntries, progress);

                // Update filtered list
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
                // Load hardcoded tweaks
                tweaks = tweakParser.LoadTweaks();
                
                if (tweaks.Count == 0)
                {
                    lblStatus.Text += " | No tweaks loaded.";
                    return;
                }

                // Initialize manager and panel
                tweakManager = new TweakManager(registryWriter, registryReader);
                tweakManager.LoadState(tweaks);

                tweaksPanel = new TweaksPanel(tweakManager, allEntries, RefreshListView);
                tweaksPanel.LoadTweaks(tweaks);
                
                this.Controls.Add(tweaksPanel);
                
                // Adjust main list view to make room for tweaks panel
                listViewDwords.Width = this.ClientSize.Width - tweaksPanel.Width;
                
                lblStatus.Text += $" | {tweaks.Count} tweaks loaded.";
            }
            catch (Exception ex)
            {
                lblStatus.Text += $" | Error loading tweaks: {ex.Message}";
            }
        }
    }
}
