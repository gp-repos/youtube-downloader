using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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

            StatusTextBlock.Text = "Status: " + CookieHelper.CookieCopier();

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

            var arguments =
                $"-f \"bv*+ba/b\" -o \"{Path.Combine(outputDir, "%(title)s.%(ext)s")}\" --windows-filenames --trim-filenames 200 --cookies youtube_cookies.txt {url}";
            StatusTextBlock.Text = "Status: Downloading...";
            DownloadButton.IsEnabled = false;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = 100;
            DownloadProgressBar.Value = 0;
            
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
                        var percentage = ExtractPercentage(line);
                        if (percentage.HasValue)
                        {
                            Dispatcher.Invoke(() => DownloadProgressBar.Value = percentage.Value);
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

        private int? ExtractPercentage(string input)
        {
            // Define a regex pattern to match percentages (digits followed by %)
            var pattern = @"\b(\d+)%\b";

            // Use Regex to find a match
            var match = Regex.Match(input, pattern);

            // If a match is found, parse it as an integer; otherwise, return null
            return match.Success ? int.Parse(match.Groups[1].Value) : (int?)null;
        }
    }
}
