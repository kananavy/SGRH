using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Desing
{
    public partial class Attendance : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        AttendanceData attendanceData = new AttendanceData(); // Instance de AttendanceData

        public Attendance()
        {
            InitializeComponent();
            displayEmployees();
            displayEmployees2();
            LoadEmployeeNames(); // Charge les noms d'employés au démarrage
            DateNow.Value = DateTime.Now; // Définit la date actuelle par défaut
            RefreshData();
        }

        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }

            displayEmployees2();
            displayEmployees();
            LoadEmployeeNames();

        }
        public void DisplayData(string query, ComboBox comboBox)
        {

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


        private void LoadEmployeeNames()
        {
            string query = "SELECT full_name FROM employees";
            DisplayData(query, EmployeeName);


        }


        public void displayEmployees()
        {
            List<AttendanceData> attendanceList = new List<AttendanceData>();
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
                           ROW_NUMBER() OVER (PARTITION BY employee_name ORDER BY date DESC) AS rn
                    FROM Attendance
                    WHERE date IS NOT NULL
                )
                SELECT * 
                FROM LatestConge
                WHERE rn = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // On remplace la valeur @SelectedDate par la date actuelle ou celle sélectionnée
                        cmd.Parameters.AddWithValue("@date", DateNow.Value); // Assurez-vous que DateNow est un contrôle DateTimePicker ou similaire

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AttendanceData ad = new AttendanceData
                            {


                                EmployeeName = reader["employee_name"].ToString(),
                                Date = Convert.ToDateTime(reader["date"]),
                                HeurEntrerAM = reader["heure_entrerAM"] != DBNull.Value ? (TimeSpan)reader["heure_entrerAM"] : TimeSpan.Zero,
                                HeurSortieAM = reader["heure_sortieAM"] != DBNull.Value ? (TimeSpan)reader["heure_sortieAM"] : TimeSpan.Zero,
                                HeurEntrerPM = reader["heure_entrerPM"] != DBNull.Value ? (TimeSpan)reader["heure_entrerPM"] : TimeSpan.Zero,
                                HeurSortiePM = reader["heure_sortiePM"] != DBNull.Value ? (TimeSpan)reader["heure_sortiePM"] : TimeSpan.Zero,
                                Status = reader["status"].ToString(),
                                Session = reader["session"].ToString()
                            };

                            attendanceList.Add(ad);
                        }



                        // Lier la liste au DataGridView
                        dataGridView1.DataSource = attendanceList;


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void displayEmployees2()
        {
            List<AttendanceData> attendanceList = new List<AttendanceData>();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Récupérer les données d'assiduité pour une date spécifique

                    string query = @"
                WITH LatestConge AS (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY employee_name ORDER BY date DESC) AS rn
                    FROM AttendanceHistorique
                    WHERE date IS NOT NULL
                )
                SELECT * 
                FROM LatestConge";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // On remplace la valeur @SelectedDate par la date actuelle ou celle sélectionnée
                        cmd.Parameters.AddWithValue("@date", DateNow.Value); // Assurez-vous que DateNow est un contrôle DateTimePicker ou similaire

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AttendanceData ad = new AttendanceData
                            {
                                EmployeeName = reader["employee_name"].ToString(),
                                Date = Convert.ToDateTime(reader["date"]),
                                HeurEntrerAM = reader["heure_entrerAM"] != DBNull.Value ? (TimeSpan)reader["heure_entrerAM"] : TimeSpan.Zero,
                                HeurSortieAM = reader["heure_sortieAM"] != DBNull.Value ? (TimeSpan)reader["heure_sortieAM"] : TimeSpan.Zero,
                                HeurEntrerPM = reader["heure_entrerPM"] != DBNull.Value ? (TimeSpan)reader["heure_entrerPM"] : TimeSpan.Zero,
                                HeurSortiePM = reader["heure_sortiePM"] != DBNull.Value ? (TimeSpan)reader["heure_sortiePM"] : TimeSpan.Zero,
                                Status = reader["status"].ToString(),
                                Session = reader["session"].ToString()
                            };

                            attendanceList.Add(ad);
                        }


                        // Lier la liste au DataGridView
                        dataGridView2.DataSource = attendanceList;

                        if (attendanceList.Count == 0)
                        {
                            MessageBox.Show("No records found for the selected date.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //============================================= BUTTOM_CLICK CLEAR============================//

        public void clearFields()
        {

            EmployeeName.SelectedIndex = -1;
            Session.SelectedIndex = -1;
            Status.SelectedIndex = -1;
            HeurEntrerAM.Text = "";
            HeurSortieAM.Text = "";
            HeurEntrerPM.Text = "";
            HeurSortiePM.Text = "";
            DateNow.Text = "";

        }

        private void Btn_addPresence_Click(object sender, EventArgs e)
        {
            // Déclaration des variables pour la vérification des champs
            DateTime heureEntrerAM;
            DateTime heureSortieAM;
            DateTime heureEntrerPM;
            DateTime heureSortiePM;

            // Vérification des champs obligatoires et des dates
            if (string.IsNullOrEmpty(EmployeeName.Text) ||
                !DateTime.TryParse(HeurEntrerAM.Text.Trim(), out heureEntrerAM) ||
                !DateTime.TryParse(HeurSortieAM.Text.Trim(), out heureSortieAM) ||
                !DateTime.TryParse(HeurEntrerPM.Text.Trim(), out heureEntrerPM) ||
                !DateTime.TryParse(HeurSortiePM.Text.Trim(), out heureSortiePM) ||
                string.IsNullOrEmpty(Session.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs correctement.", "Erreur de saisie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Connexion à la base de données
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    // Initialisation des variables
                    TimeSpan totalHeurJour = TimeSpan.Zero;
                    TimeSpan totalHeuresSupp = TimeSpan.Zero;
                    TimeSpan dureeNormaleMatin = new TimeSpan(4, 30, 0);  // 4 heures 30 minutes pour le matin
                    TimeSpan dureeNormaleApresMidi = new TimeSpan(3, 30, 0); // 3 heures 30 minutes pour l'après-midi
                    TimeSpan dureeNormaleJour = new TimeSpan(8, 0, 0);    // 8 heures pour une journée complète

                    // Calcul des heures travaillées et heures supplémentaires
                    if (Session.Text.Trim() == "Matin")
                    {
                        totalHeurJour = heureSortieAM - heureEntrerAM;
                        // Si les heures travaillées dépassent 4h30, calcul des heures supplémentaires
                        totalHeuresSupp = totalHeurJour > dureeNormaleMatin ? totalHeurJour - dureeNormaleMatin : TimeSpan.Zero;
                    }
                    else if (Session.Text.Trim() == "Après-midi")
                    {
                        totalHeurJour = heureSortiePM - heureEntrerPM;
                        // Si les heures travaillées dépassent 3h30, calcul des heures supplémentaires
                        totalHeuresSupp = totalHeurJour > dureeNormaleApresMidi ? totalHeurJour - dureeNormaleApresMidi : TimeSpan.Zero;
                    }
                    else if (Session.Text.Trim() == "Journalière")
                    {
                        TimeSpan totalHeurAM = heureSortieAM - heureEntrerAM;
                        TimeSpan totalHeurPM = heureSortiePM - heureEntrerPM;
                        totalHeurJour = totalHeurAM + totalHeurPM;
                        // Si les heures travaillées dépassent 8 heures, calcul des heures supplémentaires
                        totalHeuresSupp = totalHeurJour > dureeNormaleJour ? totalHeurJour - dureeNormaleJour : TimeSpan.Zero;
                    }

                    // Affichage des résultats (par exemple)
                    Console.WriteLine("Total heures travaillées : " + totalHeurJour);
                    Console.WriteLine("Total heures supplémentaires : " + totalHeuresSupp);


                    // Récupérer les heures supplémentaires existantes du mois en cours
                    TimeSpan heuresSupplementairesPrecedentes = GetPreviousOvertime(EmployeeName.SelectedItem.ToString().Trim());

                    // Ajouter les heures supplémentaires calculées aux heures précédentes
                    totalHeuresSupp += heuresSupplementairesPrecedentes;

                    // Mise à jour de la table employees avec le total des heures supplémentaires
                    UpdateEmployeeOvertime(EmployeeName.SelectedItem.ToString().Trim(), totalHeuresSupp);

                    // Insertion dans AttendanceHistorique
                    InsertAttendanceHistorique(EmployeeName.SelectedItem.ToString().Trim(), heureEntrerAM, heureSortieAM, heureEntrerPM, heureSortiePM, totalHeurJour, totalHeuresSupp);

                    // Insertion dans Attendance avec MERGE
                    MergeAttendance(EmployeeName.SelectedItem.ToString().Trim(), heureEntrerAM, heureSortieAM, heureEntrerPM, heureSortiePM, totalHeurJour, totalHeuresSupp);

                    MessageBox.Show("Présence ajoutée avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clearFields();
                    RefreshData();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Erreur lors de l'ajout de la présence : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        // Méthode pour obtenir les heures supplémentaires précédentes
        private TimeSpan GetPreviousOvertime(string employeeName)
        {
            string query = @"
        SELECT 
            SUM(DATEDIFF(SECOND, '00:00:00', heures_supplementaires)) AS TotalHeuresSupplementaires
        FROM 
            AttendanceHistorique
        WHERE 
            employee_name = @employee_name AND MONTH(date) = MONTH(@date) AND YEAR(date) = YEAR(@date)";

            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@employee_name", employeeName);
                cmd.Parameters.AddWithValue("@date", DateNow.Value);

                object result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value
                    ? TimeSpan.FromSeconds(Convert.ToInt32(result))
                    : TimeSpan.Zero; // Aucune donnée trouvée
            }
        }


        // Méthode pour insérer dans AttendanceHistorique
        private void InsertAttendanceHistorique(string employeeName, DateTime heureEntrerAM, DateTime heureSortieAM, DateTime heureEntrerPM, DateTime heureSortiePM, TimeSpan totalHeurJour, TimeSpan totalHeuresSupp)
        {
            string query = @"
        INSERT INTO AttendanceHistorique
        (employee_name, date, heure_entrerAM, heure_sortieAM, heure_entrerPM, heure_sortiePM, total_heurs, heures_supplementaires, session, status, insert_date) 
        VALUES 
        (@employee_name, @date, @heure_entrerAM, @heure_sortieAM, @heure_entrerPM, @heure_sortiePM, @total_heurs, @heures_supplementaires, @session, @status, @insert_date)";

            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@employee_name", employeeName);
                cmd.Parameters.AddWithValue("@date", DateNow.Value);
                cmd.Parameters.AddWithValue("@heure_entrerAM", heureEntrerAM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_sortieAM", heureSortieAM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_entrerPM", heureEntrerPM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_sortiePM", heureSortiePM.TimeOfDay);
                cmd.Parameters.AddWithValue("@total_heurs", totalHeurJour.ToString(@"hh\:mm"));
                cmd.Parameters.AddWithValue("@heures_supplementaires", totalHeuresSupp.ToString(@"hh\:mm"));
                cmd.Parameters.AddWithValue("@session", Session.Text.Trim());
                cmd.Parameters.AddWithValue("@status", Status.Text.Trim());
                cmd.Parameters.AddWithValue("@insert_date", DateTime.Today);

                cmd.ExecuteNonQuery();
            }
        }

        // Méthode pour mettre à jour les heures supplémentaires de l'employé
        private void UpdateEmployeeOvertime(string employeeName, TimeSpan totalHeuresSupp)
        {
            string query = @"
        UPDATE employees 
        SET heures_supplementaires = @heures_supplementaires
        WHERE full_name = @employee_name AND delete_date IS NULL";

            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@heures_supplementaires", totalHeuresSupp.ToString(@"hh\:mm"));
                cmd.Parameters.AddWithValue("@employee_name", employeeName);
                cmd.ExecuteNonQuery();
            }

        }

        // Méthode pour faire le MERGE dans Attendance
        private void MergeAttendance(string employeeName, DateTime heureEntrerAM, DateTime heureSortieAM, DateTime heureEntrerPM, DateTime heureSortiePM, TimeSpan totalHeurJour, TimeSpan totalHeuresSupp)
        {
            string query = @"
        MERGE INTO Attendance AS target
        USING (
            SELECT @employee_name AS employee_name, @date AS date, @heure_entrerAM AS heure_entrerAM, @heure_sortieAM AS heure_sortieAM, @heure_entrerPM AS heure_entrerPM, @heure_sortiePM AS heure_sortiePM,
                   @total_heurs AS total_heurs, @heures_supplementaires AS heures_supplementaires, @session AS session, @status AS status, @insert_date AS insert_date
        ) AS source
        ON target.employee_name = source.employee_name 
        WHEN MATCHED THEN
            UPDATE SET
                target.date = source.date,
                target.heure_entrerAM = source.heure_entrerAM,
                target.heure_sortieAM = source.heure_sortieAM,
                target.heure_entrerPM = source.heure_entrerPM,
                target.heure_sortiePM = source.heure_sortiePM,
                target.total_heurs = source.total_heurs,
                target.heures_supplementaires = source.heures_supplementaires,
                target.session = source.session,
                target.status = source.status,
                target.insert_date = source.insert_date
        WHEN NOT MATCHED THEN
            INSERT (employee_name, date, heure_entrerAM, heure_sortieAM, heure_entrerPM, heure_sortiePM, total_heurs, heures_supplementaires, session, status, insert_date)
            VALUES (source.employee_name, source.date, source.heure_entrerAM, source.heure_sortieAM, source.heure_entrerPM, source.heure_sortiePM, source.total_heurs, source.heures_supplementaires, source.session, source.status, source.insert_date);";

            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@employee_name", employeeName);
                cmd.Parameters.AddWithValue("@date", DateNow.Value);
                cmd.Parameters.AddWithValue("@heure_entrerAM", heureEntrerAM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_sortieAM", heureSortieAM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_entrerPM", heureEntrerPM.TimeOfDay);
                cmd.Parameters.AddWithValue("@heure_sortiePM", heureSortiePM.TimeOfDay);
                cmd.Parameters.AddWithValue("@total_heurs", totalHeurJour.ToString(@"hh\:mm"));
                cmd.Parameters.AddWithValue("@heures_supplementaires", totalHeuresSupp.ToString(@"hh\:mm"));
                cmd.Parameters.AddWithValue("@session", Session.Text.Trim());
                cmd.Parameters.AddWithValue("@status", Status.Text.Trim());
                cmd.Parameters.AddWithValue("@insert_date", DateTime.Today);

                cmd.ExecuteNonQuery();
            }
        }

        private void Modifier_Click(object sender, EventArgs e)
        {
            DateTime heureEntrerAM;
            DateTime heureSortieAM;
            DateTime heureEntrerPM;
            DateTime heureSortiePM;

            // Vérification des champs avant la modification
            if (string.IsNullOrEmpty(EmployeeName.Text) ||
                !DateTime.TryParse(HeurEntrerAM.Text.Trim(), out heureEntrerAM) ||
                !DateTime.TryParse(HeurSortieAM.Text.Trim(), out heureSortieAM) ||
                !DateTime.TryParse(HeurEntrerPM.Text.Trim(), out heureEntrerPM) ||
                !DateTime.TryParse(HeurSortiePM.Text.Trim(), out heureSortiePM) ||
                string.IsNullOrEmpty(Session.Text) ||
                string.IsNullOrEmpty(Status.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs correctement.", "Erreur de saisie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                DialogResult check = MessageBox.Show("Êtes-vous sûr de vouloir METTRE À JOUR " +
                    "NOM: " + EmployeeName.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    if (connect.State == ConnectionState.Closed)
                    {
                        connect.Open();
                    }

                    // Requête de mise à jour
                    string query = @"
                                UPDATE Attendance 
                                SET heure_entrerAM = @heure_entrerAM, 
                                    heure_sortieAM = @heure_sortieAM, 
                                    heure_entrerPM = @heure_entrerPM, 
                                    heure_sortiePM = @heure_sortiePM, 
                                    session = @session, 
                                    status = @status 
                                WHERE employee_name = @employee_name AND date = @date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@employee_name", EmployeeName.Text.Trim());
                        cmd.Parameters.AddWithValue("@date", DateNow.Value);  // Date du jour ou la date de présence
                        cmd.Parameters.AddWithValue("@heure_entrerAM", heureEntrerAM.TimeOfDay);
                        cmd.Parameters.AddWithValue("@heure_sortieAM", heureSortieAM.TimeOfDay);
                        cmd.Parameters.AddWithValue("@heure_entrerPM", heureEntrerPM.TimeOfDay);
                        cmd.Parameters.AddWithValue("@heure_sortiePM", heureSortiePM.TimeOfDay);
                        cmd.Parameters.AddWithValue("@session", Session.Text.Trim());
                        cmd.Parameters.AddWithValue("@status", Status.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {

                            RefreshData(); // Actualisation des données
                            clearFields();
                        }
                        else
                        {
                            MessageBox.Show("Aucune modification n'a été apportée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    connect.Close();
                }
                else
                {
                    MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


        }


        private void Supprimer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EmployeeName.Text) || DateNow.Value == null)
            {
                MessageBox.Show("Veuillez sélectionner un employé et une date.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            else
            {
                DialogResult check = MessageBox.Show("Êtes-vous sûr de vouloir SUPPRIMER " +
                    "NOM: " + EmployeeName.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        if (connect.State == ConnectionState.Closed)
                            connect.Open();

                        // Suppression de l'enregistrement
                        string query = @"
                                    DELETE FROM Attendance
                                    WHERE employee_name = @employee_name AND date = @date";

                        using (SqlCommand cmd = new SqlCommand(query, connect))
                        {
                            cmd.Parameters.AddWithValue("@employee_name", EmployeeName.SelectedItem.ToString().Trim());
                            cmd.Parameters.AddWithValue("@date", DateNow.Value);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                RefreshData();
                                clearFields();
                            }
                            else
                            {
                                MessageBox.Show("Aucune donnée trouvée à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression de la présence : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void restaurer_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                EmployeeName.Text = row.Cells[0].Value.ToString();
                DateNow.Text = row.Cells[1].Value.ToString();
                HeurEntrerAM.Text = row.Cells[2].Value.ToString();
                HeurSortieAM.Text = row.Cells[3].Value.ToString();
                HeurEntrerPM.Text = row.Cells[4].Value.ToString();
                HeurSortiePM.Text = row.Cells[5].Value.ToString();
                Session.Text = row.Cells[6].Value.ToString();
                Status.Text = row.Cells[7].Value.ToString();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                /* DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                 EmployeeName.Text = row.Cells[0].Value.ToString();
                 DateNow.Text = row.Cells[1].Value.ToString();
                 HeurEntrerAM.Text = row.Cells[2].Value.ToString();
                 HeurSortieAM.Text = row.Cells[3].Value.ToString();
                 HeurEntrerPM.Text = row.Cells[4].Value.ToString();
                 HeurSortiePM.Text = row.Cells[5].Value.ToString();
                 Session.Text = row.Cells[6].Value.ToString();
                 Status.Text = row.Cells[7].Value.ToString(); */

            }
        }


        private void BHirsotique_Click(object sender, EventArgs e)
        {
            PHistorique.Visible = true;
            ligneHistorique.Visible = true;
            PPresence.Visible = false;
            lignePresence.Visible = false;
        }

        private void BPresence_Click(object sender, EventArgs e)
        {
            PPresence.Visible = true;
            lignePresence.Visible = true;
            PHistorique.Visible = false;
            ligneHistorique.Visible = false;
        }
        // Filtrage lorsque l'utilisateur clique sur le bouton de recherche (par texte uniquement)
        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim().ToLower();

            // Passez la date sélectionnée à la méthode de filtrage
            FilterEmployeeData(searchTerm, DatePresence.Value.Date);
        }

        // Filtrage automatique lorsqu'une date est sélectionnée
        private void DatePresence_ValueChanged(object sender, EventArgs e)
        {
            // Récupère la date sélectionnée dans le DateTimePicker
            DateTime selectedDate = DatePresence.Value.Date;

            // Utilisez un terme de recherche vide si aucun texte n'est fourni
            string searchTerm = searchTextBox.Text.Trim().ToLower();

            // Passez les deux paramètres à la méthode de filtrage
            FilterEmployeeData(searchTerm, selectedDate);
        }

        // Méthode de filtrage des données, accepte un terme de recherche et une date
        private void FilterEmployeeData(string searchTerm, DateTime selectedDate)
        {
            List<AttendanceData> filteredList = new List<AttendanceData>();

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.DataBoundItem is AttendanceData employee)
                {
                    // Vérifie si le nom d'employé contient le terme de recherche
                    bool matchesSearchTerm = string.IsNullOrEmpty(searchTerm) || employee.EmployeeName.ToLower().Contains(searchTerm);
                    // Vérifie si la date de l'employé correspond à la date sélectionnée
                    bool matchesDate = employee.Date.Date == selectedDate;

                    // Filtrer uniquement si les deux critères sont satisfaits
                    if (matchesSearchTerm && matchesDate)
                    {
                        filteredList.Add(employee);
                    }
                }
            }

            // Met à jour le DataGridView avec les données filtrées
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = filteredList;
            dataGridView2.Refresh();
        }

        // Méthode pour réinitialiser le filtre
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            DatePresence.Value = DateTime.Now; // Réinitialise le DateTimePicker à la date actuelle

            // Optionnel : si vous avez un ComboBox pour les critères de recherche, le réinitialiser
            // searchCriteriaComboBox.SelectedIndex = -1;

            RefreshData(); // Méthode pour rafraîchir les données dans le DataGridView
        }



    }
}
