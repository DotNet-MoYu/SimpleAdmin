namespace SimpleAdmin.Application;

public static class DocumentConst
{
    public const string ROOT_ANCESTOR = ",0,";
    public const string FILE_TYPE_FOLDER = "文件夹";
    public const string FILE_TYPE_DOCUMENT = "文档";
    public const string FILE_TYPE_IMAGE = "图片";
    public const string FILE_TYPE_ARCHIVE = "压缩包";
    public const string FILE_TYPE_APPLICATION = "应用程序";
    public const string FILE_TYPE_FILE = "文件";

    public static readonly string[] DOCUMENT_SUFFIXES = { "doc", "docx", "pdf", "xls", "xlsx", "ppt", "pptx", "txt", "md" };
    public static readonly string[] IMAGE_SUFFIXES = { "png", "jpg", "jpeg", "gif", "bmp", "webp" };
    public static readonly string[] ARCHIVE_SUFFIXES = { "zip", "rar", "7z", "tar", "gz" };
    public static readonly string[] APPLICATION_SUFFIXES = { "exe", "msi", "bat", "cmd", "com" };
    public static readonly string[] KNOWN_FILE_SUFFIXES = DOCUMENT_SUFFIXES.Concat(IMAGE_SUFFIXES).Concat(ARCHIVE_SUFFIXES).Concat(APPLICATION_SUFFIXES).Distinct().ToArray();

    public static string GetFileTypeLabel(DocumentNodeType nodeType, string suffix)
    {
        if (nodeType == DocumentNodeType.Folder)
            return FILE_TYPE_FOLDER;

        suffix = NormalizeSuffix(suffix);
        if (DOCUMENT_SUFFIXES.Contains(suffix))
            return FILE_TYPE_DOCUMENT;
        if (IMAGE_SUFFIXES.Contains(suffix))
            return FILE_TYPE_IMAGE;
        if (ARCHIVE_SUFFIXES.Contains(suffix))
            return FILE_TYPE_ARCHIVE;
        if (APPLICATION_SUFFIXES.Contains(suffix))
            return FILE_TYPE_APPLICATION;
        return FILE_TYPE_FILE;
    }

    public static string[] GetFileTypeSuffixes(string fileType)
    {
        return fileType switch
        {
            FILE_TYPE_DOCUMENT => DOCUMENT_SUFFIXES,
            FILE_TYPE_IMAGE => IMAGE_SUFFIXES,
            FILE_TYPE_ARCHIVE => ARCHIVE_SUFFIXES,
            FILE_TYPE_APPLICATION => APPLICATION_SUFFIXES,
            _ => Array.Empty<string>()
        };
    }

    public static string NormalizeSuffix(string suffix)
    {
        return suffix?.Trim().TrimStart('.').ToLowerInvariant() ?? string.Empty;
    }

    public static string ComposeRenamedFileName(string baseName, string suffix)
    {
        var normalizedName = baseName?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw Oops.Bah("名称不能为空");

        var normalizedSuffix = NormalizeSuffix(suffix);
        if (string.IsNullOrWhiteSpace(normalizedSuffix))
            return normalizedName;

        if (normalizedName.Contains('.'))
            throw Oops.Bah("文件名称不能包含扩展名，请只输入基础名称");

        return $"{normalizedName}.{normalizedSuffix}";
    }
}
