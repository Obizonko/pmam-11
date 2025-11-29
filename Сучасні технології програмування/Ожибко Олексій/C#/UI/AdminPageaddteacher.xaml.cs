using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLL;
using DB_Connection;
namespace UI
{
    /// <summary>
    /// Interaction logic for AdminPageaddteacher.xaml
    /// </summary>
    public partial class AdminPageaddteacher : Window
    {
        private readonly TeacherService _teacherService;

        public AdminPageaddteacher()
        {
            InitializeComponent();
            _teacherService = new TeacherService(new JournalDbContext());
            LoadSubjects();
        }

        private void LoadSubjects()
        {
            var subjects = _teacherService.GetAllSubjects();
            subjectComboBox.ItemsSource = subjects.ToList();
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = textBox1.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var firstName = fullName.Length > 0 ? fullName[0] : string.Empty;
            var lastName = fullName.Length > 1 ? fullName[1] : string.Empty;
            var username = textBox2.Text;
            var password = HashPassword(textBox3.Text);
            var email = textBox4.Text;
            
            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                RoleId = 2, 
                ClassGroupId = null,
                DateCreated = DateTime.UtcNow
            };
            
            string selectedSubject = subjectComboBox.SelectedItem as string;
            List<string> subjects = new List<string>();
            if (!string.IsNullOrEmpty(selectedSubject))
            {
                subjects.Add(selectedSubject);
            }
            
            _teacherService.AddNewTeacher(newUser, subjects);
            
            this.Close();
        }


    }
}
