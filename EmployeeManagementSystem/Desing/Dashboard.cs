using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class Dashboard : UserControl
    {

        private SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        private Timer refreshTimer;
        private int currentEmployeeIndex = 0;


        public Dashboard()
        {

            InitializeComponent();
            LoadDepartments();
            RefreshData();

        }


        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }

            DisplayRandomEmployees(); // Changement des employeur
            displayTE();
            displayAE();
            displayIE();
            LoadDepartments();

        }


        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }


        private void Dashboard_Load(object sender, EventArgs e)
        {
            // Initialisation et configuration du Timer
            refreshTimer = new Timer
            {
                Interval = 5000 // 5000 millisecondes = 5 secondes
            };
            refreshTimer.Start();
            refreshTimer.Tick += RefreshTimer_Tick;
        }


        // DISPLAY EMPLOYE TOTAL
        public void displayTE()
        {
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM employees WHERE delete_date IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int count = Convert.ToInt32(reader[0]);
                            dashboard_TE.Text = count.ToString(); // Assurez-vous que dashboard_TE est un contrôle Label ou TextBox
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in displayTE: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }


        // DISPLAY EMPLOYE ACTIVE
        public void displayAE()
        {

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM employees WHERE status = @status AND delete_date IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@status", "Active");
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int count = Convert.ToInt32(reader[0]);
                            dashboard_AE.Text = count.ToString(); // Assurez-vous que dashboard_AE est un contrôle Label ou TextBox
                            dashboard_AE.ForeColor = System.Drawing.Color.Green;
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in displayAE: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }


        // DISPLAY EMPLOYE INACTIVE
        public void displayIE()
        {
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM employees WHERE status = @status AND delete_date IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@status", "Inactive");
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int count = Convert.ToInt32(reader[0]);
                            dashboard_IE.Text = count.ToString(); // Assurez-vous que dashboard_IE est un contrôle Label ou TextBox
                            dashboard_IE.ForeColor = System.Drawing.Color.Red;
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in displayIE: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }


        // LOAD EMPLOYEE DATA
        private void LoadDepartments()
        {
            string department = NomDepart.Text.Trim(); // Utiliser le texte du label NomDepart
            NomDepart.Visible = false;

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    // Compter le nombre total d'employés dans l'entreprise
                    string totalEmployeesQuery = "SELECT COUNT(id) FROM employees WHERE delete_date IS NULL";
                    int totalEmployeesInCompany = 0;
                    using (SqlCommand totalCmd = new SqlCommand(totalEmployeesQuery, connect))
                    {
                        totalEmployeesInCompany = (int)totalCmd.ExecuteScalar();
                    }

                    // Compter le nombre d'employés dans le département spécifié
                    string countQuery = "SELECT COUNT(id) FROM employees WHERE departement = @department AND delete_date IS NULL";
                    int totalEmployeesInDepartment = 0;
                    using (SqlCommand countCmd = new SqlCommand(countQuery, connect))
                    {
                        countCmd.Parameters.AddWithValue("@department", department);
                        totalEmployeesInDepartment = (int)countCmd.ExecuteScalar();
                    }

                    // Calculer le pourcentage d'employés dans le département par rapport au total des employés
                    if (totalEmployeesInCompany > 0)
                    {
                        double percentage = ((double)totalEmployeesInDepartment / totalEmployeesInCompany) * 100;
                        lblPercentage.Text = $"{percentage:0.00}%";
                        lblPercentage.Visible = true; // Afficher le label du pourcentage
                    }
                    else
                    {
                        lblPercentage.Text = "0%";
                        lblPercentage.Visible = true;
                    }

                    // Charger les employés de manière aléatoire
                    string query = "SELECT departement FROM employees WHERE delete_date IS NULL ORDER BY NEWID()";
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Afficher le nom du département
                            NomDepart.Text = reader["departement"].ToString();
                        }
                        reader.Close();
                        RefreshData();
                        DisplayRandomEmployees();
                    }

                    if (string.IsNullOrEmpty(department))
                    {
                        ClearFields(); // Nettoyer les champs si le département est vide
                        return;
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in LoadDepartments: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }



        private void DisplayRandomEmployees()
        {
            string department = NomDepart.Text.Trim(); // Utiliser le texte du label NomDepart
            if (string.IsNullOrEmpty(department))
            {
                ClearFields(); // Nettoyer les champs si le département est vide
                return;
            }

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    // Récupérer tous les employés du département donné, triés par employee_id
                    string query = @"
                                SELECT * FROM employees
                                WHERE departement = @department AND delete_date IS NULL
                                ORDER BY employee_id ASC";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@department", department);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable employeeTable = new DataTable();
                        adapter.Fill(employeeTable); // Remplir le DataTable avec les résultats de la requête

                        if (employeeTable.Rows.Count == 0)
                        {
                            ClearFields();
                            // InitializeComponent(); // Teste apres.................................
                            return;
                        }

                        // Si l'index actuel dépasse la taille de la liste, on revient au début
                        if (currentEmployeeIndex >= employeeTable.Rows.Count)
                        {
                            currentEmployeeIndex = 0;
                        }

                        // Récupérer l'employé actuel
                        DataRow currentEmployee = employeeTable.Rows[currentEmployeeIndex];

                        // Afficher les données de l'employé
                        lblEmployeeID.Text = currentEmployee["employee_id"].ToString();
                        lblFullName.Text = currentEmployee["full_name"].ToString();
                        lblGender.Text = currentEmployee["gender"].ToString();
                        lblContactNumber.Text = currentEmployee["contact_number"].ToString();
                        lblAdresse.Text = currentEmployee["adresse"].ToString();
                        lblCIN.Text = currentEmployee["cin"].ToString();
                        lblBirthDay.Text = Convert.ToDateTime(currentEmployee["birth_day"]).ToString("dd/MM/yyyy");
                        lblBirthPlace.Text = currentEmployee["birth_place"].ToString();
                        lblNationality.Text = currentEmployee["nationality"].ToString();
                        lblDiploma.Text = currentEmployee["diploma"].ToString();
                        lblDepartement.Text = currentEmployee["departement"].ToString();
                        lblDepartement2.Text = currentEmployee["departement"].ToString();
                        lblPosition.Text = currentEmployee["position"].ToString();
                        lblGrade.Text = currentEmployee["grade"].ToString();
                        lblDateRecrute.Text = Convert.ToDateTime(currentEmployee["date_recrute"]).ToString("dd/MM/yyyy");

                        // Charger l'image de l'employé
                        string imagePath = currentEmployee["image"].ToString();
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                        {
                            pbEmployeeImage.Image = Image.FromFile(imagePath);
                        }
                        else
                        {
                            pbEmployeeImage.Image = null;
                        }

                        // Changer la couleur des panels selon le statut
                        string status = currentEmployee["status"].ToString();
                        if (status == "Active")
                        {
                            panel7.BackColor = Color.Green;
                            panel10.BackColor = Color.Green;
                        }
                        else if (status == "Inactive")
                        {
                            panel7.BackColor = Color.Red;
                            panel10.BackColor = Color.Red;
                        }

                        // Incrémenter l'index pour le prochain employé
                        currentEmployeeIndex++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in DisplayNextEmployee: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }

        }


        // CLEAR FIELDS
        public void ClearFields()
        {
            lblEmployeeID.Text = "";
            lblFullName.Text = "";
            lblGender.Text = "";
            lblContactNumber.Text = "";
            lblAdresse.Text = "";
            lblCIN.Text = "";
            lblBirthDay.Text = "";
            lblBirthPlace.Text = "";
            lblNationality.Text = "";
            lblDiploma.Text = "";
            lblDepartement.Text = "";
            lblDepartement2.Text = "";
            lblPosition.Text = "";
            lblGrade.Text = "";
            lblDateRecrute.Text = "";
            pbEmployeeImage.Image = null;
        }


    }
}
