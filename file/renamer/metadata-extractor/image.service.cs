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
        { "Gif", typeof(GifHeaderDirectory) },
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
            var dir = directories.FirstOrDefault(d => supportedType.IsAssignableFrom(d.GetType()));
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

        var exifIfd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        var exifSubIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        if (exifIfd0 != null)
        {
            if (exifIfd0.ContainsTag(ExifDirectoryBase.TagMake))
                metadata["camera_make"] = exifIfd0.GetDescription(ExifDirectoryBase.TagMake) ?? "";
            if (exifIfd0.ContainsTag(ExifDirectoryBase.TagModel))
                metadata["camera_model"] = exifIfd0.GetDescription(ExifDirectoryBase.TagModel) ?? "";
            if (exifIfd0.ContainsTag(ExifDirectoryBase.TagOrientation))
                metadata["orientation"] = exifIfd0.GetDescription(ExifDirectoryBase.TagOrientation) ?? "";
            if (exifIfd0.ContainsTag(ExifDirectoryBase.TagImageWidth))
                metadata["width"] = exifIfd0.GetDescription(ExifDirectoryBase.TagImageWidth) ?? "";
            if (exifIfd0.ContainsTag(ExifDirectoryBase.TagImageHeight))
                metadata["height"] = exifIfd0.GetDescription(ExifDirectoryBase.TagImageHeight) ?? "";
        }

        if (exifSubIfd != null && exifSubIfd.ContainsTag(ExifDirectoryBase.TagColorSpace))
            metadata["color_space"] = exifSubIfd.GetDescription(ExifDirectoryBase.TagColorSpace) ?? "";
        

        return Task.FromResult(metadata);
    }

    private static void ExtractTiffMetadata(GeoTiffDirectory tiffDir, Dictionary<string, object> metadata)
    {
        if (tiffDir.ContainsTag(GeoTiffDirectory.TagGeographicType))
            metadata["geo_type"] = tiffDir.GetDescription(GeoTiffDirectory.TagGeographicType) ?? "";
        if (tiffDir.ContainsTag(GeoTiffDirectory.TagProjCenterLat))
            metadata["proj_center_lat"] = tiffDir.GetInt32(GeoTiffDirectory.TagProjCenterLat);
        if (tiffDir.ContainsTag(GeoTiffDirectory.TagProjCenterLong))
            metadata["proj_center_long"] = tiffDir.GetInt32(GeoTiffDirectory.TagProjCenterLong);
    }

    private static void ExtractBmpMetadata(BmpHeaderDirectory bmpDir, Dictionary<string, object> metadata)
    {
        if (bmpDir.ContainsTag(BmpHeaderDirectory.TagImageWidth))
            metadata["width"] = bmpDir.GetInt32(BmpHeaderDirectory.TagImageWidth);
        if (bmpDir.ContainsTag(BmpHeaderDirectory.TagImageHeight))
            metadata["height"] = bmpDir.GetInt32(BmpHeaderDirectory.TagImageHeight);
        if (bmpDir.ContainsTag(BmpHeaderDirectory.TagColorSpaceType))
            metadata["color_space"] = bmpDir.GetDescription(BmpHeaderDirectory.TagColorSpaceType) ?? "";
    }

    private static void ExtractGifMetadata(GifHeaderDirectory gifDir, Dictionary<string, object> metadata)
    {
        if (gifDir.ContainsTag(GifHeaderDirectory.TagImageWidth))
            metadata["width"] = gifDir.GetInt32(GifHeaderDirectory.TagImageWidth);
        if (gifDir.ContainsTag(GifHeaderDirectory.TagImageHeight))
            metadata["height"] = gifDir.GetInt32(GifHeaderDirectory.TagImageHeight);
    }

    private static void ExtractPngMetadata(PngDirectory pngDir, Dictionary<string, object> metadata)
    {
        if (pngDir.ContainsTag(PngDirectory.TagImageWidth))
            metadata["width"] = pngDir.GetInt32(PngDirectory.TagImageWidth);
        if (pngDir.ContainsTag(PngDirectory.TagImageHeight))
            metadata["height"] = pngDir.GetInt32(PngDirectory.TagImageHeight);
        if (pngDir.ContainsTag(PngDirectory.TagColorType))
            metadata["color_type"] = pngDir.GetDescription(PngDirectory.TagColorType) ?? "";
        if (pngDir.ContainsTag(PngDirectory.TagBackgroundColor))
            metadata["background_color"] = pngDir.GetDescription(PngDirectory.TagBackgroundColor) ?? "";
    }

    private static void ExtractJpegMetadata(JpegDirectory dir, Dictionary<string, object> metadata)
    {
        if (dir.ContainsTag(JpegDirectory.TagImageWidth))
            metadata["width"] = dir.GetInt32(JpegDirectory.TagImageWidth);
        if (dir.ContainsTag(JpegDirectory.TagImageHeight))
            metadata["height"] = dir.GetInt32(JpegDirectory.TagImageHeight);
    }
}