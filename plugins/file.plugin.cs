

// using Microsoft.SemanticKernel;

// public class FileTypeConfig {
//     public string renameStrategy;
//     public List<string> metadataFields;
// }

// public enum CaseType {
//     CamelCase,
//     SnakeCase,
//     KebabCase,
//     PascalCase
// }

// public class UserConfig {
//     public CaseType caseType = CaseType.KebabCase;
//     public Dictionary<string, FileTypeConfig> fileTypeConfig;
// }


// namespace file_rover.plugins
// {
//     public class FilePlugin
//     {
//         private const string PluginName = nameof(FilePlugin);
//         private readonly UserConfig TEST_USER_CONFIG = new UserConfig
//         {
//             fileTypeConfig = new Dictionary<string, FileTypeConfig>
//             {
//                 { ".png", new FileTypeConfig { renameStrategy = "metadata", metadataFields = new List<string> { "file_size", "resolution" } } },
//             }
//         }

//         [KernelFunction]
//         [Description("Renames a file based on its metadata, content, and user configuration.")]
//         public async Task<string> RenameFile(string filePath) {
//             // Plugin implementation goes here
//             var fileInfo = new FileInfo(filePath);
//             if (!fileInfo.Exists)
//             {
//                 throw new FileNotFoundException($"File not found: {filePath}");
//             }

//             // Return a dummy result for now
//             return await Task.FromResult("asdf");
//         }
//         // Plugin implementation goes here
//     }
// }