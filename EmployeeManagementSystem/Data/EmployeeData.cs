using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    public class EmployeeData
    {
        public string EmployeeID { set; get; }
        public string Nom { set; get; }
        public string Genre { set; get; }
        public string Contact { set; get; }
        public string Adresse { set; get; }
        public string CIN { set; get; }
        public DateTime Date_Naissance { set; get; }
        public string Lieu_Naissance { set; get; }
        public string Nationalité { set; get; }
        public string Diplôme { set; get; }
        public string Departement { set; get; }
        public string Poste { set; get; }
        public string Grade { set; get; }
        public string Numero_compte { set; get; }
        public string Agence { set; get; }
        public string Image { set; get; }
        public DateTime Date_de_recrutement { set; get; }
        public string Reliquat { get; set; }
        public string Salary { get; set; }
        public string Status { set; get; }

        private SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");

        public List<EmployeeData> employeeListData(string Name)
        {
            List<EmployeeData> listdata = new List<EmployeeData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM employees WHERE delete_date IS NULL ORDER BY employee_id";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeeData ed = new EmployeeData
                            {
                                EmployeeID = reader["employee_id"].ToString(),
                                Nom = reader["full_name"].ToString(),
                                Genre = reader["gender"].ToString(),
                                Adresse = reader["adresse"].ToString(),
                                Contact = reader["contact_number"].ToString(),
                                CIN = reader["cin"].ToString(),
                                Date_Naissance = (DateTime)reader["birth_day"],
                                Lieu_Naissance = reader["birth_place"].ToString(),
                                Nationalité = reader["nationality"].ToString(),
                                Diplôme = reader["diploma"].ToString(),
                                Departement = reader["departement"].ToString(),
                                Poste = reader["position"].ToString(),
                                Grade = reader["grade"].ToString(),
                                Numero_compte = reader["number_count"].ToString(),
                                Agence = reader["agency"].ToString(),
                                Image = reader["image"].ToString(),
                                Date_de_recrutement = (DateTime)reader["date_recrute"],

                                Salary = reader["gross_salary"].ToString(),
                                Status = reader["status"].ToString()
                            };

                            // Formater le reliquat avec " jour(s)"
                            int Reliquat = reader["Reliquat"] != DBNull.Value ? Convert.ToInt32(reader["Reliquat"]) : 0;
                            ed.Reliquat = $"{Reliquat} jour(s)";

                            listdata.Add(ed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listdata;
        }

        public void UpdateReliquatAnnuel()
        {
            DateTime today = DateTime.Today;
            int currentYear = today.Year;

            // Ouvrir la connexion
            if (connect.State == ConnectionState.Closed)
            {
                connect.Open();
            }

            try
            {
                // Requête pour mettre à jour les employés dont le reliquat n'a pas encore été mis à jour pour l'année en cours
                string updateReliquatQuery = "UPDATE employees SET reliquat = reliquat + 30, last_updated_year = @currentYear " +
                                             "WHERE last_updated_year < @currentYear OR last_updated_year IS NULL";

                using (SqlCommand cmd = new SqlCommand(updateReliquatQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@currentYear", currentYear);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }
        }
    }
}
