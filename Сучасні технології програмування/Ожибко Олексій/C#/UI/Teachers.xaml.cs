using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using BLL;
using DB_Connection;


namespace UI
{
    /// <summary>
    /// Interaction logic for Teachers.xaml
    /// </summary>
    public partial class Teachers : Window
    {
        private readonly UserService _userService;
        private readonly TeacherService _teacherService;
        public Teachers()
        {
            InitializeComponent(); ;
            _teacherService = new TeacherService(new JournalDbContext());
            _userService = new UserService(new JournalDbContext());
            UpdateCounts();
            LoadTeachers();
        }
        private void LoadTeachers()
        {
            var teachers = _teacherService.GetAllTeachers();
            teachersDataGrid.ItemsSource = teachers;
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = searchTextBox.Text.ToLower();
            var filteredTeachers = _teacherService.GetAllTeachers()
                .Where(teacher => teacher.FirstName.ToLower().Contains(searchText) 
                                  || teacher.LastName.ToLower().Contains(searchText)
                                  || teacher.Username.ToLower().Contains(searchText))
                .ToList();

            teachersDataGrid.ItemsSource = filteredTeachers;
        }
        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити цього вчителя?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _teacherService.DeleteTeacher(userId);
                    LoadTeachers(); 
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            teachersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            foreach (var item in teachersDataGrid.Items)
            {
                if (item is User teacher)
                {
                    _teacherService.UpdateTeacher(teacher);
                }
            }
            MessageBox.Show("Зміни збережено!");
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader column)
            {
                string sortBy = column.Tag.ToString();
                SortTeachersList(sortBy);
            }
        }

        private void SortTeachersList(string sortBy)
        {
            var sortedList = _teacherService.GetAllTeachers()
                .OrderBy(property => property.GetType().GetProperty(sortBy).GetValue(property, null))
                .ToList();
            teachersDataGrid.ItemsSource = sortedList;
        }
        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            var addTeacherWindow = new AdminPageaddteacher();
            addTeacherWindow.ShowDialog();
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
        private void UpdateCounts()
        {
            teachersCountTextBlock.Text = "Кількість вчителів: " + _userService.GetTeachersCount().ToString();
            studentsCountTextBlock.Text = "Кількість учнів: " + _userService.GetStudentsCount().ToString();
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
