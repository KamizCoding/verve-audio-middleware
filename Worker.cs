using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OneDriveHelper _driveHelper;
    private readonly AudioUploader _uploader;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        _driveHelper = new OneDriveHelper(
            Environment.GetEnvironmentVariable("ONEDRIVE_CLIENT_ID"),
            Environment.GetEnvironmentVariable("ONEDRIVE_TENANT_ID"),
            Environment.GetEnvironmentVariable("ONEDRIVE_CLIENT_SECRET"),
            Environment.GetEnvironmentVariable("ONEDRIVE_REFRESH_TOKEN"),
            "/Recordings"
        );

        _uploader = new AudioUploader("https://audiotranslator.onrender.com/api/audio/mic-translate");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔁 Verve Middleware Job started...");

        try
        {
            var files = await _driveHelper.GetAudioFilesSortedAsync();

            if (files == null || files.Count == 0)
            {
                _logger.LogInformation("✅ No new files found. Exiting.");
                return;
            }

            foreach (var file in files)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("🛑 Cancellation requested. Exiting job.");
                    break;
                }

                var stream = await _driveHelper.DownloadFileAsync(file.Id);
                await _uploader.UploadAudioAsync(stream, file.Name);
                _logger.LogInformation($"✅ Uploaded: {file.Name}");

                await Task.Delay(10000, stoppingToken);
            }

            _logger.LogInformation("🎉 All files processed. Exiting job.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Middleware execution failed: {ex.Message}");
        }
    }
}
