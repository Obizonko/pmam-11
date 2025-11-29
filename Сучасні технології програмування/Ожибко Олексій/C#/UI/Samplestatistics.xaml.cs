using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DB_Connection;
using BLL;
using LiveCharts;
using LiveCharts.Wpf;
using UI;

namespace UI;

public partial class Samplestatistics : Window
{
    private JournalDbContext _dbContext;
    private readonly UserService _userService;
    private readonly StudentService _studentService;
    public Samplestatistics()
    {
        InitializeComponent();
        _userService = new UserService(new JournalDbContext());
        _studentService = new StudentService(new JournalDbContext());
        UpdateCounts();
        _dbContext = new JournalDbContext();
        LoadClassGroups();
    }
    private void UpdateCounts()
    {
        teachersCountTextBlock.Text = "Кількість вчителів: " + _userService.GetTeachersCount().ToString();
        studentsCountTextBlock.Text = "Кількість учнів: " + _userService.GetStudentsCount().ToString();
    }
    private void LoadClassGroups()
    {
        var classGroups = _dbContext.ClassGroups.ToList(); 
        comboBox.ItemsSource = classGroups; 
        comboBox.DisplayMemberPath = "ClassGroupName"; 
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
    private void UpdateClassGradesPieChart(int excellentCount, int satisfactoryCount, int unsatisfactoryCount)
    {
        var seriesCollection = new SeriesCollection
        {
            new PieSeries
            {
                Title = "Відмінно",
                Values = new ChartValues<int> { excellentCount },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Задовільно",
                Values = new ChartValues<int> { satisfactoryCount },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Незадовільно",
                Values = new ChartValues<int> { unsatisfactoryCount },
                DataLabels = true
            }
        };

        classGradesPieChart.Series = seriesCollection;
    }
    private void UpdateStudentGradesPieChart(int excellentCount, int satisfactoryCount, int unsatisfactoryCount)
    {
        var seriesCollection = new SeriesCollection
        {
            new PieSeries
            {
                Title = "Відмінно",
                Values = new ChartValues<int> { excellentCount },
                DataLabels = true,
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
            },
            new PieSeries
            {
                Title = "Задовільно",
                Values = new ChartValues<int> { satisfactoryCount },
                DataLabels = true,
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
            },
            new PieSeries
            {
                Title = "Незадовільно",
                Values = new ChartValues<int> { unsatisfactoryCount },
                DataLabels = true,
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
            }
        };

        studentGradesPieChart.Series = seriesCollection;
    }


    private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboBox.SelectedItem is ClassGroup selectedClassGroup)
        {
            var studentCount = _studentService.GetStudentCountByClassGroup(selectedClassGroup.ClassGroupId);
            var totalAbsences = _studentService.GetTotalAbsencesByClassGroup(selectedClassGroup.ClassGroupId);
            var excellentCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 10, 12);
            var satisfactoryCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 6, 9);
            var unsatisfactoryCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 1, 5);
            var totalGradesCount = _studentService.GetTotalGradesCountByClassGroup(selectedClassGroup.ClassGroupId);
            
            classInfoTextBlock.Text = $"Cтатистика класу:\n" +
                                      $"К-сть учнів: {studentCount}\n" +
                                      $"К-сть пропусків: {totalAbsences}\n" +
                                      $"К-сть оцінок: {totalGradesCount}\n" +
                                      $"Відмінно (10-12 балів): {excellentCount}\n" +
                                      $"Задовільно (6-9 балів): {satisfactoryCount}\n" +
                                      $"Незадовільно (1-5 балів): {unsatisfactoryCount}";
            var students = _studentService.GetStudentsByClassGroup(selectedClassGroup.ClassGroupId);
            comboBox2.ItemsSource = students;
            comboBox2.DisplayMemberPath = "FullName";
            studentInfoTextBlock.Text = "";
            comboBox2.SelectedIndex = -1; 
            UpdateClassGradesPieChart(excellentCount, satisfactoryCount, unsatisfactoryCount);
            ClearStudentGradesPieChart();
        }
    }
    private void ClearStudentGradesPieChart()
    {
        studentGradesPieChart.Series.Clear();
    }


    private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboBox2.SelectedItem is User selectedStudent)
        {
            var absencesCount = _studentService.GetAbsencesCountByStudent(selectedStudent.UserId);
            var excellentCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 10, 12);
            var satisfactoryCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 6, 9);
            var unsatisfactoryCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 1, 5);
            var totalGradesCount = _studentService.GetTotalGradesCountByStudent(selectedStudent.UserId);
            UpdateStudentGradesPieChart(excellentCount, satisfactoryCount, unsatisfactoryCount);
            studentInfoTextBlock.Text = $"Cтатистика учня:\n" +
                                        $"К-сть пропусків: {absencesCount}\n" +
                                        $"К-сть оцінок: {totalGradesCount}\n" +
                                        $"Відмінно (10-12 балів): {excellentCount}\n" +
                                        $"Задовільно (6-9 балів): {satisfactoryCount}\n" +
                                        $"Незадовільно (1-5 балів): {unsatisfactoryCount}";
            
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