
namespace file_rover.file.renamer.bo;

public sealed class FileRenamerImageBo : RenamerBo<FileRenamerImageBo>
{
    public FileRenamerImageBo(FileInfo fileInfo, string convention, Dictionary<string, string>? extractedMetadata = null) 
        : base(fileInfo, convention, extractedMetadata)
    {}

}