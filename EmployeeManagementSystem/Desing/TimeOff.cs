using EmployeeManagementSystem.Formulaire;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class TimeOff : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");

        public TimeOff()
        {
            InitializeComponent();

            // ============================================TO DISPLAY THE DATA FROM DATABASE TO YOUR DATA GRID VIEW=================//
            displayEmployeeData();
            displayEmployeeData2();

            // ============================================TO DISPLAY THE DATA FROM DATABASE TO YOUR DEPARTEMENT=================//

            displayDEP();

            // ============================================TO DISPLAY THE DATA FROM DATABASE TO YOUR DEPARTEMENT=================//

            displayPOST();
            addEmployee_departement.SelectedIndexChanged += addEmployee_departement_SelectedIndexChanged;

            // Dans le constructeur ou une méthode de chargement du formulaire
            addEmployee_reliquat.Text = "30";
            addEmployee_reliquat.Enabled = false; // Rendre le champ non modifiable


        }
        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }
            displayEmployeeData();
            displayEmployeeData2();
            displayDEP();

            displayPOST();

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


        //============================================= ADD TO EMPLOYEE DATA DISPLAY============================//
        private void addEmployee_addBtn_Click_1(object sender, EventArgs e)
        {
            // Déclaration des variables pour les périodes
            DateTime periodeDebut;
            DateTime periodeFin;

            // Vérification des champs obligatoires et des dates
            if (string.IsNullOrEmpty(addEmployee_id.Text) ||
                string.IsNullOrEmpty(addEmployee_fullName.Text) ||
                string.IsNullOrEmpty(addEmployee_departement.Text) ||
                string.IsNullOrEmpty(addEmployee_position.Text) ||
                string.IsNullOrEmpty(addEmployee_nature.Text) ||
                string.IsNullOrEmpty(addEmployee_motifs.Text) ||
                string.IsNullOrEmpty(addEmployee_adresse.Text) ||
                !DateTime.TryParse(addEmployee_debut.Text.Trim(), out periodeDebut) || // Vérification de la date de début
                !DateTime.TryParse(addEmployee_fin.Text.Trim(), out periodeFin)) // Vérification de la date de fin
            {
                pictureBox6.Visible = false;
                pictureBox7.Visible = true;
                return;
            }

            // Vérification si la connexion est fermée et ouverture de celle-ci
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    // Récupérer la date de fin du dernier congé
                    DateTime lastEndDate = DateTime.MinValue;
                    string getLastEndDateQuery = @"
                SELECT MAX(periode_fin)
                FROM congeHistorique
                WHERE employee_id = @employeeID AND delete_date IS NULL";

                    using (SqlCommand getLastEndDateCmd = new SqlCommand(getLastEndDateQuery, connect))
                    {
                        getLastEndDateCmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        object result = getLastEndDateCmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            lastEndDate = (DateTime)result;
                        }
                    }

                    // Vérifiez si la date de début du nouveau congé est valide
                    if (periodeDebut <= lastEndDate)
                    {
                        MessageBox.Show($"La date de début doit être postérieure à la date de fin du dernier congé ({lastEndDate:dd/MM/yyyy}).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Vérifiez si la date de début est supérieure à la date de fin
                    if (periodeDebut > periodeFin)
                    {
                        MessageBox.Show("La date de début ne peut pas être après la date de fin.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Vérifier l'existence d'un congé pour la même période
                    string checkExistingLeaveQuery = @"
                SELECT COUNT(*) 
                FROM congeHistorique 
                WHERE employee_id = @employeeID 
                AND periode_debut = @periode_debut 
                AND periode_fin = @periode_fin 
                AND delete_date IS NULL";

                    using (SqlCommand checkExistingLeaveCmd = new SqlCommand(checkExistingLeaveQuery, connect))
                    {
                        checkExistingLeaveCmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        checkExistingLeaveCmd.Parameters.AddWithValue("@periode_debut", periodeDebut);
                        checkExistingLeaveCmd.Parameters.AddWithValue("@periode_fin", periodeFin);

                        int existingLeaveCount = (int)checkExistingLeaveCmd.ExecuteScalar();

                        if (existingLeaveCount > 0)
                        {
                            MessageBox.Show("Un congé pour cette période existe déjà pour cet employé.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Récupérer le dernier numéro de congé pour l'employé
                    string getLastNumCongeQuery = @"
                SELECT ISNULL(MAX(num_conge), 0)
                FROM congeHistorique
                WHERE employee_id = @employeeID AND delete_date IS NULL";

                    int newNumConge;

                    using (SqlCommand getLastNumCongeCmd = new SqlCommand(getLastNumCongeQuery, connect))
                    {
                        getLastNumCongeCmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        int lastNumConge = (int)getLastNumCongeCmd.ExecuteScalar();
                        newNumConge = lastNumConge + 1; // Incrémentation du numéro de congé
                    }

                    double initialLeaveBalance = 30; // Solde initial par défaut
                    double lastReliquat = initialLeaveBalance;

                    string getLastReliquatQuery = @"
                SELECT ISNULL(SUM(jour_conge), 0)
                FROM congeHistorique 
                WHERE employee_id = @employeeID AND delete_date IS NULL";

                    using (SqlCommand getLastReliquatCmd = new SqlCommand(getLastReliquatQuery, connect))
                    {
                        getLastReliquatCmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        object result = getLastReliquatCmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            lastReliquat = initialLeaveBalance - Convert.ToDouble(result);
                        }
                    }

                    // Calculer le nombre de jours de congé demandé
                    double daysDifference = (periodeFin - periodeDebut).Days + 1;

                    // Vérification du solde de congés
                    if (daysDifference > lastReliquat)
                    {
                        MessageBox.Show($"Le solde de congés est insuffisant. Le solde de congés est de {lastReliquat} jours.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Calculer le nouveau reliquat
                    if (addEmployee_nature.Text.Trim().ToLower() == "exceptionnel")
                    {
                        lastReliquat -= daysDifference;
                    }

                    // Insertion des données dans la table congeHistorique
                    string insertData = @"
                INSERT INTO congeHistorique
                (employee_id, full_name, departement, position, nature_conge, motif, adresse, jour_conge, reliquat, insert_date, periode_debut, periode_fin, num_conge)
                VALUES
                (@employeeID, @fullName, @departement, @position, @nature_conge, @motif, @adresse, @jour_conge, @reliquat, @insert_date, @periode_debut, @periode_fin, @num_conge)";

                    using (SqlCommand cmd = new SqlCommand(insertData, connect))
                    {
                        cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        cmd.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                        cmd.Parameters.AddWithValue("@departement", addEmployee_departement.Text.Trim());
                        cmd.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                        cmd.Parameters.AddWithValue("@nature_conge", addEmployee_nature.Text.Trim());
                        cmd.Parameters.AddWithValue("@motif", addEmployee_motifs.Text.Trim());
                        cmd.Parameters.AddWithValue("@adresse", addEmployee_adresse.Text.Trim());
                        cmd.Parameters.AddWithValue("@periode_debut", periodeDebut);
                        cmd.Parameters.AddWithValue("@periode_fin", periodeFin);
                        cmd.Parameters.AddWithValue("@jour_conge", daysDifference); // Stocker directement en double
                        cmd.Parameters.AddWithValue("@reliquat", lastReliquat); // Stocker directement en double
                        cmd.Parameters.AddWithValue("@insert_date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@num_conge", newNumConge);

                        cmd.ExecuteNonQuery();
                        displayEmployeeData2();
                    }

                    // Mise à jour de la table employees avec le nouveau reliquat
                    string updateEmployee = @"
                UPDATE employees 
                SET reliquat = @reliquat
                WHERE employee_id = @employeeID AND delete_date IS NULL";
                    using (SqlCommand cmd = new SqlCommand(updateEmployee, connect))
                    {
                        cmd.Parameters.AddWithValue("@reliquat", lastReliquat); // Stocker directement en double
                        cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    // Mettre à jour ou insérer dans la table conge
                    string upsertData = @"
                MERGE INTO conge AS target
                USING (
                    SELECT @employeeID AS employee_id, @fullName AS full_name, @departement AS departement, @position AS position,
                           @nature_conge AS nature_conge, @motif AS motif, @adresse AS adresse, @jour_conge AS jour_conge, @reliquat AS reliquat,
                           @insert_date AS insert_date, @periode_debut AS periode_debut, @periode_fin AS periode_fin, @num_conge AS num_conge
                ) AS source
                ON target.employee_id = source.employee_id
                WHEN MATCHED THEN
                    UPDATE SET
                        target.full_name = source.full_name,
                        target.departement = source.departement,
                        target.position = source.position,
                        target.nature_conge = source.nature_conge,
                        target.motif = source.motif,
                        target.adresse = source.adresse,
                        target.jour_conge = source.jour_conge,
                        target.reliquat = source.reliquat,
                        target.insert_date = source.insert_date,
                        target.periode_debut = source.periode_debut,
                        target.periode_fin = source.periode_fin,
                        target.num_conge = source.num_conge
                WHEN NOT MATCHED THEN
                    INSERT (employee_id, full_name, departement, position, nature_conge, motif, adresse, jour_conge, reliquat, insert_date, periode_debut, periode_fin, num_conge)
                    VALUES (source.employee_id, source.full_name, source.departement, source.position, source.nature_conge, source.motif, source.adresse, source.jour_conge, source.reliquat, source.insert_date, source.periode_debut, source.periode_fin, source.num_conge);";

                    using (SqlCommand upsertCmd = new SqlCommand(upsertData, connect))
                    {
                        upsertCmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@departement", addEmployee_departement.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@nature_conge", addEmployee_nature.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@motif", addEmployee_motifs.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@adresse", addEmployee_adresse.Text.Trim());
                        upsertCmd.Parameters.AddWithValue("@periode_debut", periodeDebut);
                        upsertCmd.Parameters.AddWithValue("@periode_fin", periodeFin);
                        upsertCmd.Parameters.AddWithValue("@jour_conge", daysDifference);
                        upsertCmd.Parameters.AddWithValue("@reliquat", lastReliquat);
                        upsertCmd.Parameters.AddWithValue("@insert_date", DateTime.Today);
                        upsertCmd.Parameters.AddWithValue("@num_conge", newNumConge);

                        upsertCmd.ExecuteNonQuery();
                    }

                    UpdateEmployeeStatus(addEmployee_id.Text.Trim(), false); // Passer à false pour inactif
                    UpdateEmployeeStatusconge1(addEmployee_fullName.Text.Trim(), false); // Passer à false pour inactif




                    // Afficher les données des employés
                    displayEmployeeData();

                    // Affichage des icônes
                    pictureBox6.Visible = true;
                    pictureBox7.Visible = false;
                    clearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Optionnel : Logger l'erreur
                }
                finally
                {
                    connect.Close();
                }
            }
        }




        public void displayEmployeeData()
        {
            List<CongeData1> congeList = new List<CongeData1>();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Récupérer la dernière entrée de congé pour chaque employé
                    string query = @"
                WITH LatestConge AS (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY employee_id ORDER BY insert_date DESC) AS rn
                    FROM conge
                    WHERE delete_date IS NULL
                )
                SELECT * 
                FROM LatestConge
                WHERE rn = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            CongeData1 cd = new CongeData1
                            {
                                EmployeeID = reader["employee_id"].ToString(),
                                Nom = reader["full_name"].ToString(),
                                Adresse = reader["adresse"].ToString(),
                                Departement = reader["departement"].ToString(),
                                Poste = reader["position"].ToString(),
                                nature = reader["nature_conge"].ToString(),
                                Motif = reader["motif"].ToString(),
                                Periode_debut = (DateTime)reader["periode_debut"],
                                Periode_fin = (DateTime)reader["periode_fin"],
                                Nun_conge = (int)reader["num_conge"]
                            };

                            // Formater le reliquat avec " jour(s)"
                            int reliquat = reader["reliquat"] != DBNull.Value ? Convert.ToInt32(reader["reliquat"]) : 0;
                            cd.reliquat = $"{reliquat} jour(s)";

                            // Formater le Numbrejour avec " jour(s)"
                            int jour_conge = reader["jour_conge"] != DBNull.Value ? Convert.ToInt32(reader["jour_conge"]) : 0;
                            cd.jour_conge = $"{jour_conge} jour(s)";

                            congeList.Add(cd);
                        }

                        dataGridView1.DataSource = congeList;
                        //dataGridView2.DataSource = congeList;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public void displayEmployeeData2()
        {
            List<CongeData1> congeList = new List<CongeData1>();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Récupérer la dernière entrée de congé pour chaque employé
                    string query = @"
                WITH LatestConge AS (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY employee_id ORDER BY num_conge ASC) AS rn
                    FROM congeHistorique
                    WHERE delete_date IS NULL
                )
                SELECT * 
                FROM LatestConge";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            CongeData1 cd = new CongeData1
                            {
                                EmployeeID = reader["employee_id"].ToString(),
                                Nom = reader["full_name"].ToString(),
                                Adresse = reader["adresse"].ToString(),
                                Departement = reader["departement"].ToString(),
                                Poste = reader["position"].ToString(),
                                nature = reader["nature_conge"].ToString(),
                                Motif = reader["motif"].ToString(),
                                Periode_debut = (DateTime)reader["periode_debut"],
                                Periode_fin = (DateTime)reader["periode_fin"],
                                Nun_conge = (int)reader["num_conge"]
                            };

                            // Formater le reliquat avec " jour(s)"
                            int reliquat = reader["reliquat"] != DBNull.Value ? Convert.ToInt32(reader["reliquat"]) : 0;
                            cd.reliquat = $"{reliquat} jour(s)";

                            // Formater le Numbrejour avec " jour(s)"
                            int jour_conge = reader["jour_conge"] != DBNull.Value ? Convert.ToInt32(reader["jour_conge"]) : 0;
                            cd.jour_conge = $"{jour_conge} jour(s)";

                            congeList.Add(cd);
                        }


                        dataGridView2.DataSource = congeList;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //============================================= UPDATE STATUS ACTIF TO INACTIF ============================//

        private void UpdateEmployeeStatus(string employeeId, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30"))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        connection.Open();
                        string updateStatusQuery = "UPDATE employees SET status = @status WHERE employee_id = @employeeId";

                        using (SqlCommand cmd = new SqlCommand(updateStatusQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@status", isActive ? "Active" : "Inactive");
                            cmd.Parameters.AddWithValue("@employeeId", employeeId);

                            cmd.ExecuteNonQuery();
                            displayEmployeeData();

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        //============================================= MAJ STATUS Présent -> Congé ============================//

        private void UpdateEmployeeStatusconge1(string addEmployee_fullName, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30"))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        connection.Open();
                        string updateStatusQuery = "UPDATE Attendance SET status = @status WHERE employee_name = @employee_name";

                        using (SqlCommand cmd = new SqlCommand(updateStatusQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@status", isActive ? "Active" : "Congé");
                            cmd.Parameters.AddWithValue("@employee_name", addEmployee_fullName.Trim());

                            cmd.ExecuteNonQuery();
                            displayEmployeeData();


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        //============================================= MAJ STATUS Présent -> Congé ============================//

        private void UpdateEmployeeStatusConge2(string addEmployee_fullName, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30"))
            {
                try
                {
                    connection.Open();
                    string updateStatusQuery = "UPDATE Attendance SET status = @status WHERE employee_name = @employee_name";

                    using (SqlCommand cmd = new SqlCommand(updateStatusQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@status", isActive ? "Congé" : "Présent");
                        cmd.Parameters.AddWithValue("@employee_name", addEmployee_fullName.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // displayEmployeeData();
                            // MessageBox.Show("Le statut de l'employé a été mis à jour avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Aucun employé trouvé avec ce nom.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message, "Message d'erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //============================================= UPDATE STATUS INACTIF TO ACTIF============================//

        private void UpdateEmployeeStatus3(string employeeId, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30"))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        connection.Open();
                        string updateStatusQuery = "UPDATE employees SET status = @status WHERE employee_id = @employeeId";

                        using (SqlCommand cmd = new SqlCommand(updateStatusQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@status", isActive ? "Active" : "Inactive");
                            cmd.Parameters.AddWithValue("@employeeId", employeeId);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                Debug.WriteLine($"ID: {row.Cells[0].Value}");
                Debug.WriteLine($"Full Name: {row.Cells[1].Value}");
                // Répétez pour les autres colonnes

                addEmployee_id.Text = row.Cells[0].Value?.ToString() ?? string.Empty;
                addEmployee_fullName.Text = row.Cells[1].Value?.ToString() ?? string.Empty;
                addEmployee_adresse.Text = row.Cells[2].Value?.ToString() ?? string.Empty;
                addEmployee_departement.Text = row.Cells[3].Value?.ToString() ?? string.Empty;
                addEmployee_position.Text = row.Cells[4].Value?.ToString() ?? string.Empty;
                addEmployee_nature.Text = row.Cells[5].Value?.ToString() ?? string.Empty;
                addEmployee_motifs.Text = row.Cells[6].Value?.ToString() ?? string.Empty;
                addEmployee_debut.Text = row.Cells[7].Value?.ToString() ?? string.Empty;
                addEmployee_fin.Text = row.Cells[8].Value?.ToString() ?? string.Empty;
                addEmployee_reliquat.Text = row.Cells[10].Value?.ToString() ?? string.Empty;

                pictureBox6.Visible = false;
                pictureBox7.Visible = false;

            }
        }

        //====================================POST GRIDVIEW============================================//
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                Debug.WriteLine($"ID: {row.Cells[0].Value}");
                Debug.WriteLine($"Full Name: {row.Cells[1].Value}");
                // Répétez pour les autres colonnes

                addEmployee_id.Text = row.Cells[0].Value?.ToString() ?? string.Empty;
                addEmployee_fullName.Text = row.Cells[1].Value?.ToString() ?? string.Empty;
                addEmployee_adresse.Text = row.Cells[2].Value?.ToString() ?? string.Empty;
                addEmployee_departement.Text = row.Cells[3].Value?.ToString() ?? string.Empty;
                addEmployee_position.Text = row.Cells[4].Value?.ToString() ?? string.Empty;
                addEmployee_nature.Text = row.Cells[5].Value?.ToString() ?? string.Empty;
                addEmployee_motifs.Text = row.Cells[6].Value?.ToString() ?? string.Empty;
                addEmployee_debut.Text = row.Cells[7].Value?.ToString() ?? string.Empty;
                addEmployee_fin.Text = row.Cells[8].Value?.ToString() ?? string.Empty;
                addEmployee_reliquat.Text = row.Cells[10].Value?.ToString() ?? string.Empty;

                pictureBox6.Visible = false;
                pictureBox7.Visible = false;

            }
        }


        //============================================= BUTTOM_CLICK CLEAR============================//

        public void clearFields()
        {
            addEmployee_id.Text = "";
            addEmployee_fullName.Text = "";
            addEmployee_departement.SelectedIndex = -1;
            addEmployee_position.SelectedIndex = -1;
            addEmployee_nature.SelectedIndex = -1;
            addEmployee_motifs.Text = "";
            addEmployee_adresse.Text = "";
            addEmployee_debut.Text = "";
            addEmployee_fin.Text = "";
            addEmployee_reliquat.Text = "30";


        }

        //============================================= BUTTOM_CLICK DELETE============================//


        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == ""
                || addEmployee_fullName.Text == ""
                || addEmployee_departement.Text == ""
                || addEmployee_position.Text == ""
                || addEmployee_nature.Text == ""
                || addEmployee_motifs.Text == ""
                || addEmployee_adresse.Text == ""
                || addEmployee_reliquat.Text == "")
            {
                pictureBox6.Visible = false;
                pictureBox7.Visible = true;

            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to DELETE " +
                    "Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();

                        // Supprimer le congé de la base de données
                        string deleteData = "DELETE FROM conge WHERE employee_id = @employeeID";
                        using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                        {
                            cmd.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        string employeeId = addEmployee_id.Text.Trim(); // Obtenez l'ID de l'employé sélectionné
                        if (string.IsNullOrEmpty(employeeId))
                        {
                            MessageBox.Show("Please select an employee.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Mettre à jour le statut de l'employé en fonction de la suppression du congé
                        UpdateEmployeeStatus3(employeeId, true); // Passer à `true` pour actif
                                                                 // Mettre à jour le statut de l'employé pour le passer en "Présent"
                        UpdateEmployeeStatusConge2(addEmployee_fullName.Text.Trim(), false); // Présent


                        pictureBox6.Visible = true;
                        pictureBox7.Visible = false;


                        displayEmployeeData();
                        clearFields();
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
                else
                {
                    MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;

        }

        private void addEmployee_id_Click(object sender, EventArgs e)
        {
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;

        }

        private void TimeOff_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private DataTable GetEmployeeData(string employeeId)
        {
            DataTable dataTable = new DataTable();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kananavy\Documents\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";
            // Modification de la requête pour filtrer par employee_id
            string query = "SELECT id, employee_id, full_name, departement, position, adresse, motif, reliquat, nature_conge, insert_date, periode_debut, periode_fin, jour_conge FROM conge WHERE employee_id = @employeeId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Ajout du paramètre à la requête
                        command.Parameters.AddWithValue("@employeeId", employeeId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
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
            // Récupérer l'ID de l'employé à partir de l'interface
            string employeeId = addEmployee_id.Text; // Assurez-vous que ce champ contient l'ID correct

            // Créer une instance du formulaire de prévisualisation
            using (fprint frm = new fprint())
            {
                // Configurer le rapport
                ReportDataSource reportDataSource = new ReportDataSource
                {
                    Name = "Conge", // Nom du DataSet défini dans RDLC
                    Value = GetEmployeeData(employeeId) // Les données récupérées pour cet employé
                };

                // Chemin complet du fichier RDLC
                string reportPath = @"C:\Users\kananavy\Pictures\Teste2\SGE\EmployeeManagementSystem\Repport\RpConge.rdlc";

                // Assurez-vous que le fichier RDLC est correctement ajouté au projet et le chemin est relatif au projet ou accessible
                frm.Rpv.LocalReport.ReportPath = reportPath;
                frm.Rpv.LocalReport.DataSources.Clear();
                frm.Rpv.LocalReport.DataSources.Add(reportDataSource);

                // Vérifiez que le paramètre existe dans le rapport
                bool paramExists = false;
                /*    foreach (var param in frm.Rpv.LocalReport.GetParameters())
                    {
                        if (param.Name == "PaperSize")
                        {
                            paramExists = true;
                            break;
                        }
                    }*/

                if (paramExists)
                {
                    frm.Rpv.LocalReport.SetParameters(new ReportParameter[]
                    {
        new ReportParameter("PaperSize", "A4")
                    });
                }


                // Rafraîchir le rapport
                frm.Rpv.RefreshReport();

                // Afficher le formulaire de prévisualisation
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();

            }




        }


        private void Hirsotique_Click(object sender, EventArgs e)
        {
            panelHistorique.Visible = true;
            panel1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            ligneConge.Visible = false;
            ligneHistorique.Visible = true;


        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel3.Visible = true;
            panelHistorique.Visible = true;
            panel4.Visible = true;
            ligneConge.Visible = true;
            ligneHistorique.Visible = false;


        }
    }
}
