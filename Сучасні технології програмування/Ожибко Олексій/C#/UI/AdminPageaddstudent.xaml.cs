using System.Text;
using System.Windows;
using BLL;
using DB_Connection;


namespace UI
{
    /// <summary>
    /// Interaction logic for AdminPageaddstudent.xaml
    /// </summary>
    public partial class AdminPageaddstudent : Window
    {
        private readonly StudentService _studentService;

        public AdminPageaddstudent()
        {
            InitializeComponent();
            _studentService = new StudentService(new JournalDbContext());
            LoadClassGroups();
        }

        private void LoadClassGroups()
        {
            var classGroups = _studentService.GetAllClassGroups();
            comboBox.ItemsSource = classGroups.Select(cg => cg.ClassGroupName).ToList();
        }
        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = textBox1.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var firstName = fullName.Length > 0 ? fullName[0] : string.Empty;
            var lastName = fullName.Length > 1 ? fullName[1] : string.Empty;
            var username = textBox2.Text;
            var password = HashPassword(textBox3.Text);
            var email = textBox4.Text;
            var selectedClassGroupName = comboBox.SelectedItem as string;

            int classGroupId = _studentService.GetClassGroupIdByName(selectedClassGroupName);

            var newStudent = new User
            {
                Username = username,
                Password = password,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                RoleId = 1, 
                ClassGroupId = classGroupId,
                DateCreated = DateTime.UtcNow
            };

            _studentService.AddNewStudent(newStudent);
            this.Close();
        }
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
        
    }

}
