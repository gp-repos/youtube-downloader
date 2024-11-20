
# YoutubeDownloader

YoutubeDownloader is a Windows desktop application built using C#, .NET 8.0, and WPF. The application allows users to download videos from YouTube using the powerful `yt-dlp` tool. It provides a user-friendly interface where you can input a YouTube video URL, track the download progress, and save the video to a specified directory.

---

## Features
- **YouTube Video Download**: Downloads videos from YouTube via the `yt-dlp` command-line tool.
- **Configurable Paths**: Specify the location of `yt-dlp.exe` and the output directory through the configuration file (`appsettings.json`).
- **Progress Tracking**: Displays real-time download progress using a progress bar.
- **Simple UI**: Clean and intuitive interface for easy use.

---

## Prerequisites
- Windows operating system
- .NET 8.0 SDK ([Download Here](https://dotnet.microsoft.com/))
- `yt-dlp.exe` ([Download Here](https://github.com/yt-dlp/yt-dlp))

---

## Installation and Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/YoutubeDownloader.git
   cd YoutubeDownloader
   ```

2. **Add `yt-dlp.exe`**
   - Download `yt-dlp.exe` and place it in the directory specified in `appsettings.json` (default: project folder).

3. **Configure the Application**
   - Open the `appsettings.json` file to specify:
     - Path to `yt-dlp.exe`:
       ```json
       "YtDlpPath": "yt-dlp.exe"
       ```
     - Output directory for downloaded videos:
       ```json
       "OutputDirectory": "C:\\Videos\\YouTubeDownloads"
       ```

4. **Build and Run**
   - Open the solution in Visual Studio and build the project.
   - Run the application to start downloading videos.

---

## Usage

1. **Enter URL**: Paste the YouTube video URL in the text box.
2. **Click Download**: Press the "Download" button to start the download.
3. **Monitor Progress**: The progress bar will update as the video downloads.
4. **Completion**: The video will be saved in the directory specified in the configuration.

---

## Project Structure
- **MainWindow.xaml**: Defines the UI for the application.
- **MainWindow.xaml.cs**: Contains the logic for downloading videos and updating the UI.
- **appsettings.json**: Stores configurable settings such as paths and directories.
- **yt-dlp.exe**: External tool for handling YouTube downloads.

---

## Acknowledgments
This application leverages the following technologies:
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) for downloading videos.
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/) for configuration management.
- WPF and .NET 8.0 for the desktop application framework.

---

## License
This project is licensed under the [MIT License](LICENSE).
