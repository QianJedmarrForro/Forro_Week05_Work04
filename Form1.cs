using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forro_Week05_Work04
{
    public partial class Form1 : Form
    {
        // Simulates a database of saved barcodes
        private readonly List<string> _savedBarcodes = new List<string>();

        private readonly Color _errorColor = Color.MistyRose;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AcceptButton = btnSave;     // Enter = Save
            this.CancelButton = btnCancel;   // Esc = Cancel

            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Items.AddRange(new object[]
                { "Food", "Beverages", "Electronics", "Clothing", "Household" });

            txtPrice.ReadOnly = true;        // Price is calculated

            txtCost.KeyPress += NumericOnly_KeyPress;
            txtMarkup.KeyPress += NumericOnly_KeyPress;

            ClearForm();
        }

        private void txtbarcode_TextChanged(object sender, EventArgs e)
        {
            string code = txtbarcode.Text.Trim();

            if (code == "")
            {
                lblBarcodeStatus.Text = "";
            }
            else if (_savedBarcodes.Contains(code))
            {
                lblBarcodeStatus.Text = "Barcode is already in use.";
                lblBarcodeStatus.ForeColor = Color.Firebrick;
            }
            else
            {
                lblBarcodeStatus.Text = "Barcode is available.";
                lblBarcodeStatus.ForeColor = Color.DimGray;
            }
        }

        private void txtCost_TextChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void txtMarkup_TextChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        // Price = Cost + (Cost x Markup%)
        private void Recalculate()
        {
            decimal cost, markup;
            bool costOk = decimal.TryParse(txtCost.Text, NumberStyles.Number,
                              CultureInfo.InvariantCulture, out cost);
            bool markupOk = decimal.TryParse(txtMarkup.Text, NumberStyles.Number,
                              CultureInfo.InvariantCulture, out markup);

            if (costOk && markupOk)
                txtPrice.Text = (cost * (1 + markup / 100m)).ToString("F2", CultureInfo.InvariantCulture);
            else
                txtPrice.Text = "";
        }

        // Only digits and one decimal point
        private void NumericOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox box = (TextBox)sender;
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isControl = char.IsControl(e.KeyChar);
            bool isFirstDot = e.KeyChar == '.' && box.Text.IndexOf('.') < 0;

            if (!isDigit && !isControl && !isFirstDot)
                e.Handled = true;
        }

        // Puts the fields back to their normal color
        private void ResetFieldColors()
        {
            txtbarcode.BackColor = SystemColors.Window;
            txtName.BackColor = SystemColors.Window;
            cmbCategory.BackColor = SystemColors.Window;
            txtCost.BackColor = SystemColors.Window;
            txtMarkup.BackColor = SystemColors.Window;
        }

        private bool ValidateInputs()
        {
            ResetFieldColors();

            List<string> errors = new List<string>();
            Control firstInvalid = null;

            string code = txtbarcode.Text.Trim();
            if (code == "")
            {
                errors.Add("- Barcode is required.");
                txtbarcode.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = txtbarcode;
            }
            else if (_savedBarcodes.Contains(code))
            {
                errors.Add("- This barcode already exists.");
                txtbarcode.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = txtbarcode;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errors.Add("- Name is required.");
                txtName.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = txtName;
            }

            if (cmbCategory.SelectedIndex < 0)
            {
                errors.Add("- Please select a category.");
                cmbCategory.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = cmbCategory;
            }

            decimal cost, markup;

            if (!decimal.TryParse(txtCost.Text, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out cost) || cost <= 0)
            {
                errors.Add("- Enter a cost greater than 0.");
                txtCost.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = txtCost;
            }

            if (!decimal.TryParse(txtMarkup.Text, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out markup) || markup < 0)
            {
                errors.Add("- Enter a markup of 0 or more.");
                txtMarkup.BackColor = _errorColor;
                if (firstInvalid == null) firstInvalid = txtMarkup;
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    "Please fix the following:\n\n" + string.Join("\n", errors),
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                firstInvalid.Focus();
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            _savedBarcodes.Add(txtbarcode.Text.Trim());

            string origin = rbLocal.Checked ? "Local" : "Abroad";
            string status = cbStatus.Checked ? "Active" : "Inactive";

            MessageBox.Show(
                "Product saved!\n\n" +
                "Barcode: " + txtbarcode.Text + "\n" +
                "Name: " + txtName.Text + "\n" +
                "Description: " + txtDescription.Text + "\n" +
                "Category: " + cmbCategory.Text + "\n" +
                "Origin: " + origin + "\n" +
                "Status: " + status + "\n" +
                "Cost: " + txtCost.Text + "\n" +
                "Markup: " + txtMarkup.Text + "%\n" +
                "Price: " + txtPrice.Text,
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Close the form? Unsaved data will be lost.",
                "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                this.Close();
        }

        private void ClearForm()
        {
            ResetFieldColors();
            txtbarcode.Clear();
            txtName.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            rbLocal.Checked = true;
            cbStatus.Checked = false;
            txtCost.Clear();
            txtMarkup.Clear();
            txtPrice.Clear();
            lblBarcodeStatus.Text = "";
            txtbarcode.Focus();
        }

        // Unused handlers: keep them, the Designer file references them
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void lblBarcode_Click(object sender, EventArgs e) { }
        private void lblName_Click(object sender, EventArgs e) { }
        private void lblDescription_Click(object sender, EventArgs e) { }
        private void txtDescription_TextChanged(object sender, EventArgs e) { }
        private void lblCategory_Click(object sender, EventArgs e) { }
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblOrigin_Click(object sender, EventArgs e) { }
        private void gbManufactured_Enter(object sender, EventArgs e) { }
        private void rbLocal_CheckedChanged(object sender, EventArgs e) { }
        private void rbAbroad_CheckedChanged(object sender, EventArgs e) { }
        private void lblStatus_Click(object sender, EventArgs e) { }
        private void cbStatus_CheckedChanged(object sender, EventArgs e) { }
        private void lblCost_Click(object sender, EventArgs e) { }
        private void txtPrice_TextChanged(object sender, EventArgs e) { }
        private void lblPrice_Click(object sender, EventArgs e) { }
        private void lblMarkup_Click(object sender, EventArgs e) { }
    }
}