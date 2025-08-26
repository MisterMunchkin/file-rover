using System.Text.Json;
using file_rover.user.config.dto;
using Microsoft.Extensions.Configuration;

namespace file_rover.user.config;

public class UserConfigService
{
    private readonly IConfiguration _configuration;
    public string SystemMessagePath { get; }
    public string UserConfigPath { get; }
    public string OrganiseNewFileSystemMessageContent { get; }
    public UserConfig UserConfig { get; private set; }
    public string WatchPath { get; set; }

    public UserConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
        SystemMessagePath = _configuration["SystemMessagePath"] ?? throw new InvalidOperationException("SystemMessagePath is not configured in appsettings.json");
        UserConfigPath = _configuration["UserConfigPath"] ?? throw new InvalidOperationException("UserConfigPath is not configured in appsettings.json");
        OrganiseNewFileSystemMessageContent = File.ReadAllText(Path.Combine(SystemMessagePath, "organise-new-file.mdc"));
        UserConfig = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(UserConfigPath)) ?? throw new InvalidOperationException("Deserialized user config is null");
        WatchPath = UserConfig.FileSystemWatcher?.WatchPath ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    public void ReloadUserConfig()
    {
        UserConfig = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(UserConfigPath)) ?? throw new InvalidOperationException("Deserialized user config is null");
        WatchPath = UserConfig.FileSystemWatcher?.WatchPath ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
}
