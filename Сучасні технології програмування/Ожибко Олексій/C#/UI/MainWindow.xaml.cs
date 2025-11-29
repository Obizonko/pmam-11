using System.Diagnostics;
using System.Windows;
using BLL;
using DB_Connection;
using UI;

namespace UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = usernameTextBox.Text;
            var password = passwordBox.Password;
            var authService = new UsersAuthentication(new JournalDbContext());

            var user = authService.AuthenticateUser(username, password);

            if (user != null)
            {
                NavigateToPage(user);
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль. Спробуйте ще раз.");
            }
        }

        private void NavigateToPage(User user)
        {
            var scheduleService = new ScheduleService(new JournalDbContext());

            Window? page = null;
            if (user.RoleId == 1) 
            {
                page = new StudentPage(scheduleService, user.UserId, user.ClassGroupId ?? 0);
            }
            else if (user.RoleId == 2) 
            {
                page = new TeacherPage();
                
            }
            else if (user.RoleId == 3)
            {
                page = new AdminPage();
            }

            page?.Show();
            this.Close();
        }


        private void RecoveryLink_Click(object sender, RoutedEventArgs e)
        {
            RecoveryPage recoveryPage = new RecoveryPage();
            recoveryPage.Show();
            this.Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://t.me/EduJ_Support",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка. Не вдається відкрити посилання. Пряме посилання: https://t.me/EduJ_Support {ex.Message}");
            }
        }

    }
}