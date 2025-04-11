﻿using EmployeeManagementSystem.Formulaire;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;


namespace EmployeeManagementSystem
{
    public partial class AddEmployee : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        private System.Windows.Forms.Timer updateTimer; // Spécifiez que c'est System.Windows.Forms.Timer
        private EmployeeData employeeData = new EmployeeData();


        public AddEmployee()
        {
            InitializeComponent();
            StartReliquatTimer();
            InitializeSearchCriteriaComboBox();

            displayDEP();
            displayPOST();
            displayEmployeeData();
            addEmployee_departement.SelectedIndexChanged += addEmployee_departement_SelectedIndexChanged;
        }

        private void AddEmployee_Load(object sender, EventArgs e)
        {
            // Mise à jour automatique du reliquat en début d'année
            employeeData.UpdateReliquatAnnuel();
        }
        private void StartReliquatTimer()
        {
            updateTimer = new System.Windows.Forms.Timer(); // Utilise System.Windows.Forms.Timer
            updateTimer.Interval = 86400000; // Exécution toutes les 24 heures
                                             // updateTimer.Tick += OnTimedEvent; // Abonnement à l'événement Tick
            updateTimer.Start(); // Démarre le timer
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Mise à jour annuelle du reliquat
            employeeData.UpdateReliquatAnnuel();
        }

        // ACTUALISATION
        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }
            displayEmployeeData();
            displayDEP();
            displayPOST();

        }

        // DISPLAY LIST
        public void displayEmployeeData()
        {
            EmployeeData ed = new EmployeeData();
            List<EmployeeData> listData = ed.employeeListData("insert_date");

            dataGridView1.DataSource = listData;
        }


        //LOAD DEPARTEMENT AND CHANGE DEPART 
        public void LoadPositionsByDepartment(string departmentName)
        {
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    // Requête pour sélectionner les postes associés au département
                    string selectData = "SELECT poste_name FROM poste WHERE liste_dep = @depart_name";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@depart_name", departmentName);

                        SqlDataReader reader = cmd.ExecuteReader();

                        // Nettoyer les éléments existants avant d'ajouter les nouveaux postes
                        addEmployee_position.Items.Clear();


                        while (reader.Read())
                        {
                            string posteName = reader["poste_name"].ToString();

                            // Ajouter chaque poste associé au département sélectionné
                            if (!addEmployee_position.Items.Contains(posteName))
                            {
                                addEmployee_position.Items.Add(posteName);

                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void addEmployee_departement_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Vérifiez si un élément est sélectionné avant d'essayer de l'utiliser
            if (addEmployee_departement.SelectedItem != null)
            {
                // Obtenir le département sélectionné
                string selectedDepartment = addEmployee_departement.SelectedItem.ToString();

                // Charger les postes pour ce département
                LoadPositionsByDepartment(selectedDepartment);
            }

        }

        public void DisplayData(string query, ComboBox comboBox)
        {
            comboBox.Items.Clear();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string item = reader[0].ToString();
                            if (!comboBox.Items.Contains(item))
                            {
                                comboBox.Items.Add(item);
                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        //DISPLAY DEPARTEMENT AND POSITION
        public void displayDEP()
        {
            string query = "SELECT depart_name FROM departement";
            DisplayData(query, addEmployee_departement);
        }

        public void displayPOST()
        {
            string query = "SELECT poste_name FROM poste";
            DisplayData(query, addEmployee_position);
        }


        // IMPORT TO IMAGE
        private void addEmployee_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                string imagePath = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addEmployee_picture.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // BUTTOM ADD
        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == ""
                || addEmployee_fullName.Text == ""
                || addEmployee_gender.Text == ""
                || addEmployee_adresse.Text == ""
                || addEmployee_phoneNum.Text == ""
                || addEmployee_Cin.Text == ""
                || addEmployee_lieuNaissance.Text == ""
                || addEmployee_nationalite.Text == ""
                || addEmployee_diplome.Text == ""
                || addEmployee_departement.Text == ""
                || addEmployee_position.Text == ""
                || addEmployee_grade.Text == ""
                || addEmployee_status.Text == ""
                || addEmployee_picture.Image == null)
            {
                pictureBox6.Visible = false;
                pictureBox7.Visible = true;
                pictureBox8.Visible = false;
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();
                        string checkEmID = "SELECT COUNT(*) FROM employees WHERE employee_id = @emID AND delete_date IS NULL";

                        using (SqlCommand checkEm = new SqlCommand(checkEmID, connect))
                        {
                            checkEm.Parameters.AddWithValue("@emID", addEmployee_id.Text.Trim());
                            int count = (int)checkEm.ExecuteScalar();

                            if (count >= 1)
                            {
                                pictureBox7.Visible = false;
                                pictureBox6.Visible = false;
                                pictureBox8.Visible = true;
                            }
                            else
                            {
                                DateTime today = DateTime.Today;

                                TimeSpan heures_supplementaires = new TimeSpan(0, 0, 0); // 0 heures, 0 minutes, 0 secondes
                                                                                         // Conversion des heures supplémentaires en heures décimales
                                decimal heures_supplementairesDecimal = (decimal)heures_supplementaires.TotalHours;

                                // Formatage de l'affichage
                                string heuresSupplementairesString = $"{heures_supplementaires.Hours:D2}h{heures_supplementaires.Minutes:D2}min";


                                double reliquatInitial = 30; // Valeur par défaut de 30 jours
                                string slaryInitial = "0 Ar";
                                string insertData = "INSERT INTO employees " +
                                    "(employee_id, full_name, gender, adresse, contact_number" +
                                    ", cin , birth_day, birth_place, nationality, diploma" +
                                    ", departement, position, grade" +
                                    ", image, gross_salary, heures_supplementaires,insert_date, date_recrute, status, reliquat) " +  // Ajout de 'reliquat'
                                    "VALUES(@employeeID, @fullName, @gender, @adresse, @contactNum" +
                                    ", @cin, @birth_day, @birth_place, @nationality, @diploma" +
                                    ", @departement, @position, @grade" +
                                    ", @image, @gross_salary, @heures_supplementaires,@insertDate, @date_recrute, @status, @reliquat)"; // Paramètre reliquat ajouté

                                string path = Path.Combine(@"C:\Users\kananavy\Images\Employee-Management-System-in-CSharp-main\Employee-Management-System-in-CSharp-main\EmployeeManagementSystem\EmployeeManagementSystem\Directory\"
                                    + addEmployee_id.Text.Trim() + ".jpg");

                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(addEmployee_picture.ImageLocation, path, true);

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@gender", addEmployee_gender.Text.Trim());
                                    cmd.Parameters.AddWithValue("@contactNum", addEmployee_phoneNum.Text.Trim());
                                    cmd.Parameters.AddWithValue("@adresse", addEmployee_adresse.Text.Trim());
                                    cmd.Parameters.AddWithValue("@cin", addEmployee_Cin.Text.Trim());
                                    cmd.Parameters.AddWithValue("@birth_day", DateTime.Parse(addEmployee_dateNaissance.Text.Trim()));
                                    cmd.Parameters.AddWithValue("@birth_place", addEmployee_lieuNaissance.Text.Trim());
                                    cmd.Parameters.AddWithValue("@nationality", addEmployee_nationalite.Text.Trim());
                                    cmd.Parameters.AddWithValue("@diploma", addEmployee_diplome.Text.Trim());
                                    cmd.Parameters.AddWithValue("@departement", addEmployee_departement.Text.Trim());
                                    cmd.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                                    cmd.Parameters.AddWithValue("@grade", addEmployee_grade.Text.Trim());
                                    cmd.Parameters.AddWithValue("@image", path);
                                    cmd.Parameters.AddWithValue("@gross_salary", slaryInitial);
                                    cmd.Parameters.AddWithValue("@heures_supplementaires", heures_supplementaires);
                                    cmd.Parameters.AddWithValue("@insertDate", today);
                                    cmd.Parameters.AddWithValue("@date_recrute", DateTime.Parse(addEmployee_recrute.Text.Trim()));
                                    cmd.Parameters.AddWithValue("@status", addEmployee_status.Text.Trim());
                                    cmd.Parameters.AddWithValue("@reliquat", reliquatInitial); // Assignation du reliquat initial

                                    cmd.ExecuteNonQuery();

                                    displayEmployeeData();

                                    pictureBox6.Visible = true;
                                    pictureBox7.Visible = false;
                                    pictureBox8.Visible = false;

                                    clearFields();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }


        // BUTTOM UPDATE
        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == ""
                || addEmployee_fullName.Text == ""
                || addEmployee_gender.Text == ""
                || addEmployee_adresse.Text == ""
                || addEmployee_phoneNum.Text == ""
                || addEmployee_Cin.Text == ""
                || addEmployee_lieuNaissance.Text == ""
                || addEmployee_nationalite.Text == ""
                || addEmployee_diplome.Text == ""
                || addEmployee_departement.Text == ""
                || addEmployee_position.Text == ""
                || addEmployee_grade.Text == ""
                || addEmployee_status.Text == ""
                || addEmployee_picture.Image == null)
            {
                pictureBox6.Visible = false;
                pictureBox7.Visible = true;
                pictureBox8.Visible = false;
            }
            else
            {
                DialogResult check = MessageBox.Show("Etes-vous sûr de vouloir METTRE À JOUR " +
                "Identifiant employé : " + addEmployee_id.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();
                        DateTime today = DateTime.Today;

                        string updateData = "UPDATE employees SET full_name = @fullName" +
                            ", gender = @gender, contact_number = @contactNum" +
                            ", adresse = @adresse, cin = @cin, birth_day = @birth_day" +
                            ",birth_place = @birth_place, nationality = @nationality, diploma = @diploma, departement = @departement" +
                            ", position = @position, grade = @grade, update_date = @updateDate, status = @status , date_recrute = @date_recrute WHERE employee_id = @employeeID";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                            cmd.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                            cmd.Parameters.AddWithValue("@gender", addEmployee_gender.Text.Trim());
                            cmd.Parameters.AddWithValue("@contactNum", addEmployee_phoneNum.Text.Trim());
                            cmd.Parameters.AddWithValue("@adresse", addEmployee_adresse.Text.Trim());
                            cmd.Parameters.AddWithValue("@cin", addEmployee_Cin.Text.Trim());
                            cmd.Parameters.AddWithValue("@birth_day", DateTime.Parse(addEmployee_dateNaissance.Text.Trim()));
                            cmd.Parameters.AddWithValue("@birth_place", addEmployee_lieuNaissance.Text.Trim());
                            cmd.Parameters.AddWithValue("@nationality", addEmployee_nationalite.Text.Trim());
                            cmd.Parameters.AddWithValue("@diploma", addEmployee_diplome.Text.Trim());
                            cmd.Parameters.AddWithValue("@departement", addEmployee_departement.Text.Trim());
                            cmd.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                            cmd.Parameters.AddWithValue("@grade", addEmployee_grade.Text.Trim());
                            cmd.Parameters.AddWithValue("@salary", 0);
                            cmd.Parameters.AddWithValue("@updateDate", today);
                            cmd.Parameters.AddWithValue("@date_recrute", DateTime.Parse(addEmployee_recrute.Text.Trim()));
                            cmd.Parameters.AddWithValue("@status", addEmployee_status.Text.Trim());


                            cmd.ExecuteNonQuery();

                            displayEmployeeData();
                            pictureBox6.Visible = true;
                            pictureBox7.Visible = false;
                            pictureBox8.Visible = false;

                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex
                        , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Cancelled."
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }


        // BUTTOM DELETE
        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == ""
                || addEmployee_fullName.Text == ""
                || addEmployee_gender.Text == ""
                || addEmployee_adresse.Text == ""
                || addEmployee_phoneNum.Text == ""
                || addEmployee_Cin.Text == ""
                || addEmployee_lieuNaissance.Text == ""
                || addEmployee_nationalite.Text == ""
                || addEmployee_diplome.Text == ""
                || addEmployee_departement.Text == ""
                || addEmployee_position.Text == ""
                || addEmployee_grade.Text == ""
                || addEmployee_status.Text == ""
                || addEmployee_picture.Image == null)


            {
                pictureBox6.Visible = false;
                pictureBox7.Visible = true;
                pictureBox8.Visible = false;
            }
            else
            {
                DialogResult check = MessageBox.Show("Etes-vous sûr de vouloir SUPPRIMER " +
                "Identifiant employé : " + addEmployee_id.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();
                        DateTime today = DateTime.Today;


                        string deleteData = "DELETE FROM employees WHERE employee_id = @employeeID";

                        using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                        {
                            cmd.Parameters.AddWithValue("@deleteDate", today);
                            cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());

                            cmd.ExecuteNonQuery();

                            displayEmployeeData();

                            pictureBox6.Visible = true;
                            pictureBox7.Visible = false;
                            pictureBox8.Visible = false;

                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex
                        , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Cancelled."
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }


        // BUTTOM CLEAR
        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
        }

        // FUNCTION CLEAR
        public void clearFields()
        {
            addEmployee_id.Text = "";
            addEmployee_fullName.Text = "";
            addEmployee_gender.Text = "";
            addEmployee_adresse.Text = "";
            addEmployee_phoneNum.Text = "";
            addEmployee_Cin.Text = "";
            addEmployee_lieuNaissance.Text = "";
            addEmployee_nationalite.Text = "";
            addEmployee_diplome.Text = "";
            addEmployee_departement.SelectedIndex = -1;
            addEmployee_position.SelectedIndex = -1;
            addEmployee_grade.Text = "";
            addEmployee_status.SelectedIndex = -1;
            addEmployee_picture.Image = null;
            addEmployee_dateNaissance.Text = "";
            addEmployee_recrute.Text = "";
        }


        // DATA GRID VIEW
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                addEmployee_id.Text = row.Cells[0].Value.ToString();
                addEmployee_fullName.Text = row.Cells[1].Value.ToString();
                addEmployee_gender.Text = row.Cells[2].Value.ToString();
                addEmployee_phoneNum.Text = row.Cells[3].Value.ToString();
                addEmployee_adresse.Text = row.Cells[4].Value.ToString();
                addEmployee_Cin.Text = row.Cells[5].Value.ToString();
                addEmployee_dateNaissance.Text = row.Cells[6].Value.ToString();
                addEmployee_lieuNaissance.Text = row.Cells[7].Value.ToString();
                addEmployee_nationalite.Text = row.Cells[8].Value.ToString();
                addEmployee_diplome.Text = row.Cells[9].Value.ToString();
                addEmployee_departement.Text = row.Cells[10].Value.ToString();
                addEmployee_position.Text = row.Cells[11].Value.ToString();
                addEmployee_grade.Text = row.Cells[12].Value.ToString();

                string imagePath = row.Cells[15].Value.ToString();

                if (imagePath != null)
                {
                    addEmployee_picture.Image = Image.FromFile(imagePath);
                }
                else
                {
                    addEmployee_picture.Image = null;
                }
                addEmployee_recrute.Text = row.Cells[16].Value.ToString();
                addEmployee_status.Text = row.Cells[19].Value.ToString();


                //displayEmployeeData();

                pictureBox6.Visible = false;
                pictureBox7.Visible = false;
                pictureBox8.Visible = false;

            }
        }


        //SETTING
        private void addEmployee_id_Click_1(object sender, EventArgs e)
        {
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            panel_info1.Visible = true;
            panel_info2.Visible = false;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            panel_info1.Visible = false;
            panel_info2.Visible = true;
        }


        //BAR DE RECHERCHE
        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim().ToLower();
            string searchCriteria = searchCriteriaComboBox.SelectedItem.ToString();

            // Si aucun terme de recherche n'est entré et le critère n'est pas "Tous", affichez un message
            if (string.IsNullOrEmpty(searchTerm) && searchCriteria != "Tous")
            {
                MessageBox.Show("Please enter a search term.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Recharger toutes les données si le critère est "Tous"
            if (searchCriteria == "Tous")
            {
                LoadEmployeeData();
            }
            else
            {
                // Filtrer les données selon les critères et le terme de recherche
                FilterEmployeeData(searchTerm, searchCriteria);
            }
        }

        private void FilterEmployeeData(string searchTerm, string searchCriteria)
        {
            List<EmployeeData> filteredList = new List<EmployeeData>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.DataBoundItem is EmployeeData employee)
                {
                    bool matches = false;

                    switch (searchCriteria)
                    {
                        case "ID":
                            matches = employee.EmployeeID.ToLower().Contains(searchTerm);
                            break;
                        case "Nom":
                            matches = employee.Nom.ToLower().Contains(searchTerm);
                            break;
                        case "Département":
                            matches = employee.Departement.ToLower().Contains(searchTerm);
                            break;
                        default:
                            MessageBox.Show("Please select a valid search criteria.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;

                    }

                    if (matches)
                    {
                        filteredList.Add(employee);
                    }

                }
                RefreshData();
            }

            // Mettre à jour le DataGridView avec les résultats filtrés
            dataGridView1.DataSource = filteredList;
        }

        public void DisplaySearchResults(string query, string searchTerm)
        {
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        if (!query.Contains("Tous"))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }

            }
        }

        public void InitializeSearchCriteriaComboBox()
        {
            searchCriteriaComboBox.Items.Add("Tous");
            searchCriteriaComboBox.Items.Add("ID");
            searchCriteriaComboBox.Items.Add("Nom");
            searchCriteriaComboBox.Items.Add("Département");
            searchCriteriaComboBox.SelectedIndex = 0; // Par défaut sur "Tous"

        }

        private void LoadEmployeeData()
        {
            EmployeeData employeeData = new EmployeeData();
            List<EmployeeData> employees = employeeData.employeeListData("");

            dataGridView1.DataSource = employees;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            searchCriteriaComboBox.SelectedIndex = 0; // Par défaut sur "Tous"

            //searchCriteriaComboBox.SelectedIndex = -1;
            RefreshData();
        }

        // PRINT AND APERÇU
        private DataTable GetEmployeeData()
        {
            DataTable dataTable = new DataTable();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kananavy\Documents\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";
            string query = "SELECT id, employee_id, full_name, gender, contact_number, adresse, departement, position, image, gross_salary, insert_date, date_recrute, status, update_date, delete_date FROM employees";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue lors de la récupération des données : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        private void addEmployee_printBtn_Click(object sender, EventArgs e)
        {
            // Créer une instance du formulaire de prévisualisation
            using (fprint frm = new fprint())
            {
                // Configurer le rapport
                ReportDataSource reportDataSource = new ReportDataSource
                {
                    Name = "DataSetListeemployee", // Nom du DataSet défini dans RDLC
                    Value = GetEmployeeData() // Les données récupérées
                };

                // Chemin complet du fichier RDLC
                string reportPath = @"C:\Users\kananavy\Pictures\Teste2\SGE\EmployeeManagementSystem\Repport\RpListemploye.rdlc";

                // Assurez-vous que le fichier RDLC est correctement ajouté au projet et le chemin est relatif au projet ou accessible
                frm.Rpv.LocalReport.ReportPath = reportPath;
                frm.Rpv.LocalReport.DataSources.Clear();
                frm.Rpv.LocalReport.DataSources.Add(reportDataSource);

                // Rafraîchir le rapport
                frm.Rpv.RefreshReport();
                frm.Dock = DockStyle.Fill;

                // Afficher le formulaire de prévisualisation
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
            }
        }



    }
}
