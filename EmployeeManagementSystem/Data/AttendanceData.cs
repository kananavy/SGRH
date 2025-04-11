using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EmployeeManagementSystem
{
    internal class AttendanceData
    {
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan HeurEntrerAM { get; set; }
        public TimeSpan HeurSortieAM { get; set; }
        public TimeSpan HeurEntrerPM { get; set; }
        public TimeSpan HeurSortiePM { get; set; }
        public string Session { get; set; }
        public string Status { get; set; }

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30");

        public List<AttendanceData> GetAttendanceRecords(DateTime selectedDate)
        {
            List<AttendanceData> listdata = new List<AttendanceData>();
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\KANANAVY\DOCUMENTS\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                try
                {
                    connect.Open();
                    string query = @"
                WITH LatestHistorique AS (
                    SELECT
                        employee_name,
                        MAX(date) AS latest_date_presence
                    FROM Attendance
                    WHERE insert_date IS NOT NULL
                    GROUP BY employee_name
                )
                SELECT
                    a.employee_name,
                    a.date,
                    a.heure_entrerAM,
                    a.heure_sortieAM,
                    a.heure_entrerPM,
                    a.heure_sortiePM,
                    a.status,
                    a.session,
                FROM Attendance a
                INNER JOIN LatestHistorique lh ON a.employee_name = lh.employee_name AND a.date = lh.LatestHistorique
                WHERE a.insert_date IS NOT NULL ";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
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

                            listdata.Add(ad);

                            // Log pour vérifier les doublons
                            Debug.WriteLine($"Adding: {ad.EmployeeName} - {ad.Date}");

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
                return listdata; // Renvoie la liste des enregistrements de présence
            }
        }
    }
}
