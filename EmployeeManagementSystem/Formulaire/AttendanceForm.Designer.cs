namespace EmployeeManagementSystem.Formulaire
{
    partial class AttendanceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.saveButton = new System.Windows.Forms.Button();
            this.reasonTextBox = new System.Windows.Forms.TextBox();
            this.exitDatePicker = new System.Windows.Forms.DateTimePicker();
            this.entryDatePicker = new System.Windows.Forms.DateTimePicker();
            this.employeeComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(561, 196);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(126, 27);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Enresistrer";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // reasonTextBox
            // 
            this.reasonTextBox.Location = new System.Drawing.Point(560, 90);
            this.reasonTextBox.Name = "reasonTextBox";
            this.reasonTextBox.Size = new System.Drawing.Size(110, 20);
            this.reasonTextBox.TabIndex = 8;
            // 
            // exitDatePicker
            // 
            this.exitDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.exitDatePicker.Location = new System.Drawing.Point(456, 142);
            this.exitDatePicker.Name = "exitDatePicker";
            this.exitDatePicker.Size = new System.Drawing.Size(72, 20);
            this.exitDatePicker.TabIndex = 7;
            // 
            // entryDatePicker
            // 
            this.entryDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.entryDatePicker.Location = new System.Drawing.Point(213, 127);
            this.entryDatePicker.Name = "entryDatePicker";
            this.entryDatePicker.Size = new System.Drawing.Size(77, 20);
            this.entryDatePicker.TabIndex = 6;
            // 
            // employeeComboBox
            // 
            this.employeeComboBox.FormattingEnabled = true;
            this.employeeComboBox.Items.AddRange(new object[] {
            "Alice Dupont",
            "John Smith",
            "Maria Hernandez"});
            this.employeeComboBox.Location = new System.Drawing.Point(333, 38);
            this.employeeComboBox.Name = "employeeComboBox";
            this.employeeComboBox.Size = new System.Drawing.Size(145, 21);
            this.employeeComboBox.TabIndex = 5;
            // 
            // AttendanceForm
            // 
            this.ClientSize = new System.Drawing.Size(860, 289);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.reasonTextBox);
            this.Controls.Add(this.exitDatePicker);
            this.Controls.Add(this.entryDatePicker);
            this.Controls.Add(this.employeeComboBox);
            this.Name = "AttendanceForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox reasonTextBox;
        private System.Windows.Forms.DateTimePicker exitDatePicker;
        private System.Windows.Forms.DateTimePicker entryDatePicker;
        private System.Windows.Forms.ComboBox employeeComboBox;
    }
}