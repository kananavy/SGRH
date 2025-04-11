using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class Poste : UserControl

    {
        private string currentDepartment = null;

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        public Poste()
        {
            InitializeComponent();
            displayDEP();
            displayEmployeeData();
            addEmployee_departement.SelectedIndexChanged += new EventHandler(addEmployee_departement_SelectedIndexChanged);

            // Sélectionner "Tous" par défaut
            addEmployee_departement.SelectedIndex = 0;
        }

        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }
            displayEmployeeData();
            displayDEP();

        }
        public void displayEmployeeData()
        {
            PosteData ed = new PosteData();
            List<PosteData> listData;

            if (string.IsNullOrEmpty(currentDepartment) || currentDepartment == "Tous")
            {
                // Si aucun département n'est sélectionné ou "Tous" est sélectionné, afficher tous les postes
                listData = ed.PosteListData("insert_date");
                clearFields();
            }
            else
            {
                // Filtrer par le département actuellement sélectionné
                listData = ed.PosteListData("insert_date", currentDepartment);
            }

            dataGridView1.DataSource = listData;

            // Masquer la colonne 'Liste_departement'
            if (dataGridView1.Columns["Liste_departement"] != null)
            {
                dataGridView1.Columns["Liste_departement"].Visible = false;
            }
        }


        private void addEmployee_departement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (addEmployee_departement.SelectedItem.ToString() == "Tous")
            {
                currentDepartment = null; // Afficher tous les postes
            }
            else
            {
                currentDepartment = addEmployee_departement.SelectedItem.ToString();
            }
            displayEmployeeData();
        }

        // DISPLAY DEPARTEMENT
        public void displayDEP()
        {
            string query = "SELECT depart_name FROM departement";
            DisplayData(query, addEmployee_departement);

            // Ajouter l'option "Tous" au début de la liste
            if (!addEmployee_departement.Items.Contains("Tous"))
            {
                addEmployee_departement.Items.Insert(0, "Tous");
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


        // BUTTOM ADD
        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            if (AddPoste_name.Text == ""
               || AddPoste_tache.Text == ""
               || addEmployee_departement.Text == "")
            {
                Faux.Visible = true;
                Vrais.Visible = false;
            }

            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectPosteName = "SELECT COUNT(id) FROM poste WHERE poste_name = @poste_name";


                        using (SqlCommand selectPoste = new SqlCommand(selectPosteName, connect))
                        {
                            selectPoste.Parameters.AddWithValue("@poste_name", AddPoste_name.Text.Trim());
                            int count = (int)selectPoste.ExecuteScalar();

                            if (count >= 1)
                            {
                                label7.Visible = true;

                            }
                            else
                            {
                                DateTime today = DateTime.Today;

                                string insertData = "INSERT INTO poste " +
                                    "(poste_name, poste_tache, liste_dep, insert_date) " +
                                "VALUES(@poste_name, @poste_tache, @liste_dep, @insert_date)";

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@poste_name", AddPoste_name.Text.Trim());
                                    cmd.Parameters.AddWithValue("@poste_tache", AddPoste_tache.Text.Trim());
                                    cmd.Parameters.AddWithValue("@liste_dep", addEmployee_departement.Text.Trim());
                                    cmd.Parameters.AddWithValue("@insert_date", today);

                                    cmd.ExecuteNonQuery();
                                    // Après l'ajout, réappliquer le filtre
                                    displayEmployeeData();
                                    Vrais.Visible = true;
                                    Faux.Visible = false;
                                    clearFields();


                                }
                            }
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
            }
        }

        // BUTTOM UPDATE
        private void addEmployee_updateBtn_Click_1(object sender, EventArgs e)
        {
            if (AddPoste_name.Text == ""
             || AddPoste_tache.Text == ""
             || addEmployee_departement.Text == "")
            {
                Faux.Visible = true;
                Vrais.Visible = false;
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to UPDATE " +
                    "Poste Name: " + AddPoste_name.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();
                        DateTime today = DateTime.Today;

                        string updateData = "UPDATE poste SET poste_name = @poste_name, poste_tache = @poste_tache" +
                            ", liste_dep = @liste_dep, update_date = @update_date " +
                            "WHERE poste_name = @poste_name";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@poste_name", AddPoste_name.Text.Trim());
                            cmd.Parameters.AddWithValue("@poste_tache", AddPoste_tache.Text.Trim());
                            cmd.Parameters.AddWithValue("@liste_dep", addEmployee_departement.Text.Trim());
                            cmd.Parameters.AddWithValue("@update_date", today);

                            cmd.ExecuteNonQuery();

                            // Après l'ajout, réappliquer le filtre
                            displayEmployeeData();

                            clearFields();
                            Vrais.Visible = true;


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
        private void addEmployee_deleteBtn_Click_1(object sender, EventArgs e)
        {
            if (AddPoste_name.Text == ""
              || AddPoste_tache.Text == ""
              || addEmployee_departement.Text == "")


            {
                Faux.Visible = true;
                Vrais.Visible = false;
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to DELETE " +
                    "Poste Name: " + AddPoste_name.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();
                        DateTime today = DateTime.Today;

                        string updateData = "DELETE FROM poste WHERE poste_name = @poste_name";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@delete_date", today);
                            cmd.Parameters.AddWithValue("@poste_name", AddPoste_name.Text.Trim());

                            cmd.ExecuteNonQuery();

                            displayEmployeeData();
                            Vrais.Visible = true;
                            Faux.Visible = false;
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
        // Dans la méthode de réinitialisation
        private void clearFieldss_Click_1(object sender, EventArgs e)
        {
            clearFields();
            addEmployee_departement.SelectedItem = "Tous";
        }

        public void clearFields()
        {
            AddPoste_name.Text = "";
            AddPoste_tache.Text = "";
            // Ne pas réinitialiser le ComboBox ici
        }



        // DISPLAY DATA GRID VIEW
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Afficher les données dans les champs appropriés
                AddPoste_name.Text = row.Cells["Nom_Poste"].Value.ToString();
                AddPoste_tache.Text = row.Cells["Tache_Poste"].Value.ToString();
                addEmployee_departement.Text = row.Cells["Liste_departement"].Value.ToString();
            }
        }


        // SETTING
        private void AddPoste_name_Click(object sender, EventArgs e)
        {
            Faux.Visible = false;
            label7.Visible = false;
            Vrais.Visible = false;

        }
    }
}
