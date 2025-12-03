using System;
using System.Drawing;
using System.Windows.Forms;

namespace AMD_DWORD_Viewer
{
    public class EditValueDialog : Form
    {
        private Label lblKeyName;
        private Label lblDecimal;
        private Label lblHex;
        private TextBox txtDecimal;
        private TextBox txtHex;
        private Button btnOk;
        private Button btnCancel;
        private bool isUpdating = false;

        public uint Value { get; private set; }

        public EditValueDialog(string keyName, uint currentValue, bool isNewValue = false)
        {
            InitializeComponent();
            
            lblKeyName.Text = isNewValue ? $"Add new value: {keyName}" : $"Edit value: {keyName}";
            Value = currentValue;
            
            txtDecimal.Text = currentValue.ToString();
            txtHex.Text = "0x" + currentValue.ToString("X8");
        }

        private void InitializeComponent()
        {
            this.lblKeyName = new Label();
            this.lblDecimal = new Label();
            this.lblHex = new Label();
            this.txtDecimal = new TextBox();
            this.txtHex = new TextBox();
            this.btnOk = new Button();
            this.btnCancel = new Button();
            
            this.SuspendLayout();
            
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ClientSize = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit DWORD Value";
            this.Font = new Font("Segoe UI", 9F);
            
            this.lblKeyName.AutoSize = true;
            this.lblKeyName.ForeColor = Color.White;
            this.lblKeyName.Location = new Point(20, 20);
            this.lblKeyName.Size = new Size(360, 20);
            this.lblKeyName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            
            this.lblDecimal.AutoSize = true;
            this.lblDecimal.ForeColor = Color.White;
            this.lblDecimal.Location = new Point(20, 60);
            this.lblDecimal.Text = "Decimal:";
            
            this.txtDecimal.BackColor = Color.FromArgb(50, 50, 50);
            this.txtDecimal.ForeColor = Color.White;
            this.txtDecimal.Location = new Point(120, 57);
            this.txtDecimal.Size = new Size(250, 23);
            this.txtDecimal.TextChanged += TxtDecimal_TextChanged;
            
            this.lblHex.AutoSize = true;
            this.lblHex.ForeColor = Color.White;
            this.lblHex.Location = new Point(20, 95);
            this.lblHex.Text = "Hexadecimal:";
            
            this.txtHex.BackColor = Color.FromArgb(50, 50, 50);
            this.txtHex.ForeColor = Color.White;
            this.txtHex.Location = new Point(120, 92);
            this.txtHex.Size = new Size(250, 23);
            this.txtHex.TextChanged += TxtHex_TextChanged;
            
            this.btnOk.BackColor = Color.FromArgb(60, 60, 60);
            this.btnOk.FlatStyle = FlatStyle.Flat;
            this.btnOk.ForeColor = Color.White;
            this.btnOk.Location = new Point(200, 145);
            this.btnOk.Size = new Size(85, 30);
            this.btnOk.Text = "OK";
            this.btnOk.Click += BtnOk_Click;
            
            this.btnCancel.BackColor = Color.FromArgb(60, 60, 60);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(295, 145);
            this.btnCancel.Size = new Size(85, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.DialogResult = DialogResult.Cancel;
            
            this.Controls.Add(this.lblKeyName);
            this.Controls.Add(this.lblDecimal);
            this.Controls.Add(this.txtDecimal);
            this.Controls.Add(this.lblHex);
            this.Controls.Add(this.txtHex);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void TxtDecimal_TextChanged(object? sender, EventArgs e)
        {
            if (isUpdating) return;
            
            isUpdating = true;
            
            try
            {
                if (uint.TryParse(txtDecimal.Text, out uint value))
                {
                    Value = value;
                    txtHex.Text = "0x" + value.ToString("X8");
                    txtDecimal.ForeColor = Color.White;
                }
                else
                {
                    txtDecimal.ForeColor = Color.Red;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void TxtHex_TextChanged(object? sender, EventArgs e)
        {
            if (isUpdating) return;
            
            isUpdating = true;
            
            try
            {
                string hexText = txtHex.Text.Replace("0x", "").Replace("0X", "");
                
                if (uint.TryParse(hexText, System.Globalization.NumberStyles.HexNumber, null, out uint value))
                {
                    Value = value;
                    txtDecimal.Text = value.ToString();
                    txtHex.ForeColor = Color.White;
                }
                else
                {
                    txtHex.ForeColor = Color.Red;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (!uint.TryParse(txtDecimal.Text, out uint decValue))
            {
                MessageBox.Show("Invalid decimal value. Must be between 0 and 4,294,967,295.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Value = decValue;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
