﻿
//using EmployeeManagementSystem.Formulaire;
using System;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Connexion());
            Application.Run(new LoadingPage());
            //Application.Run(new AttendanceForm());
        }
    }
}
/*

 * 
 */
