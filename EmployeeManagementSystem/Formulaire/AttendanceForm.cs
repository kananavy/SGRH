using System;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Formulaire
{
    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
        }


        private void saveButton_Click(object sender, EventArgs e)
        {
            string employee = employeeComboBox.SelectedItem != null ? employeeComboBox.SelectedItem.ToString() : "Non sélectionné";
            DateTime entryTime = entryDatePicker.Value;
            DateTime exitTime = exitDatePicker.Value;
            string reason = reasonTextBox.Text;

            // Affichage des informations enregistrées
            MessageBox.Show($"Employé: {employee}\nEntrée: {entryTime}\nSortie: {exitTime}\nMotif: {reason}",
                            "Présence enregistrée");
        }
    }
}
