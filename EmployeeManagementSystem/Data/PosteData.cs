using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    internal class PosteData
    {
        public string Liste_departement { set; get; } // 2
        public string Nom_Poste { set; get; } // 1
        public string Tache_Poste { set; get; } // 2


        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");

        public List<PosteData> PosteListData(string name, string department = null)
        {
            List<PosteData> listdata = new List<PosteData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM poste WHERE delete_date IS NULL ";

                    if (!string.IsNullOrEmpty(department))
                    {
                        selectData += " AND liste_dep = @department ORDER BY poste_name";
                    }

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        if (!string.IsNullOrEmpty(department))
                        {
                            cmd.Parameters.AddWithValue("@department", department);
                        }

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            PosteData ed = new PosteData();
                            ed.Nom_Poste = reader["poste_name"].ToString();
                            ed.Tache_Poste = reader["poste_tache"].ToString();
                            ed.Liste_departement = reader["liste_dep"].ToString();
                            listdata.Add(ed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
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
