using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace YouTubeDownloader
{
    public partial class MainWindow : Window
    {
        private readonly string ytDlpPath;
        private readonly string outputDir;

        public MainWindow()
        {
            InitializeComponent();

            // Load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ytDlpPath = config["YtDlpPath"];
            outputDir = config["OutputDirectory"];

            // Ensure output directory exists
            Directory.CreateDirectory(outputDir);
        }
        
        private void UrlTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // This method updates the placeholder visibility as the user types.
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlTextBox.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                StatusTextBlock.Text = "Status: Please enter a valid URL.";
                return;
            }

            if (!File.Exists(ytDlpPath))
            {
                StatusTextBlock.Text = $"Status: {ytDlpPath} not found!";
                return;
            }

            var arguments = $"-f \"bv*+ba/b\" -o \"{Path.Combine(outputDir, "%(title)s.%(ext)s")}\" --cookies www.youtube.com_cookies.txt {url}";
            StatusTextBlock.Text = "Status: Downloading...";
            DownloadButton.IsEnabled = false;

            try
            {
                await RunYtDlp(arguments);
                StatusTextBlock.Text = "Status: Download completed!";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Status: Error - {ex.Message}";
            }
            finally
            {
                DownloadButton.IsEnabled = true;
            }
        }

        private Task RunYtDlp(string arguments)
        {
            return Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ytDlpPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (line.Contains("%"))
                    {
                        var percentageString = line.Split('%')[0].Trim();
                        if (double.TryParse(percentageString, out var percentage))
                        {
                            Dispatcher.Invoke(() => DownloadProgressBar.Value = percentage);
                        }
                    }
                }

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    throw new Exception(error);
                }
            });
        }

    }
}
