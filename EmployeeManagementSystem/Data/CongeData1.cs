using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EmployeeManagementSystem
{
    internal class CongeData1
    {
        //public int ID { set; get; } // 0
        public string EmployeeID { set; get; } // 1
        public string Nom { set; get; } // 2
        public string Adresse { set; get; } // 7
        public string Departement { set; get; } // 3
        public string Poste { set; get; } // 4
        public string nature { set; get; } // 5
        public string Motif { set; get; } // 6
        public DateTime Periode_debut { set; get; }
        public DateTime Periode_fin { set; get; }
        public string jour_conge { set; get; } // 10
        public string reliquat { set; get; } // 10
        public int Nun_conge { set; get; }






        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        public List<CongeData1> congeEmployeeListData(string name)
        {
            List<CongeData1> listdata = new List<CongeData1>();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                try
                {
                    connect.Open();

                    string query = @"
                WITH LatestConge AS (
                    SELECT
                        employee_id,
                        MAX(num_conge) AS latest_num_conge
                    FROM conge
                    WHERE delete_date IS NULL
                    GROUP BY employee_id
                )
                SELECT
                    c.employee_id,
                    c.full_name,
                    c.departement,
                    c.position,
                    c.nature_conge,
                    c.motif,
                    c.adresse,
                    c.jour_conge,
                    c.reliquat,
                    c.periode_debut,
                    c.periode_fin,
                    c.num_conge
                FROM conge c
                INNER JOIN LatestConge lc ON c.employee_id = lc.employee_id AND c.num_conge = lc.latest_num_conge
                WHERE c.delete_date IS NULL
            ";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
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

                            // Formater le reliquat avec " jour(s)"
                            double jour_conge = reader["jour_conge"] != DBNull.Value ? Convert.ToInt32(reader["jour_conge"]) : 0;
                            cd.jour_conge = $"{jour_conge} jour(s)";

                            // Log pour vérifier les doublons
                            Debug.WriteLine($"Adding: {cd.EmployeeID} - {cd.Nun_conge}");

                            listdata.Add(cd);
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


    }
}
