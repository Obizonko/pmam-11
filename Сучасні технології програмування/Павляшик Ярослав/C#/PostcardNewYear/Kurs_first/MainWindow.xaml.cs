using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Kurs_first
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private DispatcherTimer snowfallTimer = new DispatcherTimer();
        private List<Tuple<TextBlock, Storyboard, DispatcherTimer>> animatedTextList = new List<Tuple<TextBlock, Storyboard, DispatcherTimer>>();


        private bool isDragging = false;
        private double offsetX, offsetY;


        private double fallSpeed = 2;
        public double FallSpeed
        {
            get { return fallSpeed; }
            set { fallSpeed = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            snowfallTimer.Interval = TimeSpan.FromMilliseconds(5);
            snowfallTimer.Tick += SnowfallTimer_Tick;
        }

        private void StartSnowfall_Click(object sender, RoutedEventArgs e)
        {
            StartSnowfall();
            AddAnimatedText();

        }

        private void StartSnowfall()
        {
            snowfallTimer.Start();
        }

        private void SnowfallTimer_Tick(object sender, EventArgs e)
        {
            AddSnowflake();
        }
        private void AddSnowflake()
        {
            double horizontalDirection = -1;
            double snowflakeSize = 30;
            double angle = MyCanvas.ActualWidth / 2;
            double left = random.NextDouble() * (MyCanvas.ActualWidth + 2 * angle) - angle;
            double top = -snowflakeSize;

            System.Windows.Controls.Image snowflake = new System.Windows.Controls.Image
            {
                Width = snowflakeSize,
                Height = snowflakeSize,
                Source = new BitmapImage(new Uri("D:\\Programming\\Visual Studio\\C#\\University\\Kurs_first\\firstPlace.png", UriKind.RelativeOrAbsolute))
            };

            Canvas.SetLeft(snowflake, left);
            Canvas.SetTop(snowflake, top);

            RotateTransform rotateTransform = new RotateTransform();
            TranslateTransform verticalTranslateTransform = new TranslateTransform();
            TranslateTransform horizontalTranslateTransform = new TranslateTransform();
            snowflake.RenderTransform = new TransformGroup { Children = { rotateTransform, verticalTranslateTransform, horizontalTranslateTransform } };

            MyCanvas.Children.Add(snowflake);


            DoubleAnimation verticalAnimation = new DoubleAnimation
            {
                To = MyCanvas.ActualHeight,
                Duration = TimeSpan.FromSeconds(random.Next(5, 10) / FallSpeed)
            };

            verticalAnimation.Completed += (sender, e) =>
            {
                MyCanvas.Children.Remove(snowflake);
            };

            verticalTranslateTransform.BeginAnimation(TranslateTransform.YProperty, verticalAnimation);


            DoubleAnimation horizontalAnimation = new DoubleAnimation
            {
                By = horizontalDirection * angle,
                Duration = TimeSpan.FromSeconds(random.Next(5, 10) / FallSpeed)
            };

            horizontalTranslateTransform.BeginAnimation(TranslateTransform.XProperty, horizontalAnimation);
        }


        private void AddAnimatedText() 
        {
            TextBlock animatedText = new TextBlock
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 50, 0, 0)
            };

            animatedText.RenderTransform = new TranslateTransform();
            MyCanvas.Children.Add(animatedText);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                double textWidth = animatedText.ActualWidth;
                double textHeight = animatedText.ActualHeight;

                string text = "З Новим Роком!";

                List<Tuple<Run, Color>> runColors = new List<Tuple<Run, Color>>();

                foreach (var word in text.Split(' '))
                {
                    Run run = new Run(word + " ");
                    Color wordColor = GetRandomColor(); 
                    run.Foreground = new SolidColorBrush(wordColor);
                    runColors.Add(new Tuple<Run, Color>(run, wordColor));
                }

                animatedText.Inlines.AddRange(runColors.Select(tuple => tuple.Item1));

                DoubleAnimation xAnimation = new DoubleAnimation
                {
                    From = MyCanvas.ActualWidth,
                    To = -2 * textWidth,
                    Duration = TimeSpan.FromSeconds(5),
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(xAnimation);

                Storyboard.SetTarget(xAnimation, animatedText);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

                animatedText.Margin = new Thickness(0, 50, 0, 0);

                storyboard.Begin();

                DispatcherTimer colorChangeTimer = new DispatcherTimer();
                colorChangeTimer.Interval = TimeSpan.FromSeconds(2);
                colorChangeTimer.Tick += (sender, args) =>
                {
                    foreach (var runColor in runColors)
                    {
                        runColor.Item1.Foreground = new SolidColorBrush(GetRandomColor());
                    }
                };
                colorChangeTimer.Start();

                animatedTextList.Add(new Tuple<TextBlock, Storyboard, DispatcherTimer>(animatedText, storyboard, colorChangeTimer));
            }));
        }


        private Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }

        private void StopAnimation_Click(object sender, RoutedEventArgs e)
        {
            snowfallTimer.Stop();

            foreach (var animatedTextTuple in animatedTextList)
            {
                var animatedText = animatedTextTuple.Item1;
                var storyboard = animatedTextTuple.Item2;
                var colorChangeTimer = animatedTextTuple.Item3;

                storyboard.Stop();
                colorChangeTimer.Stop();

                double textWidth = animatedText.ActualWidth;

                if (Canvas.GetLeft(animatedText) <= -2 * textWidth)
                {
                    animatedText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Canvas.SetLeft(animatedText, -2 * textWidth);
                }
            }
        }


        private void GrayToy_Click(object sender, RoutedEventArgs e)
        {
            AddToy("D:\\Programming\\Visual Studio\\C#\\University\\Kurs_first\\gray_toy_new.png"); 
        }

        private void RedToy_Click(object sender, RoutedEventArgs e)
        {
            AddToy("D:\\Programming\\Visual Studio\\C#\\University\\Kurs_first\\red_toy_new.png"); 
        }

        private void BlueToy_Click(object sender, RoutedEventArgs e)
        {
            AddToy("D:\\Programming\\Visual Studio\\C#\\University\\Kurs_first\\toy_new.png"); 
        }


        private void AddToy(string imagePath)
        {
            System.Windows.Controls.Image toyImage = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)),
                Width = 50,
                Height = 50,
            };

            Canvas.SetLeft(toyImage, 200);
            Canvas.SetTop(toyImage, 200);

            MyCanvas.Children.Add(toyImage);

            toyImage.MouseLeftButtonDown += ToyImage_MouseLeftButtonDown;
            toyImage.MouseMove += ToyImage_MouseMove;
            toyImage.MouseLeftButtonUp += ToyImage_MouseLeftButtonUp;
        }




        private void ToyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var image = (System.Windows.Controls.Image)sender;
            var position = e.GetPosition(MyCanvas);

            offsetX = position.X - Canvas.GetLeft(image);
            offsetY = position.Y - Canvas.GetTop(image);
        }

        private void ToyImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var image = (System.Windows.Controls.Image)sender;
                var position = e.GetPosition(MyCanvas);

                Canvas.SetLeft(image, position.X - offsetX);
                Canvas.SetTop(image, position.Y - offsetY);
            }
        }

        private void ToyImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

    }

}
