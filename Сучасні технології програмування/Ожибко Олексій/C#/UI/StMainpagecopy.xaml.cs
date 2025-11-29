using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL; // Переконайтеся, що це правильний простір імен для ScheduleService

namespace UI
{
    public partial class StMainpagecopy : Window
    {
        public ObservableCollection<ScheduleDetail> ScheduleDetails { get; private set; }
        private readonly ScheduleService _scheduleService;
        private readonly int _studentId;
        private readonly int _classGroupId;
        private DateTime _selectedDate;

        public StMainpagecopy(DateTime selectedDate, ScheduleService scheduleService, int studentId, int classGroupId)
        {
            _selectedDate = selectedDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(selectedDate, DateTimeKind.Utc) 
                : selectedDate.ToUniversalTime();
            InitializeComponent();
            _scheduleService = scheduleService;
            _studentId = studentId;
            _classGroupId = classGroupId;
            ScheduleDetails = new ObservableCollection<ScheduleDetail>();
            LoadScheduleDetails();
        }

        private void LoadScheduleDetails()
        {
            ScheduleDetails.Clear();
            var selectedDate = _selectedDate.Date; // Використовуйте обрану дату без часової зони

            var detailsFromService = _scheduleService.GetScheduleForClassGroupAndDay(_classGroupId, selectedDate.DayOfWeek, _studentId, selectedDate);

            foreach (var detail in detailsFromService)
            {
                ScheduleDetails.Add(new ScheduleDetail
                {
                    ScheduleId = detail.ScheduleId,
                    SubjectName = detail.SubjectName,
                    StartTime = detail.StartTime,
                    EndTime = detail.EndTime,
                    Status = detail.Status,
                    Grade = detail.Grade
                });
            }

            SubjectsListView.ItemsSource = ScheduleDetails;
            DateTextBlock.Text = _selectedDate.ToString("dddd, dd.MM.yyyy");
        }





        private void SubjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ScheduleDetail selectedDetail)
            {

            }
        }


        private void OnPreviousDayClick(object sender, MouseButtonEventArgs e)
        {
            ChangeDate(-1);
        }

        private void OnNextDayClick(object sender, MouseButtonEventArgs e)
        {
            ChangeDate(1);
        }

        private void ChangeDate(int days)
        {
            do
            {
                _selectedDate = _selectedDate.AddDays(days).ToUniversalTime();
            }
            while (_selectedDate.DayOfWeek == DayOfWeek.Saturday || _selectedDate.DayOfWeek == DayOfWeek.Sunday);

            LoadScheduleDetails();
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

    public class ScheduleDetail
    {
        public int ScheduleId { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string FormattedTime => $"{StartTime:hh\\:mm}-{EndTime:hh\\:mm}";
        public string? Status { get; set; }
        public int? Grade { get; set; }
        
    }
}
