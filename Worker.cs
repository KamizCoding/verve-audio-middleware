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
        while (!stoppingToken.IsCancellationRequested)
        {
            var files = await _driveHelper.GetAudioFilesSortedAsync();

            foreach (var file in files)
            {
                var stream = await _driveHelper.DownloadFileAsync(file.Id);
                await _uploader.UploadAudioAsync(stream, file.Name);
                _logger.LogInformation($"Uploaded: {file.Name}");

                await Task.Delay(10000, stoppingToken); 
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
