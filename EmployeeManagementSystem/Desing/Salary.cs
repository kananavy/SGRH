using EmployeeManagementSystem.Formulaire;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class Salary : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        decimal heuresSupplementaires;

        public Salary()
        {
            InitializeComponent();
            LoadData();
            p1.Visible = radioButton1.Checked;
        }

        private void LoadData()
        {
            displayEmployees();
            disableFields();
            p1.Visible = radioButton1.Checked;
            p1.Visible = true;
        }

        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }

            displayEmployees();
            disableFields();
            p1.Visible = radioButton1.Checked;
            p1.Visible = true;
        }

        public void disableFields()
        {
            salary_employeeID.Enabled = false;
            salary_name.Enabled = false;
            salary_departement.Enabled = false;
            salary_position.Enabled = false;
            salary_Total.Enabled = false;
            salary_overtimeHours.Enabled = false;
            salary_overtimeRate.Enabled = false;
            p1.Visible = radioButton1.Checked;

        }

        public void displayEmployees()
        {
            SalaryData ed = new SalaryData();
            List<SalaryData> listData = ed.salaryEmployeeListData();
            dataGridView1.DataSource = listData;
            p1.Visible = radioButton1.Checked;
        }

        // Ajout des propriétés pour le calcul
        private decimal baseSalary;
        public decimal overtimeHours;
        public decimal overtimeRate;
        public decimal bonuses;

        public decimal BaseSalary
        {
            get => baseSalary;
            set => baseSalary = value;
        }

        // Calcul du salaire brut

        private void salary_baseSalary_TextChanged(object sender, EventArgs e)
        {

        }

        private void salary_updateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(salary_employeeID.Text) || string.IsNullOrEmpty(salary_name.Text) ||
                string.IsNullOrEmpty(salary_departement.Text) || string.IsNullOrEmpty(salary_position.Text) ||
                string.IsNullOrEmpty(salary_baseSalary.Text) || string.IsNullOrEmpty(salary_overtimeRate.Text) ||
                string.IsNullOrEmpty(salary_bonuses.Text) || string.IsNullOrEmpty(salary_numCompte.Text) ||
                string.IsNullOrEmpty(salary_agence.Text) || string.IsNullOrEmpty(salary_Total.Text))
            {
                // Afficher message d'erreur
                pictureBox6.Visible = false;
                pictureBox1.Visible = true;
                label8.Visible = true;
                p1.Visible = radioButton1.Checked;
                p2.Visible = false;
                p1.Visible = true;
                return; // Ajout d'un return pour arrêter l'exécution si des champs sont vides
            }

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string heuresSupplementairesString;
                    string selectQuery = "SELECT heures_supplementaires FROM employees WHERE employee_id = @employeeID";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, connect))
                    {
                        selectCmd.Parameters.AddWithValue("@employeeID", salary_employeeID.Text.Trim());
                        var result = selectCmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {

                            // On suppose que le résultat est une chaîne au format "HH:mm"
                            heuresSupplementairesString = result.ToString();

                            // Essayer de convertir la chaîne en TimeSpan
                            if (TimeSpan.TryParse(heuresSupplementairesString, out TimeSpan heures_supplementaires))
                            {
                                // Convertir TimeSpan en decimal (nombre d'heures)
                                heuresSupplementaires = (decimal)heures_supplementaires.TotalHours;

                                // Afficher les heures supplémentaires

                            }
                            else
                            {
                                // Gestion des erreurs
                                MessageBox.Show("La valeur d'heures supplémentaires est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            // Gestion du cas où aucune valeur n'est trouvée
                            MessageBox.Show("Aucune valeur d'heures supplémentaires trouvée pour cet employé.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }



                    // Initialiser le champ salary_overtimeHours avec la valeur récupérée
                    salary_overtimeHours.Text = heuresSupplementaires.ToString();

                    // Conversion des valeurs entrées
                    decimal baseSalary = Decimal.Parse(salary_baseSalary.Text);

                    decimal overtimeHours = heuresSupplementaires; // Utiliser les heures récupérées

                    decimal TauxHoraire = (baseSalary / 173);

                    decimal overtimeRate = (TauxHoraire * 1.5m);

                    decimal bonuses = Decimal.Parse(salary_bonuses.Text);

                    // Calcul du montant des heures supplémentaires
                    decimal overtimeAmount = overtimeHours * overtimeRate;
                    // Limiter le montant à deux chiffres après la virgule
                    overtimeAmount = Math.Round(overtimeAmount, 0);

                    Console.WriteLine($"Heures Montant supplémentaires : {overtimeAmount}");

                    // Calcul du salaire brut total
                    decimal grossSalary = baseSalary + overtimeAmount + bonuses;

                    //  Console.WriteLine($"Heures Montant salaire brut : {grossSalary}");


                    // Définir le format personnalisé pour l'ariary
                    string ariaryFormat = "### ### ### ### Ar";  // Utilisez "Ar" comme symbole et "### ### ### ###" pour le format des nombres

                    // Convertir le montant en string avec le format personnalisé
                    string formattedSalary = grossSalary.ToString(ariaryFormat, CultureInfo.InvariantCulture);


                    // Convertir le montant en string avec le format personnalisé
                    string formattedSalaryBase = baseSalary.ToString(ariaryFormat, CultureInfo.InvariantCulture);

                    // Assigner le texte formaté à la TextBox
                    salary_Total.Text = formattedSalary;
                    salary_baseSalary.Text = formattedSalaryBase;
                    Console.WriteLine($"Montent SB en Ar : {formattedSalaryBase}");
                    Console.WriteLine($"Montent total en popAr : {formattedSalary}");



                    // Mise à jour des informations dans la base de données
                    string updateData = "UPDATE employees SET salary_base = @salary_base, number_count = @number_count, agency = @agency, " +
                                        "overtime_rate = @overtime_rate, bonuses = @bonuses, " +
                                        "gross_salary = @gross_salary, update_date = @update_date, DatePaiement= @DatePaiement WHERE employee_id = @employeeID";

                    using (SqlCommand cmd = new SqlCommand(updateData, connect))
                    {
                        cmd.Parameters.AddWithValue("@number_count", salary_numCompte.Text.Trim());
                        cmd.Parameters.AddWithValue("@agency", salary_agence.Text.Trim());
                        cmd.Parameters.AddWithValue("@salary_base", formattedSalaryBase);
                        cmd.Parameters.AddWithValue("@overtime_rate", overtimeAmount);
                        cmd.Parameters.AddWithValue("@bonuses", bonuses);
                        cmd.Parameters.AddWithValue("@gross_salary", formattedSalary);
                        cmd.Parameters.AddWithValue("@update_date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@DatePaiement", DateTime.Parse(DatePaiement.Text.Trim()));
                        cmd.Parameters.AddWithValue("@employeeID", salary_employeeID.Text.Trim());

                        cmd.ExecuteNonQuery();

                        // Actualisation des données
                        displayEmployees();

                        clearFields();
                        pictureBox6.Visible = true;
                        pictureBox1.Visible = false;
                        label8.Visible = false;
                        p1.Visible = radioButton1.Checked;
                        p2.Visible = false;
                        p1.Visible = true;
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur: " + ex.Message, "Message d'erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }




        public void clearFields()
        {
            salary_employeeID.Text = "";
            salary_name.Text = "";
            salary_departement.Text = "";
            salary_position.Text = "";
            salary_baseSalary.Text = "";
            salary_overtimeHours.Text = "";
            salary_overtimeRate.Text = "";
            salary_bonuses.Text = "";
            salary_numCompte.Text = "";
            salary_agence.Text = "";
            salary_Total.Text = "";
            DatePaiement.Text = "";
            pictureBox1.Visible = true;
            pictureBox6.Visible = false;
            p1.Visible = radioButton1.Checked;
            p2.Visible = false;
            p1.Visible = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                salary_employeeID.Text = row.Cells[0].Value.ToString();
                salary_name.Text = row.Cells[1].Value.ToString();
                salary_departement.Text = row.Cells[2].Value.ToString();
                salary_position.Text = row.Cells[3].Value.ToString();
                salary_numCompte.Text = row.Cells[4].Value.ToString();
                salary_agence.Text = row.Cells[5].Value.ToString();
                salary_baseSalary.Text = row.Cells[6].Value.ToString();
                salary_overtimeHours.Text = row.Cells[7].Value.ToString();
                salary_overtimeRate.Text = row.Cells[8].Value.ToString();
                salary_bonuses.Text = row.Cells[9].Value.ToString();
                salary_Total.Text = row.Cells[10].Value.ToString();
            }
        }
        private void addEmployee_printBtn_Click(object sender, EventArgs e)
        {
            // Supposons que vous récupériez l'ID de l'employé à partir d'une TextBox
            string employeeId = salary_employeeID.Text; // Récupération de l'ID de l'employé sélectionné (string)

            using (fprint frm = new fprint())
            {
                // Création d'une source de données pour le rapport
                ReportDataSource reportDataSource = new ReportDataSource
                {
                    Name = "Salary", // Nom correspondant à votre source de données dans le rapport
                    Value = GetEmployeeData(employeeId) // Obtention des données filtrées par employé
                };

                // Chemin du rapport .rdlc
                string reportPath = @"C:\Users\kananavy\Pictures\Teste2\SGE\EmployeeManagementSystem\Repport\RpSalary.rdlc";
                frm.Rpv.LocalReport.ReportPath = reportPath;
                frm.Rpv.LocalReport.DataSources.Clear(); // Nettoyage des sources de données précédentes
                frm.Rpv.LocalReport.DataSources.Add(reportDataSource); // Ajout de la nouvelle source de données

                // Rafraîchissement du rapport
                frm.Rpv.RefreshReport();
                frm.StartPosition = FormStartPosition.CenterScreen; // Centrage du formulaire
                frm.WindowState = FormWindowState.Maximized; // Maximisation du formulaire
                frm.ShowDialog(); // Affichage du formulaire
            }
        }



        private DataTable GetEmployeeData(string employeeId)
        {
            DataTable dataTable = new DataTable();
            string query = "SELECT * FROM employees WHERE employee_id = @EmployeeId"; // Filtrer par l'ID de l'employé

            try
            {
                // Connexion à la base de données
                using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kananavy\Documents\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30"))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Ajout du paramètre avec le type varchar
                        command.Parameters.AddWithValue("@EmployeeId", employeeId); // Utilisation de l'ID comme string

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable); // Remplissage du DataTable avec les données récupérées
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue lors de la récupération des données : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable; // Retourne le DataTable rempli
        }


        private void salary_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        // Ajoutez cette méthode dans Salary.cs si elle manque
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            p1.Visible = radioButton1.Checked;
            p2.Visible = false;
            p1.Visible = true;
            p3.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            p1.Visible = false;
            p2.Visible = radioButton2.Checked;
            p2.Visible = true;
            p3.Visible = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            p1.Visible = false;
            p2.Visible = false;
            p3.Visible = radioButton3.Checked;
            p3.Visible = true;
        }

        private void salary_salary_Click1(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox6.Visible = false;
        }
        private void salary_code_Click(object sender, EventArgs e)
        {
            // Implémentez ici la logique à exécuter lors du clic sur salary_code
        }

    }
}
