using Microsoft.Reporting.WinForms;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Formulaire
{
    public partial class fprint : Form
    {
        public fprint()
        {
            InitializeComponent();
            Rpv = new ReportViewer();
            Rpv.Dock = DockStyle.Fill;
            this.Controls.Add(Rpv);
        }


    }
}

