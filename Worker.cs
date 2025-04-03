public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OneDriveHelper _driveHelper;
    private readonly AudioUploader _uploader;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        _driveHelper = new OneDriveHelper(
            "64cff0cc-575b-49cf-b837-5f3c7fc23062",
            "da845803-2435-41d3-933a-8ea49062d091",
            Environment.GetEnvironmentVariable("GRAPH_CLIENT_SECRET"),
            "b!ZABt5L9HLUim0SeiXsmOZXZVgkcPbNhCrSaNJcGX2pOBCSYwMQaCQ43M8GScok8i", 
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
