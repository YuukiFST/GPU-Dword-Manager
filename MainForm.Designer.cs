using System;
using System.Drawing;
using System.Windows.Forms;

namespace AMD_DWORD_Viewer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Controls.DarkListView listViewDwords;
        private TextBox txtSearch;
        private Label lblStatus;
        private Panel panelTop;
        private Panel panelBottom;
        private ProgressBar progressBar;
        private ComboBox cboFilter;
        private Label lblFilter;
        private Button btnTogglePath;
        private Button btnUndo;
        private Button btnHistory;
        private Label lblCredit;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem menuItemEdit;
        private ToolStripMenuItem menuItemDelete;
        private ToolStripMenuItem menuItemAdd;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listViewDwords = new Controls.DarkListView();
            this.contextMenu = new ContextMenuStrip(this.components);
            this.menuItemEdit = new ToolStripMenuItem();
            this.menuItemDelete = new ToolStripMenuItem();
            this.menuItemAdd = new ToolStripMenuItem();
            this.txtSearch = new TextBox();
            this.lblStatus = new Label();
            this.panelTop = new Panel();
            this.panelBottom = new Panel();
            this.progressBar = new ProgressBar();
            this.lblFilter = new Label();
            this.cboFilter = new ComboBox();
            this.btnTogglePath = new Button();
            this.btnUndo = new Button();
            this.btnHistory = new Button();
            this.lblCredit = new Label();
            this.contextMenu.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // panelTop
            // 
            this.panelTop.BackColor = Color.Black;
            this.panelTop.Controls.Add(this.lblFilter);
            this.panelTop.Controls.Add(this.cboFilter);
            this.panelTop.Controls.Add(this.btnTogglePath);
            this.panelTop.Controls.Add(this.btnUndo);
            this.panelTop.Controls.Add(this.btnHistory);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Location = new Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new Padding(10);
            this.panelTop.Size = new Size(1200, 100); // Reduced back to 100
            this.panelTop.TabIndex = 0;
            
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.BackColor = Color.Transparent;
            this.lblFilter.Font = new Font("Segoe UI", 9F);
            this.lblFilter.ForeColor = Color.White;
            this.lblFilter.Location = new Point(10, 25); // Moved up
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new Size(90, 15);
            this.lblFilter.TabIndex = 2;
            this.lblFilter.Text = "Show:";;
            
            // 
            // cboFilter
            // 
            this.cboFilter.BackColor = ColorTranslator.FromHtml("#1E1E1E");
            this.cboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilter.FlatStyle = FlatStyle.Flat;
            this.cboFilter.Font = new Font("Segoe UI", 9F);
            this.cboFilter.ForeColor = Color.White;
            this.cboFilter.Location = new Point(60, 22); // Moved up
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new Size(120, 23);
            this.cboFilter.TabIndex = 3;
            this.cboFilter.Items.AddRange(new object[] { "All DWORDS", "Present Only", "Missing Only" });
            this.cboFilter.SelectedIndex = 0;
            this.cboFilter.SelectedIndexChanged += new EventHandler(this.cboFilter_SelectedIndexChanged);
            
            // 
            // btnTogglePath
            // 
            this.btnTogglePath.BackColor = ColorTranslator.FromHtml("#3E3E40");
            this.btnTogglePath.FlatStyle = FlatStyle.Flat;
            this.btnTogglePath.FlatAppearance.BorderColor = Color.Red; // Changed to Red
            this.btnTogglePath.Font = new Font("Segoe UI", 9F);
            this.btnTogglePath.ForeColor = Color.White;
            this.btnTogglePath.Location = new Point(200, 20); // Moved up
            this.btnTogglePath.Name = "btnTogglePath";
            this.btnTogglePath.Size = new Size(150, 27);
            this.btnTogglePath.TabIndex = 4;
            this.btnTogglePath.Text = "Hide Registry Path"; // Changed default text
            this.btnTogglePath.UseVisualStyleBackColor = false;
            this.btnTogglePath.Click += new EventHandler(this.btnTogglePath_Click);
            
            // 
            // btnUndo
            // 
            this.btnUndo.BackColor = Color.FromArgb(60, 60, 60);
            this.btnUndo.FlatStyle = FlatStyle.Flat;
            this.btnUndo.FlatAppearance.BorderColor = Color.Red;
            this.btnUndo.Font = new Font("Segoe UI", 9F);
            this.btnUndo.ForeColor = Color.White;
            this.btnUndo.Location = new Point(365, 20); // Moved up
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new Size(100, 27);
            this.btnUndo.TabIndex = 5;
            this.btnUndo.Text = "Undo Last";
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.Enabled = false;
            this.btnUndo.Click += new EventHandler(this.btnUndo_Click);
            
            // 
            // btnHistory
            // 
            this.btnHistory.BackColor = Color.FromArgb(60, 60, 60);
            this.btnHistory.FlatStyle = FlatStyle.Flat;
            this.btnHistory.FlatAppearance.BorderColor = Color.Red;
            this.btnHistory.Font = new Font("Segoe UI", 9F);
            this.btnHistory.ForeColor = Color.White;
            this.btnHistory.Location = new Point(475, 20); // Moved up
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new Size(100, 27);
            this.btnHistory.TabIndex = 6;
            this.btnHistory.Text = "View History";
            this.btnHistory.UseVisualStyleBackColor = false;
            this.btnHistory.Click += new EventHandler(this.btnHistory_Click);
            
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = ColorTranslator.FromHtml("#1E1E1E");
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.Dock = DockStyle.Bottom;
            this.txtSearch.Font = new Font("Segoe UI", 10F);
            this.txtSearch.ForeColor = Color.White;
            this.txtSearch.Location = new Point(10, 60); // Moved up
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new Size(1180, 25);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.Text = "Search DWORDS...";
            this.txtSearch.Margin = new Padding(0, 10, 0, 10); // Increased margin
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.Enter += new EventHandler(this.txtSearch_Enter);
            this.txtSearch.Leave += new EventHandler(this.txtSearch_Leave);
            
            // 
            // listViewDwords
            // 
            this.listViewDwords.Dock = DockStyle.Fill;
            this.listViewDwords.Font = new Font("Consolas", 9F);
            this.listViewDwords.Location = new Point(0, 100); // Updated for reduced panel height
            this.listViewDwords.Name = "listViewDwords";
            this.listViewDwords.Size = new Size(1200, 580); // Adjusted height
            this.listViewDwords.TabIndex = 6;
            this.listViewDwords.UseCompatibleStateImageBehavior = false;
            this.listViewDwords.VirtualMode = true;
            this.listViewDwords.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.listViewDwords_RetrieveVirtualItem);
            this.listViewDwords.DoubleClick += new EventHandler(this.listViewDwords_DoubleClick);
            this.listViewDwords.ColumnClick += new ColumnClickEventHandler(this.listViewDwords_ColumnClick);
            
            // Add columns - Registry Path moved to the end (after Status) - ONLY 5 columns total
            this.listViewDwords.Columns.Add("Key Name", 400);
            this.listViewDwords.Columns.Add("Hex Value", 150);
            this.listViewDwords.Columns.Add("Decimal Value", 150);
            this.listViewDwords.Columns.Add("Status", 120);
            this.listViewDwords.Columns.Add("Registry Path", 800); // Wider to show full path
            
            // Context menu
            this.listViewDwords.ContextMenuStrip = this.contextMenu;
            
            // 
            // contextMenu
            // 
            this.contextMenu.BackColor = Color.FromArgb(45, 45, 48);
            this.contextMenu.ForeColor = Color.White;
            this.contextMenu.Items.AddRange(new ToolStripItem[] {
                this.menuItemEdit,
                this.menuItemDelete,
                this.menuItemAdd
            });
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new Size(150, 70);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Name = "menuItemEdit";
            this.menuItemEdit.Size = new Size(149, 22);
            this.menuItemEdit.Text = "Edit Value";
            this.menuItemEdit.Click += new EventHandler(this.menuItemEdit_Click);
            
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Name = "menuItemDelete";
            this.menuItemDelete.Size = new Size(149, 22);
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new EventHandler(this.menuItemDelete_Click);
            
            // 
            // menuItemAdd
            // 
            this.menuItemAdd.Name = "menuItemAdd";
            this.menuItemAdd.Size = new Size(149, 22);
            this.menuItemAdd.Text = "Add Missing Value";
            this.menuItemAdd.Click += new EventHandler(this.menuItemAdd_Click);
            
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = Color.Black;
            this.panelBottom.Controls.Add(this.lblStatus);
            this.panelBottom.Controls.Add(this.progressBar);
            this.panelBottom.Controls.Add(this.lblCredit);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Location = new Point(0, 680);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new Padding(10);
            this.panelBottom.Size = new Size(1200, 40);
            this.panelBottom.TabIndex = 3;
            
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = Color.Transparent;
            this.lblStatus.Font = new Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = Color.White;
            this.lblStatus.Location = new Point(10, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(1000, 20); // Fixed width to leave space for credit
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Loading...";
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            
            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(10, 10);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(1000, 20); // Fixed width to leave space for credit
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            
            // 
            // lblCredit
            // 
            this.lblCredit.AutoSize = true;
            this.lblCredit.BackColor = Color.Transparent;
            this.lblCredit.Font = new Font("Segoe UI", 8F);
            this.lblCredit.ForeColor = Color.Gray;
            this.lblCredit.Location = new Point(1090, 12);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new Size(100, 13);
            this.lblCredit.TabIndex = 2;
            this.lblCredit.Text = "made by yuu_0711";
            this.lblCredit.TextAlign = ContentAlignment.MiddleRight;
            this.lblCredit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black; // Changed to Black
            this.ClientSize = new Size(1200, 720);
            this.Controls.Add(this.listViewDwords);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Font = new Font("Segoe UI", 9F);
            this.ForeColor = Color.White;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "AMD Config";
            this.Load += new EventHandler(this.MainForm_Load);
            this.contextMenu.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
