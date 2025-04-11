using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    public class SalaryData
    {
        public string EmployeeID { get; set; }
        public string Nom { get; set; }
        public string Departement { get; set; }
        public string Poste { get; set; }
        public string Numero_compte { get; set; }
        public string Agence { get; set; }
        public String Salaire_Base { get; set; }
        public String Heurs_Sup { get; set; }
        public String MontantSup { get; set; }
        public String Primes { get; set; }
        public String Salaire { get; set; }

        private SqlConnection connect;

        // Constructor to initialize the SQL connection
        public SalaryData()
        {
            connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");
        }

        public List<SalaryData> salaryEmployeeListData()
        {
            List<SalaryData> listdata = new List<SalaryData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM employees WHERE status = 'Active' AND delete_date IS NULL ORDER BY employee_id";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            SalaryData sd = new SalaryData();
                            sd.EmployeeID = reader["employee_id"].ToString();
                            sd.Nom = reader["full_name"].ToString();
                            sd.Departement = reader["departement"].ToString();
                            sd.Poste = reader["position"].ToString();
                            sd.Agence = reader["agency"].ToString();
                            sd.Numero_compte = reader["number_count"].ToString();
                            sd.Salaire = reader["gross_salary"].ToString();
                            sd.Heurs_Sup = reader["heures_supplementaires"].ToString();
                            sd.Salaire_Base = reader["salary_base"].ToString();

                            // Formater le reliquat avec " Ar" en limitant à deux décimales
                            decimal overtimeRate = reader["overtime_rate"] != DBNull.Value ? Convert.ToDecimal(reader["overtime_rate"]) : 0;
                            sd.MontantSup = $"{overtimeRate:0.00} Ar"; // Limite à deux décimales

                            // Formater le reliquat avec " Ar" en limitant à deux décimales
                            decimal bonuses = reader["bonuses"] != DBNull.Value ? Convert.ToDecimal(reader["bonuses"]) : 0;
                            sd.Primes = $"{bonuses:0.00} Ar"; // Limite à deux décimales





                            listdata.Add(sd);
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
