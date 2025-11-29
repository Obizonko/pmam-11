using System.Diagnostics;
using System.Windows;
using BLL;
using DB_Connection;
using UI;

namespace UI
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        private readonly UserService _userService;

        public AdminPage()
        {
            InitializeComponent();
            _userService = new UserService(new JournalDbContext());
            UpdateCounts();
            
        }
        private void UpdateCounts()
        {
            teachersCountTextBlock.Text = "Кількість вчителів: " + _userService.GetTeachersCount().ToString();
            studentsCountTextBlock.Text = "Кількість учнів: " + _userService.GetStudentsCount().ToString();
        }
        private void EditTeachers_Click(object sender, RoutedEventArgs e)
        {
            Teachers teachers = new Teachers();
            teachers.Show();
            this.Close();
        }

        private void EditStudents_Click(object sender, RoutedEventArgs e)
        {
            Students students = new Students();
            students.Show();
            this.Close();
        }

        private void ViewSchedule_Click(object sender, RoutedEventArgs e)
        {
            Shedule shedule = new Shedule();
            shedule.Show();
            this.Close();
        }

        private void GeneralStatistics_Click(object sender, RoutedEventArgs e)
        {
            Generalstatistics generalstatistics = new Generalstatistics();
            generalstatistics.Show();
            this.Close();
        }

        private void SelectiveStatistics_Click(object sender, RoutedEventArgs e)
        {
            Samplestatistics samplestatistics = new Samplestatistics();
            samplestatistics.Show();
            this.Close();
        }

        private void ComparativeStatistics_Click(object sender, RoutedEventArgs e)
        {
            Сomparativestatistic comparativestatistic = new Сomparativestatistic();
            comparativestatistic.Show();
            this.Close();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginPage = new MainWindow();
            
            Application.Current.MainWindow = loginPage;
            loginPage.Show();
            
            this.Close();
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void TechSupport_Click(object sender, RoutedEventArgs e)
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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
