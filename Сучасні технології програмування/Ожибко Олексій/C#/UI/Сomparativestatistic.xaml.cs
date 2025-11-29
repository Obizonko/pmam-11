using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BLL;
using DB_Connection;
using LiveCharts;
using LiveCharts.Wpf;
using UI;

namespace UI;
public partial class Сomparativestatistic : Window
{
    private JournalDbContext _dbContext;
    private readonly UserService _userService;
    private readonly StudentService _studentService;
    public Сomparativestatistic()
    {
        InitializeComponent();
        _userService = new UserService(new JournalDbContext());
        _studentService = new StudentService(new JournalDbContext());
        UpdateCounts();
        _dbContext = new JournalDbContext();
        LoadClassGroups();
    }
    private void UpdateChartData()
    {
        if (comboBox.SelectedItem is ClassGroup selectedClassGroup1 && comboBox3.SelectedItem is ClassGroup selectedClassGroup2)
        {
            string className1 = selectedClassGroup1.ClassGroupName;
            string className2 = selectedClassGroup2.ClassGroupName;

            int classGroupId1 = selectedClassGroup1.ClassGroupId;
            int classGroupId2 = selectedClassGroup2.ClassGroupId;

            var excellentCount1 = _studentService.GetGradeCountByClassGroup(classGroupId1, 10, 12);
            var satisfactoryCount1 = _studentService.GetGradeCountByClassGroup(classGroupId1, 6, 9);
            var unsatisfactoryCount1 = _studentService.GetGradeCountByClassGroup(classGroupId1, 1, 5);

            var excellentCount2 = _studentService.GetGradeCountByClassGroup(classGroupId2, 10, 12);
            var satisfactoryCount2 = _studentService.GetGradeCountByClassGroup(classGroupId2, 6, 9);
            var unsatisfactoryCount2 = _studentService.GetGradeCountByClassGroup(classGroupId2, 1, 5);

            // Оновіть діаграму даними
            UpdateChart(className1, className2, excellentCount1, satisfactoryCount1, unsatisfactoryCount1, excellentCount2, satisfactoryCount2, unsatisfactoryCount2);
        }
    }
    
    private void UpdateChart(string className1, string className2, int excellentCount1, int satisfactoryCount1, int unsatisfactoryCount1, int excellentCount2, int satisfactoryCount2, int unsatisfactoryCount2)
    {
        chart1.Series = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = className1,
                Values = new ChartValues<int> { excellentCount1, satisfactoryCount1, unsatisfactoryCount1 }
            },
            new ColumnSeries
            {
                Title = className2,
                Values = new ChartValues<int> { excellentCount2, satisfactoryCount2, unsatisfactoryCount2 }
            }
        };

        chart1.AxisX.Clear();
        chart1.AxisX.Add(new Axis { Title = "Категорія", Labels = new[] { "Відмінно", "Задовільно", "Незадовільно" } });

        chart1.AxisY.Clear();
        chart1.AxisY.Add(new Axis { Title = "Кількість оцінок" });
    }


    private double CalculatePercentage(int count, int totalCount)
    {
        if (totalCount == 0) return 0;
        return (double)count / totalCount * 100;
    }
    private void LoadClassGroups()
    {
        var classGroups = _studentService.GetAllClassGroups();
        comboBox.ItemsSource = classGroups;
        comboBox.DisplayMemberPath = "ClassGroupName";
        comboBox3.ItemsSource = classGroups;
        comboBox3.DisplayMemberPath = "ClassGroupName";
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

    private void ComparativeStatistics_Click(object sender, RoutedEventArgs e)
    {
        Сomparativestatistic comparativestatistic = new Сomparativestatistic();
        comparativestatistic.Show();
        this.Close();
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
            UpdateChartData();
        }
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
            studentInfoTextBlock.Text = $"Cтатистика учня:\n" +
                                        $"К-сть пропусків: {absencesCount}\n" +
                                        $"К-сть оцінок: {totalGradesCount}\n" +
                                        $"Відмінно (10-12 балів): {excellentCount}\n" +
                                        $"Задовільно (6-9 балів): {satisfactoryCount}\n" +
                                        $"Незадовільно (1-5 балів): {unsatisfactoryCount}";
            
        }
    }
    private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboBox3.SelectedItem is ClassGroup selectedClassGroup)
        {
            var studentCount = _studentService.GetStudentCountByClassGroup(selectedClassGroup.ClassGroupId);
            var totalAbsences = _studentService.GetTotalAbsencesByClassGroup(selectedClassGroup.ClassGroupId);
            var excellentCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 10, 12);
            var satisfactoryCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 6, 9);
            var unsatisfactoryCount = _studentService.GetGradeCountByClassGroup(selectedClassGroup.ClassGroupId, 1, 5);
            var totalGradesCount = _studentService.GetTotalGradesCountByClassGroup(selectedClassGroup.ClassGroupId);

            classInfoTextBlock2.Text = $"Cтатистика класу:\n" +
                                       $"К-сть учнів: {studentCount}\n" +
                                       $"К-сть пропусків: {totalAbsences}\n" +
                                       $"К-сть оцінок: {totalGradesCount}\n" +
                                       $"Відмінно (10-12 балів): {excellentCount}\n" +
                                       $"Задовільно (6-9 балів): {satisfactoryCount}\n" +
                                       $"Незадовільно (1-5 балів): {unsatisfactoryCount}";

            var students = _studentService.GetStudentsByClassGroup(selectedClassGroup.ClassGroupId);
            comboBox4.ItemsSource = students;
            comboBox4.DisplayMemberPath = "FullName";
            studentInfoTextBlock2.Text = "";
            comboBox4.SelectedIndex = -1;
            UpdateChartData();
        }
    }



    private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboBox4.SelectedItem is User selectedStudent)
        {
            var absencesCount = _studentService.GetAbsencesCountByStudent(selectedStudent.UserId);
            var excellentCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 10, 12);
            var satisfactoryCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 6, 9);
            var unsatisfactoryCount = _studentService.GetGradeCountByStudent(selectedStudent.UserId, 1, 5);
            var totalGradesCount = _studentService.GetTotalGradesCountByStudent(selectedStudent.UserId);
            studentInfoTextBlock2.Text = $"Cтатистика учня:\n" +
                                        $"К-сть пропусків: {absencesCount}\n" +
                                        $"К-сть оцінок: {totalGradesCount}\n" +
                                        $"Відмінно (10-12 балів): {excellentCount}\n" +
                                        $"Задовільно (6-9 балів): {satisfactoryCount}\n" +
                                        $"Незадовільно (1-5 балів): {unsatisfactoryCount}";
            
        }
    }

        private void SelectiveStatistics_Click(object sender, RoutedEventArgs e)
        {
            Samplestatistics samplestatistics = new Samplestatistics();
            samplestatistics.Show();
            this.Close();
        }
        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginPage = new MainWindow();

            // Встановіть сторінку логіну як вміст поточного вікна
            Application.Current.MainWindow = loginPage;
            loginPage.Show();

            // Закрийте поточне вікно RecoveryPage
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