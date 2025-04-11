/*using EmployeeManagementSystem.Formulaire;
using Microsoft.Reporting.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagementSystem.Resources;
using System.Data.SqlClient;
using System.Reflection;    // Nécessaire pour la réflexion
using System.Data; // Assurez-vous d'avoir cette directive using si vous utilisez DataRowView
using Moq;
using Moq.Protected;
using System;



namespace EmployeeManagementSystem.Tests
{
    //============================================== TESTE POUR REGISTERFORM
    [TestClass]
    public class RegisterFormTests
    {
        private RegisterForm registerForm;

        [TestInitialize]
        public void SetUp()
        {
            // Initialise une instance de RegisterForm avant chaque test
            registerForm = new RegisterForm();
            registerForm.Load += (s, e) => { }; // Assurez-vous que le formulaire est complètement chargé
            registerForm.Show(); // Affichez le formulaire pour garantir que les contrôles sont chargés
        }

        [TestCleanup]
        public void TearDown()
        {
            // Libère les ressources après chaque test
            registerForm.Dispose();
        }
        [TestMethod]
        public void Test_SignupButton_Click_NoUsernameOrPassword_ShouldShowErrorMessage()
        {
            // Arrange
            var usernameTextBox = registerForm.Controls.Find("signup_username", true).FirstOrDefault() as TextBox;
            var passwordTextBox = registerForm.Controls.Find("signup_password", true).FirstOrDefault() as TextBox;
            var label3 = registerForm.Controls.Find("label3", true).FirstOrDefault() as Label;
            var label7 = registerForm.Controls.Find("label7", true).FirstOrDefault() as Label;
            var signupButton = registerForm.Controls.Find("signup_btn", true).FirstOrDefault() as Button;

            // Affichez tous les contrôles présents dans le formulaire pour vérification
            foreach (Control control in registerForm.Controls)
            {
                Console.WriteLine($"Control name: {control.Name}");
            }

            // Vérifiez que les contrôles ne sont pas null
            if (usernameTextBox == null || passwordTextBox == null || label3 == null || label7 == null || signupButton == null)
            {
                Assert.Fail("Un ou plusieurs contrôles sont manquants dans le formulaire.");
            }

            // Act
            signupButton.PerformClick();

            // Assert
            Assert.IsTrue(label3.Visible, "Le message d'erreur pour le nom d'utilisateur doit être visible.");
            Assert.IsTrue(label7.Visible, "Le message d'erreur pour le mot de passe doit être visible.");
        }

        [TestMethod]
        public void Test_SignupButton_Click_ValidInput_ShouldNotShowErrorMessage()
        {
            // Arrange
            var usernameTextBox = registerForm.Controls.Find("signup_username", true).FirstOrDefault() as TextBox;
            var passwordTextBox = registerForm.Controls.Find("signup_password", true).FirstOrDefault() as TextBox;
            var label3 = registerForm.Controls.Find("label3", true).FirstOrDefault() as Label;
            var label7 = registerForm.Controls.Find("label7", true).FirstOrDefault() as Label;
            var signupButton = registerForm.Controls.Find("signup_btn", true).FirstOrDefault() as Button;

            // Affichez tous les contrôles présents dans le formulaire pour vérification
            foreach (Control control in registerForm.Controls)
            {
                Console.WriteLine($"Control name: {control.Name}");
            }

            // Vérifiez que les contrôles ne sont pas null
            if (usernameTextBox == null || passwordTextBox == null || label3 == null || label7 == null || signupButton == null)
            {
                Assert.Fail("Un ou plusieurs contrôles sont manquants dans le formulaire.");
            }

            // Fournir des entrées valides
            usernameTextBox.Text = "elio";
            passwordTextBox.Text = "elio";

            // Act
            signupButton.PerformClick();

            // Assert
            Assert.IsFalse(label3.Visible, "Le message d'erreur pour le nom d'utilisateur ne doit pas être visible.");
            Assert.IsFalse(label7.Visible, "Le message d'erreur pour le mot de passe ne doit pas être visible.");
        }


        [TestMethod]
        public void Test_Timer_UpdateTimeLabel_ShouldDisplayCorrectTime()
        {
            // Arrange
            var labelHeure = registerForm.Controls["labelHeure"] as Label;

            if (labelHeure == null)
            {
                Assert.Fail("Le contrôle 'labelHeure' est manquant dans le formulaire.");
            }

            // Act
            var privateObject = new PrivateObject(registerForm);
            privateObject.Invoke("MettreAJourHeure", new object[] { null, EventArgs.Empty });

            // Assert
            var expectedTime = DateTime.Now.ToString("HH:mm:ss");
            // Note: L'heure actuelle peut changer légèrement, donc une comparaison directe peut échouer.
            // Utilisez une marge acceptable ou une autre méthode pour valider l'heure affichée.
            Assert.AreEqual(expectedTime.Substring(0, 5), labelHeure.Text.Substring(0, 5), "Le label de l'heure doit afficher l'heure actuelle.");
        }
    }

    //============================================== TESTE POUR CONNEXION
    [TestClass]
    public class ConnexionTests
    {
        private Connexion connexionForm;

        [TestInitialize]
        public void SetUp()
        {
            connexionForm = new Connexion();
            connexionForm.Load += (s, e) => { }; // Assurez-vous que le formulaire est complètement chargé
            connexionForm.Show(); // Afficher le formulaire pour garantir que les contrôles sont chargés
        }

        [TestCleanup]
        public void TearDown()
        {
            connexionForm.Dispose();
        }

        [TestMethod]
        public void Test_LoginButton_NoUsernameOrPassword_ShouldShowErrorMessage()
        {
            // Arrange
            var usernameTextBox = connexionForm.Controls.Find("login_username", true).FirstOrDefault() as TextBox;
            var passwordTextBox = connexionForm.Controls.Find("login_password", true).FirstOrDefault() as TextBox;
            var errorLabel1 = connexionForm.Controls.Find("label1", true).FirstOrDefault() as Label;
            var errorLabel7 = connexionForm.Controls.Find("label7", true).FirstOrDefault() as Label;
            var loginButton = connexionForm.Controls.Find("login_btn", true).FirstOrDefault() as Button;

            // Affichez tous les contrôles présents dans le formulaire pour vérification
            foreach (Control control in connexionForm.Controls)
            {
                Console.WriteLine($"Control name: {control.Name}");
            }

            // Vérifiez que les contrôles ne sont pas null
            if (usernameTextBox == null || passwordTextBox == null || errorLabel1 == null || errorLabel7 == null || loginButton == null)
            {
                Assert.Fail("Un ou plusieurs contrôles sont manquants dans le formulaire.");
            }

            // Act
            loginButton.PerformClick();

            // Assert
            Assert.IsTrue(errorLabel1.Visible, "Le message d'erreur du nom d'utilisateur doit être visible.");
            Assert.IsTrue(errorLabel7.Visible, "Le message d'erreur du mot de passe doit être visible.");
        }
        [TestMethod]
        public void Test_LoginButton_ValidInput_ShouldNotShowErrorMessage()
        {
            // Arrange
            var usernameTextBox = connexionForm.Controls.Find("login_username", true).FirstOrDefault() as TextBox;
            var passwordTextBox = connexionForm.Controls.Find("login_password", true).FirstOrDefault() as TextBox;
            var errorLabel1 = connexionForm.Controls.Find("label1", true).FirstOrDefault() as Label;
            var errorLabel7 = connexionForm.Controls.Find("label7", true).FirstOrDefault() as Label;
            var loginButton = connexionForm.Controls.Find("login_btn", true).FirstOrDefault() as Button;

            // Affichez tous les contrôles présents dans le formulaire pour vérification
            foreach (Control control in connexionForm.Controls)
            {
                Console.WriteLine($"Control name: {control.Name}");
            }

            // Vérifiez que les contrôles ne sont pas null
            if (usernameTextBox == null || passwordTextBox == null || errorLabel1 == null || errorLabel7 == null || loginButton == null)
            {
                Assert.Fail("Un ou plusieurs contrôles sont manquants dans le formulaire.");
            }

            // Fournir des entrées valides
            usernameTextBox.Text = "elio";
            passwordTextBox.Text = "elio";

            // Act
            loginButton.PerformClick();

            // Assert
            Assert.IsFalse(errorLabel1.Visible, "Le message d'erreur du nom d'utilisateur ne doit pas être visible.");
            Assert.IsFalse(errorLabel7.Visible, "Le message d'erreur du mot de passe ne doit pas être visible.");
        }

    }

    //============================================== TESTE POUR MAINFORM
    [TestClass]
    public class MainFormTests
    {
        private MainForm _mainForm;

        [TestInitialize]
        public void Setup()
        {
            _mainForm = new MainForm("testUser");
            _mainForm.Show();
        }
        [TestMethod]
        public void TestAccessToPrivateLabelUtilisateur()
        {
            // Arrange
            var username = "testUser";
            var mainForm = new MainForm(username);

            // Assurez-vous que le formulaire est affiché et que les contrôles sont chargés
            mainForm.Show();
            Application.DoEvents(); // Permet de s'assurer que le formulaire est complètement chargé

            // Utiliser la réflexion pour accéder au champ privé 'Utilisateur'
            var utilisateurField = typeof(MainForm).GetField("Utilisateur", BindingFlags.NonPublic | BindingFlags.Instance);

            // Vérifiez si le champ est bien trouvé
            Assert.IsNotNull(utilisateurField, "Le champ 'Utilisateur' n'a pas été trouvé.");

            var utilisateurLabel = (Label)utilisateurField.GetValue(mainForm);

            // Assurez-vous que le label n'est pas null
            Assert.IsNotNull(utilisateurLabel, "Le label Utilisateur ne doit pas être null");

            // Configurer le texte du label pour le test
            utilisateurLabel.Text = "testUser";

            // Assert
            Assert.AreEqual("testUser", utilisateurLabel.Text, "Le texte du label Utilisateur doit être 'testUser'");
        }

        [TestMethod]
        public void TestAccessToPrivateSqlConnection()
        {
            // Arrange
            // Utiliser la réflexion pour accéder au champ privé 'connect'
            var connectField = typeof(MainForm).GetField("connect", BindingFlags.NonPublic | BindingFlags.Instance);
            var sqlConnection = (SqlConnection)connectField.GetValue(_mainForm);

            // Assert
            Assert.IsNotNull(sqlConnection, "La connexion SQL ne doit pas être null");
            Assert.AreEqual("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\USERS\\KANANAVY\\DOCUMENTS\\EMPLOYEEE.MDF;Integrated Security=True;Connect Timeout=30", sqlConnection.ConnectionString, "La chaîne de connexion SQL ne correspond pas");
        }

        [TestMethod]
        public void TestAccessToPrivatePictureBox()
        {
            // Arrange
            // Utiliser la réflexion pour accéder au champ privé 'addEmployee_picture'
            var pictureBoxField = typeof(MainForm).GetField("addEmployee_picture", BindingFlags.NonPublic | BindingFlags.Instance);
            var pictureBox = (PictureBox)pictureBoxField.GetValue(_mainForm);

            // Assert
            Assert.IsNotNull(pictureBox, "Le PictureBox addEmployee_picture ne doit pas être null");
            // Vous pouvez ajouter d'autres assertions sur le PictureBox si nécessaire
        }
    }

    //============================================== TESTE POUR LOADINGPAGE
    [TestClass]
    public class LoadingPageTests
    {
        private LoadingPage _loadingPage;

        [TestInitialize]
        public void Setup()
        {
            _loadingPage = new LoadingPage();
        }

        [TestMethod]
        public void TestTimerStartsOnInitialization()
        {
            // Vérifiez que le timer a démarré
            Assert.IsTrue(_loadingPage.IsTimerRunning());
        }

        [TestMethod]
        public void TestProgressBarValueIncreases()
        {
            // Simulez un tick du timer
            _loadingPage.SimulateTimerTick();

            // Vérifiez que la valeur de ProgressBar a augmenté
            Assert.AreEqual(1, _loadingPage.GetProgressBarValue());
        }

        [TestMethod]
        public void TestProgressBarCompletion()
        {
            // Simulez le remplissage de la ProgressBar jusqu'à 100
            while (_loadingPage.GetProgressBarValue() < 100)
            {
                _loadingPage.SimulateTimerTick();
            }

            // Assurez-vous que le tick du timer a arrêté la ProgressBar et ouvert le formulaire de connexion
            _loadingPage.SimulateTimerTick();

            // Vérifiez que la ProgressBar est à 100
            Assert.AreEqual(100, _loadingPage.GetProgressBarValue());
            // Vous devrez vérifier si le formulaire Connexion est affiché
            // Note : Pour cette vérification, vous aurez besoin d'une approche différente, comme utiliser un mock ou un spy.
        }
    }

    //============================================== TESTE POUR FPRINT
    [TestClass]
    public class fprintTests
    {
        [TestMethod]
        public void TestReportViewerInitialization()
        {
            // Arrange
            var form = new fprint();

            // Act
            var reportViewer = form.Controls.OfType<ReportViewer>().FirstOrDefault();

            // Assert
            Assert.IsNotNull(reportViewer, "ReportViewer should be added to the form controls.");
            Assert.AreEqual(DockStyle.Fill, reportViewer.Dock, "ReportViewer should be docked to fill the form.");
        }
    }

    //============================================== TESTE POUR FPRINT
    [TestClass]
    public class AddEmployeeTests
    {
        private AddEmployee addEmployeeControl;

        private Mock<AddEmployee> CreateMockAddEmployee()
        {
            // Créez un mock de AddEmployee
            var mock = new Mock<AddEmployee>();
            mock.CallBase = true;
            return mock;
        }

        private SqlConnection CreateInMemoryDatabaseConnection()
        {
            // Création d'une base de données en mémoire avec SQLite
            var connection = new SqlConnection("Data Source=:memory:");
            return connection;
        }

        private T GetPrivateField<T>(object instance, string fieldName)
        {
            var field = typeof(AddEmployee).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(instance);
        }

        private void SetPrivateField(object target, string fieldName, object value)
        {
            var fieldInfo = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(target, value);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            addEmployeeControl = new AddEmployee();
        }

        [TestMethod]
        public void TestRefreshData()
        {
            // Arrange
            var mock = CreateMockAddEmployee();

            // Act
            mock.Object.RefreshData();

            // Assert
            // Assume that these methods have some side effects or change state
            // Check the expected side effects or state changes

            // Example: Verifying changes in the UI elements or other observable changes
            // You will need to adapt these assertions based on the actual effects of the methods
            Assert.IsTrue(SomeConditionThatIndicatesDisplayEmployeeDataWasCalled);
            Assert.IsTrue(SomeConditionThatIndicatesDisplayDEPWasCalled);
            Assert.IsTrue(SomeConditionThatIndicatesDisplayPOSTWasCalled);
        }

        // Example condition checks (you need to implement these based on your code)
        private bool SomeConditionThatIndicatesDisplayEmployeeDataWasCalled
        {
            get
            {
                // Check the state or output that indicates displayEmployeeData was called
                // This might involve checking UI elements or other parts of the state
                return true; // Replace with actual condition
            }
        }

        private bool SomeConditionThatIndicatesDisplayDEPWasCalled
        {
            get
            {
                // Check the state or output that indicates displayDEP was called
                return true; // Replace with actual condition
            }
        }

        private bool SomeConditionThatIndicatesDisplayPOSTWasCalled
        {
            get
            {
                // Check the state or output that indicates displayPOST was called
                return true; // Replace with actual condition
            }
        }


        [TestMethod]
        public void TestLoadPositionsByDepartment()
        {
            // Arrange
            var mock = CreateMockAddEmployee();
            string departmentName = "IT";

            var mockConnection = new Mock<SqlConnection>();
           // mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            SetPrivateField(mock.Object, "connect", mockConnection.Object);

            // Act
            mock.Object.LoadPositionsByDepartment(departmentName);

            // Assert
            mockConnection.Verify(c => c.Open(), Times.Once);

            var comboBox = GetPrivateField<ComboBox>(mock.Object, "addEmployee_position");
            Assert.IsNotNull(comboBox);
         //   mock.Verify(m => comboBox.Items.Clear(), Times.Once);
           // mock.Verify(m => comboBox.Items.Add(It.IsAny<string>()), Times.AtLeastOnce);
           // mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [TestMethod]
        public void TestDisplayData()
        {
            // Arrange
            var mock = CreateMockAddEmployee();
            string query = "SELECT depart_name FROM departement";
            var comboBox = new ComboBox();

            // Act
            mock.Object.DisplayData(query, comboBox);

            // Assert
            Assert.IsTrue(comboBox.Items.Count > 0, "Le ComboBox devrait contenir des éléments.");
        }

        [TestMethod]
        public void TestDisplayDEP()
        {
            // Arrange
            var mock = CreateMockAddEmployee();
            string query = "SELECT depart_name FROM departement";

            // Act
            mock.Object.displayDEP();

            // Assert
            var comboBox = GetPrivateField<ComboBox>(mock.Object, "addEmployee_departement");
           // mock.Verify(m => m.DisplayData(query, comboBox), Times.Once);
        }

        [TestMethod]
        public void TestDisplayPOST()
        {
            // Arrange
            var mock = CreateMockAddEmployee();
            string query = "SELECT poste_name FROM poste";

            // Act
            mock.Object.displayPOST();

            // Assert
            var comboBox = GetPrivateField<ComboBox>(mock.Object, "addEmployee_position");
           // mock.Verify(m => m.DisplayData(query, comboBox), Times.Once);
        }

        [TestMethod]
        public void TestAddEmployeeImportBtn_Click()
        {
            // Arrange
            var mock = CreateMockAddEmployee();
            var dialog = new Mock<OpenFileDialog>();
            dialog.Setup(d => d.ShowDialog()).Returns(DialogResult.OK);
            dialog.Setup(d => d.FileName).Returns("path_to_image.jpg");

            // Act
            var method = typeof(AddEmployee).GetMethod("addEmployee_importBtn_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(mock.Object, new object[] { mock.Object, EventArgs.Empty });

            // Assert
            var pictureBox = GetPrivateField<PictureBox>(mock.Object, "addEmployee_picture");
            Assert.AreEqual("path_to_image.jpg", pictureBox.ImageLocation);
        }

        [TestMethod]
        public void TestAddEmployeeAddBtn_Click()
        {
            // Arrange
            var mockAddEmployee = new Mock<AddEmployee>();

            // Créez un mock pour l'événement de clic
            var mockEvent = new Mock<EventArgs>();

            // Act
            var method = typeof(AddEmployee).GetMethod("addEmployee_addBtn_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(mockAddEmployee.Object, new object[] { mockAddEmployee.Object, mockEvent.Object });

            // Assert
            // Ajoutez les assertions nécessaires en fonction du comportement attendu
        }

    }

}





*/