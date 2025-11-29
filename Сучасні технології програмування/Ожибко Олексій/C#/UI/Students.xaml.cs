using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DB_Connection;
using BLL;

using UI;

namespace UI
{
    /// <summary>
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class Students : Window
    {
        private readonly StudentService _studentService;
        private readonly UserService _userService;
        public Students()
        {
            InitializeComponent();
            _studentService = new StudentService(new JournalDbContext());
            LoadStudents();
            _userService = new UserService(new JournalDbContext());
            UpdateCounts();
        }
        private void UpdateCounts()
        {
            teachersCountTextBlock.Text = "Кількість вчителів: " + _userService.GetTeachersCount().ToString();
            studentsCountTextBlock.Text = "Кількість учнів: " + _userService.GetStudentsCount().ToString();
        }
        private void LoadStudents()
        {
            studentsDataGrid.ItemsSource = _studentService.GetAllStudents();
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            AdminPageaddstudent addStudentWindow = new AdminPageaddstudent();
            addStudentWindow.ShowDialog(); 
        }


        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                _studentService.DeleteStudent(userId);
                LoadStudents();
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchTextBox.Text.Length == 0)
            {
                LoadStudents();
            }
            else
            {
                studentsDataGrid.ItemsSource = _studentService.GetAllStudents()
                    .Where(s => s.FirstName.Contains(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                                s.LastName.Contains(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                                s.ClassGroup.ClassGroupName.Contains(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                                s.Email.Contains(searchTextBox.Text, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader column)
            {
                string sortBy = column.Tag.ToString();
                SortStudentsList(sortBy);
            }
        }

        private void SortStudentsList(string sortBy)
        {
            var sortedList = _studentService.GetAllStudents() 
                .OrderBy(property => property.GetType().GetProperty(sortBy).GetValue(property, null))
                .ToList();
            studentsDataGrid.ItemsSource = sortedList;
        }




        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            studentsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            foreach (var item in studentsDataGrid.Items)
            {
                if (item is User student)
                {
                    _studentService.UpdateStudent(student);
                }
            }
            MessageBox.Show("Зміни збережено!");
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

        public class ViewModelBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
