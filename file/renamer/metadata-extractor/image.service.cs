using MetadataExtractor;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.GeoTiff;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;

namespace file_rover.file.renamer.metadata_extractor;

public class FileRenamerMetadataExtractorImageService : FileRenamerMetadataExtractorBaseService
{
    public Dictionary<string, Type> SupportedDirectories { get; } = new()
    {
        { "Jpeg", typeof(JpegDirectory) },
        { "Png", typeof(PngDirectory) },
        { "Gif", typeof(GifImageDirectory) },
        { "Bmp", typeof(BmpHeaderDirectory) },
        { "GeoTiff",  typeof(GeoTiffDirectory) }
    };

    public override Dictionary<string, string> SupportedMetadataFields
    {
        get
        {
            base.SupportedMetadataFields.Add("camera_make", "Camera Make");
            base.SupportedMetadataFields.Add("camera_model", "Camera Model");
            base.SupportedMetadataFields.Add("orientation", "Orientation");
            base.SupportedMetadataFields.Add("width", "Width");
            base.SupportedMetadataFields.Add("height", "Height");
            base.SupportedMetadataFields.Add("color_type", "Color Type");
            base.SupportedMetadataFields.Add("background_color", "Background Color");
            base.SupportedMetadataFields.Add("color_space", "Color Space");
            base.SupportedMetadataFields.Add("geo_type", "Geo Type");
            base.SupportedMetadataFields.Add("proj_center_lat", "Projection Center Latitude");
            base.SupportedMetadataFields.Add("proj_center_long", "Projection Center Longitude");
            return base.SupportedMetadataFields;
        }
    }

//TODO: ExtractMetadata is buggy as hell.
    public override Task<Dictionary<string, object>> ExtractMetadata(string filePath)
    {
        var metadata = new Dictionary<string, object>();

        var fileInfo = new FileInfo(filePath) ??
            throw new FileNotFoundException($"File not found: {filePath}");

        metadata["file_size"] = fileInfo.Length;
        metadata["created_at"] = fileInfo.CreationTime;
        metadata["modified_at"] = fileInfo.LastWriteTime;

        var directories = ImageMetadataReader.ReadMetadata(filePath);

        // Try to extract metadata from list of supported directories
        foreach (var supportedDir in SupportedDirectories)
        {
            Type supportedType = supportedDir.Value;
            var dir = directories.FirstOrDefault(d => d.GetType() == supportedType);
            if (dir == null) continue;

            if (dir is JpegDirectory jpegDir)
                ExtractJpegMetadata(jpegDir, metadata);
            else if (dir is PngDirectory pngDir)
                ExtractPngMetadata(pngDir, metadata);
            else if (dir is GifHeaderDirectory gifDir)
                ExtractGifMetadata(gifDir, metadata);
            else if (dir is BmpHeaderDirectory bmpDir)
                ExtractBmpMetadata(bmpDir, metadata);
            else if (dir is GeoTiffDirectory tiffDir)
                ExtractTiffMetadata(tiffDir, metadata);
        }

        var exifDir = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (exifDir != null)
        {
            metadata["camera_make"] = exifDir.GetDescription(ExifDirectoryBase.TagMake) ?? "";
            metadata["camera_model"] = exifDir.GetDescription(ExifDirectoryBase.TagModel) ?? "";
            metadata["orientation"] = exifDir.GetDescription(ExifDirectoryBase.TagOrientation) ?? "";
            metadata["width"] = exifDir.GetDescription(ExifDirectoryBase.TagImageWidth) ?? "";
            metadata["height"] = exifDir.GetDescription(ExifDirectoryBase.TagImageHeight) ?? "";
        }

        return Task.FromResult(metadata);
    }

    private static void ExtractTiffMetadata(GeoTiffDirectory tiffDir, Dictionary<string, object> metadata)
    {
        metadata["geo_type"] = tiffDir.GetDescription(GeoTiffDirectory.TagGeographicType) ?? "";
        metadata["proj_center_lat"] = tiffDir.GetInt32(GeoTiffDirectory.TagProjCenterLat);
        metadata["proj_center_long"] = tiffDir.GetInt32(GeoTiffDirectory.TagProjCenterLong);
    }

    private static void ExtractBmpMetadata(BmpHeaderDirectory bmpDir, Dictionary<string, object> metadata)
    {
        metadata["width"] = bmpDir.GetInt32(BmpHeaderDirectory.TagImageWidth);
        metadata["height"] = bmpDir.GetInt32(BmpHeaderDirectory.TagImageHeight);
        metadata["color_space"] = bmpDir.GetDescription(BmpHeaderDirectory.TagColorSpaceType) ?? "";
    }

    private static void ExtractGifMetadata(GifHeaderDirectory gifDir, Dictionary<string, object> metadata)
    {
        metadata["width"] = gifDir.GetInt32(GifHeaderDirectory.TagImageWidth);
        metadata["height"] = gifDir.GetInt32(GifHeaderDirectory.TagImageHeight);
    }

    private static void ExtractPngMetadata(PngDirectory pngDir, Dictionary<string, object> metadata)
    {
        metadata["width"] = pngDir.GetInt32(PngDirectory.TagImageWidth);
        metadata["height"] = pngDir.GetInt32(PngDirectory.TagImageHeight);
        metadata["color_type"] = pngDir.GetDescription(PngDirectory.TagColorType) ?? "";
        metadata["background_color"] = pngDir.GetDescription(PngDirectory.TagBackgroundColor) ?? "";
    }

    private static void ExtractJpegMetadata(JpegDirectory dir, Dictionary<string, object> metadata)
    {
        metadata["width"] = dir.GetInt32(JpegDirectory.TagImageWidth);
        metadata["height"] = dir.GetInt32(JpegDirectory.TagImageHeight);
    }
}