using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using BLL; // Використання BLL

namespace UI
{
    public partial class StudentPage : Window
    {
        private StMainpagecopy _activeStMainpagecopy;
        private readonly ScheduleService _scheduleService;
        private readonly int _studentId;
        private readonly int _classGroupId;

        public StudentPage(ScheduleService scheduleService, int studentId, int classGroupId)
        {
            InitializeComponent();
            _scheduleService = scheduleService;
            _studentId = studentId;
            _classGroupId = classGroupId;
            studentCalendar.PreviewMouseUp += StudentCalendar_PreviewMouseUp;
        }

        private void StudentCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is DateTime selectedDate)
            {
                if (selectedDate.DayOfWeek != DayOfWeek.Saturday && selectedDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    _activeStMainpagecopy?.Close();
                    _activeStMainpagecopy = new StMainpagecopy(selectedDate, _scheduleService, _studentId, _classGroupId);
                    _activeStMainpagecopy.Show();
                }
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