using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item;
using Microsoft.Graph.Models;
using System.Net;

public class OneDriveHelper
{
    private readonly GraphServiceClient _graphClient;
    private readonly string _driveId;
    private readonly string _folderPath;

    public OneDriveHelper(string clientId, string tenantId, string clientSecret, string driveId, string folderPath)
    {
        _driveId = driveId;
        _folderPath = folderPath;

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        _graphClient = new GraphServiceClient(credential);
    }

    public async Task<List<DriveItem>> GetAudioFilesSortedAsync()
    {
        var folder = await _graphClient
            .Drives[_driveId]
            .Root
            .ItemWithPath(_folderPath)
            .GetAsync();

        var children = await _graphClient
            .Drives[_driveId]
            .Items[folder?.Id]
            .Children
            .GetAsync();

        return children?.Value
            .Where(item => item.Name.EndsWith(".mp3") || item.Name.EndsWith(".wav") || item.Name.EndsWith(".webm"))
            .OrderBy(item => item.CreatedDateTime)
            .ToList() ?? new List<DriveItem>();
    }

    public async Task<Stream> DownloadFileAsync(string itemId)
    {
        return await _graphClient
            .Drives[_driveId]
            .Items[itemId]
            .Content
            .GetAsync();
    }
}
