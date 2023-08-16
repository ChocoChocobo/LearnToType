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

namespace LearnToType
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Additional
        private Random random = new Random();
        private DispatcherTimer keyHighlightTimer = new DispatcherTimer();

        // Constants
        private const string LOWER_CASE_CHARS = "abcdefghijklmnopqrstuvwxyz";
        private const string UPPER_CASE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Fields
        private int sliderValue = 0;
        public int SliderValue
        {
            get { return sliderValue; }
            set
            {
                sliderValue = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PreviewKeyDown += MainWindow_PreviewKeyDown;

            keyHighlightTimer.Interval = TimeSpan.FromSeconds(1);
            keyHighlightTimer.Tick += KeyHighlightTimer_Tick;
        }

        // Highlighting the keys on the virtual keyboard
        private void KeyHighlightTimer_Tick(object? sender, EventArgs e)
        {
            foreach (var rowStackPanel in lowercaseStackPanel.Children.OfType<StackPanel>())
            {
                foreach (var grid in rowStackPanel.Children.OfType<Grid>())
                {
                    // Resetting the background and opacity of all keys
                    grid.Background = Brushes.Transparent;
                    grid.Opacity = 1.0;
                }
            }

            keyHighlightTimer.Stop();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string pressedKey = e.Key.ToString().ToLower();
            if (pressedKey.Length == 1 && Char.IsLetterOrDigit(pressedKey[0]))
            {
                Grid? virtualKeyGrid = FindName(pressedKey + "KeyGrid") as Grid;

                if (virtualKeyGrid != null)
                {
                    // Removing highlights from all keys before highlighting the current one
                    foreach (var grid in lowercaseStackPanel.Children.OfType<Grid>())
                    {
                        grid.Background = Brushes.Transparent;
                    }

                    virtualKeyGrid.Background = Brushes.Yellow;

                    UserInputTextBlock.Text += pressedKey;
                    CheckUserInput();
                    e.Handled = true;
                    keyHighlightTimer.Start();
                }
            }
            else if (e.Key == Key.Back) // Backspace handle
            {
                if (UserInputTextBlock.Text.Length > 0)
                {
                    UserInputTextBlock.Text = UserInputTextBlock.Text.Substring(0, UserInputTextBlock.Text.Length - 1);
                }

                e.Handled = true;
            }
        }

        private void CheckUserInput()
        {
            string generatedString = GeneratedStringTextBlock.Text;
            string userInput = UserInputTextBlock.Text;

            int minLength = Math.Min(generatedString.Length, userInput.Length);

            UserInputTextBlock.Inlines.Clear();

            for (int i = 0; i < minLength; i++)
            {
                if (userInput[i] == generatedString[i])
                {
                    // Brusing a correctly typed character in green
                    UserInputTextBlock.Inlines.Add(new Run(userInput[i].ToString()) { Background = Brushes.LightGreen });
                }
                else
                {
                    // Brusing an incorrectly typed character in red
                    UserInputTextBlock.Inlines.Add(new Run(userInput[i].ToString()) { Background = Brushes.Red });
                }
            }

            if (userInput.Length > generatedString.Length)
            {
                UserInputTextBlock.Inlines.Add(new Run(userInput.Substring(minLength)) { Foreground = Brushes.Red });
            }
        }

        // Start button event including case sensitivity (case sensitivity doesn`t work! NOT IMPLEMENTED YET!)
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int stringLength = (int)difficultySlider.Value;
            SliderValue = stringLength;

            UserInputTextBlock.Text = "";
            UserInputTextBlock.Background = Brushes.LightGreen;

            bool caseSensitive = caseSensitiveCheckBox.IsChecked ?? false;
            string generatedString = GenerateRandomString(stringLength, caseSensitive);
            GeneratedStringTextBlock.Text = generatedString;
        }

        private string GenerateRandomString(int length, bool caseSensitive)
        {
            StringBuilder builder = new StringBuilder();
            string characters = caseSensitive ? UPPER_CASE_CHARS + LOWER_CASE_CHARS : LOWER_CASE_CHARS;

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(characters.Length);
                builder.Append(characters[randomIndex]);
            }

            return builder.ToString();
        }
    }
}
