using System.Diagnostics;
using System.Windows;
using DB_Connection;
using BLL;
using LiveCharts;
using LiveCharts.Wpf;
using UI;

namespace UI;

public partial class Generalstatistics : Window
{
    private readonly UserService _userService;
    private readonly StatisticsService _statisticsService;
    public Generalstatistics()
    {
        _userService = new UserService(new JournalDbContext());
        _statisticsService = new StatisticsService(new JournalDbContext());

        InitializeComponent();
        LoadUserCountsChart();
        LoadGradesChart();
        UpdateCounts();
    }

    private void LoadUserCreationChart()
    {
        var userCreationData = _statisticsService.GetUserCreationData();
        var dates = userCreationData.Select(x => x.Date).ToArray();
        var counts = userCreationData.Select(x => x.Count).ToArray();

        UserCreationChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<int>(counts),
                Title = "Користувачі"
            }
        };

        UserCreationChart.AxisX.Add(new Axis
        {
            Title = "Дата",
            Labels = dates.Select(x => x.ToString("d")).ToArray()
        });

        UserCreationChart.AxisY.Add(new Axis
        {
            Title = "Кількість",
            LabelFormatter = value => value.ToString("N0")
        });
    }
    private void LoadUserCountsChart()
    {
        int teacherCount = _userService.GetTeachersCount();
        int studentCount = _userService.GetStudentsCount();

        UserCreationChart.Series = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Вчителі",
                Values = new ChartValues<int> { teacherCount }
            },
            new ColumnSeries
            {
                Title = "Учні",
                Values = new ChartValues<int> { studentCount }
            }
        };

        UserCreationChart.AxisX.Add(new Axis
        {
            Title = "Категорія",
            Labels = new[] { "Вчителі", "Учні" }
        });

        UserCreationChart.AxisY.Add(new Axis
        {
            Title = "Кількість",
            LabelFormatter = value => value.ToString("N0")
        });
    }


    private void LoadGradesChart()
    {
        var gradesData = _statisticsService.GetGradesData();
        double excellentPercentage = (double)gradesData.ExcellentGradesCount / gradesData.TotalGradesCount * 100;
        double otherPercentage = 100 - excellentPercentage; // Відсоток для інших учнів

        GradesChart.Series = new SeriesCollection
        {
            new PieSeries
            {
                Values = new ChartValues<double> { excellentPercentage },
                Title = "Відмінники",
                DataLabels = true,
                LabelPoint = chartPoint => string.Format("{0:N2}%", chartPoint.Y)
            },
            new PieSeries
            {
                Values = new ChartValues<double> { otherPercentage },
                Title = "Усі інші",
                DataLabels = true,
                LabelPoint = chartPoint => string.Format("{0:N2}%", chartPoint.Y)
            }
        };
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