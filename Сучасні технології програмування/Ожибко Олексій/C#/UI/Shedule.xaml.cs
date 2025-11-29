using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using BLL; 
using DB_Connection;
namespace UI
{
    public partial class Shedule : Window
    {
        
        public ObservableCollection<User> Teachers { get; set; }
        private readonly TeacherService _teacherService;
        private readonly UserService _userService;

        public Shedule()
        {
            InitializeComponent();

            _teacherService = new TeacherService(new JournalDbContext());
            _userService = new UserService(new JournalDbContext());
            LoadTeachers();
        }
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_userService == null) 
            {
                MessageBox.Show("Помилка сервісу видалення.");
                return;
            }

            DataGrid currentDataGrid = null;
            
            var selectedTab = tabControlDaysOfWeek.SelectedItem as TabItem;
            if (selectedTab != null && selectedTab.Content is DataGrid)
            {
                currentDataGrid = selectedTab.Content as DataGrid;
            }
            
            if (currentDataGrid != null && currentDataGrid.SelectedItem is Schedule selectedSchedule)
            {
                var schedules = currentDataGrid.ItemsSource as ObservableCollection<Schedule>;
                schedules?.Remove(selectedSchedule);
                
                _userService.DeleteSchedule(selectedSchedule);
            }
            else
            {
                MessageBox.Show("Не вибрано рядок для видалення.");
            }
        }



        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveDataGridChanges(dataGridMonday, "Понеділок");
            RefreshDataGrid(dataGridMonday, "Понеділок");
            SaveDataGridChanges(dataGridTuesday, "Вівторок");
            RefreshDataGrid(dataGridTuesday, "Вівторок");
            SaveDataGridChanges(dataGridWednesday, "Середа");
            RefreshDataGrid(dataGridWednesday, "Середа");
            SaveDataGridChanges(dataGridThursday, "Четвер");
            RefreshDataGrid(dataGridThursday, "Четвер");
            SaveDataGridChanges(dataGridFriday, "П'ятниця");
            RefreshDataGrid(dataGridFriday, "П'ятниця");
            
        }
        

        private void SaveDataGridChanges(DataGrid dataGrid, string dayOfWeek)
        {
            var schedules = dataGrid.ItemsSource as IEnumerable<Schedule>;
            if (schedules == null) return;

            var selectedTeacher = comboBoxTeachers.SelectedItem as User;
            int teacherId = selectedTeacher?.UserId ?? 0;

            foreach (var schedule in schedules)
            {
                bool isNew = schedule.ScheduleId == 0;

                if (!string.IsNullOrEmpty(schedule.ClassGroup?.ClassGroupName))
                {
                    var classGroupId = _teacherService.GetClassGroupIdByName(schedule.ClassGroup.ClassGroupName);
                    if (classGroupId != -1)
                    {
                        schedule.ClassGroupId = classGroupId;
                    }
                }

                if (isNew)
                {
                    schedule.SubjectId = 1;
                    schedule.ClassGroupId = 1;
                    schedule.TeacherId = teacherId;
                    schedule.DayOfWeek = dayOfWeek;
                    _teacherService.AddNewSchedule(schedule);
                }
                else
                {
                    if (!string.IsNullOrEmpty(schedule.Subject?.SubjectName))
                    {
                        var subjectId = _teacherService.GetSubjectIdByName(schedule.Subject.SubjectName);
                        if (subjectId != -1)
                        {
                            schedule.SubjectId = subjectId;
                        }
                        else
                        {
                            continue; 
                        }
                    }

                    _teacherService.UpdateSchedule(schedule);
                }
            }
        }






        private bool IsValidSchedule(Schedule schedule)
        {
            return schedule.StartTime != TimeSpan.Zero &&
                   schedule.EndTime != TimeSpan.Zero &&
                   schedule.SubjectId != 0 &&
                   schedule.ClassGroupId != 0;
        }
        
        private int GetSubjectIdByName(string subjectName)
        {
            return _teacherService.GetSubjectIdByName(subjectName);
        }

        private void RefreshDataGrid(DataGrid dataGrid, string dayOfWeek)
        {
            if (comboBoxTeachers.SelectedItem is User selectedTeacher)
            {
                var schedule = _teacherService.GetScheduleForTeacher(selectedTeacher.UserId, dayOfWeek);
                dataGrid.ItemsSource = schedule;
            }
        }
        
        private void LoadTeachers()
        {
            var teachersList = _teacherService.GetAllTeachers();
            
            Teachers = new ObservableCollection<User>(teachersList);
            comboBoxTeachers.ItemsSource = Teachers;
        }

        private void ComboBoxTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTeachers.SelectedItem is User selectedTeacher)
            {
                UpdateScheduleForDay(selectedTeacher.UserId, "Понедiлок", dataGridMonday);
                UpdateScheduleForDay(selectedTeacher.UserId, "Вiвторок", dataGridTuesday);
                UpdateScheduleForDay(selectedTeacher.UserId, "Середа", dataGridWednesday);
                UpdateScheduleForDay(selectedTeacher.UserId, "Четвер", dataGridThursday);
                UpdateScheduleForDay(selectedTeacher.UserId, "П'ятниця", dataGridFriday);
            }
        }


        private void UpdateScheduleForDay(int teacherId, string dayOfWeek, DataGrid dataGrid)
        {
            var schedule = _teacherService.GetScheduleForTeacher(teacherId, dayOfWeek);
            dataGrid.ItemsSource = schedule;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            
           AdminPage adminPage = new AdminPage();
           
            Application.Current.MainWindow = adminPage;
            adminPage.Show();
            
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
