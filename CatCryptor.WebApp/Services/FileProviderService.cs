using Microsoft.AspNetCore.Mvc;

namespace CatCryptor.WebApp.Services;

/// <remarks>
/// Service for managing file uploads and downloads. Initializes a new instance of the <see cref="FileProviderService"/> class.
/// </remarks>
/// <param name="configuration">The configuration used to retrieve the path for uploaded files.</param>
public class FileProviderService(IConfiguration configuration)
{
    private readonly string _uploadedFilesPath = configuration.GetValue<string>("UploadedFilesPath") ?? Path.Combine(Directory.GetCurrentDirectory(), "_uploads");
    private readonly long _maxFileWeight = configuration.GetValue<long>("MaxFileWeight");

    /// <summary>
    /// Uploads a file to the service.
    /// </summary>
    /// <param name="file">The file to be uploaded.</param>
    /// <returns>A task representing the asynchronous operation, returning the filename of the uploaded file.</returns>
    public async Task<string> UploadFile(IFormFile file)
    {
        if (file.Length > _maxFileWeight)
            throw new ArgumentOutOfRangeException("File size exceeds 100 megabytes");

        CheckUploadsDirectory();

        string fileName = Path.GetFileName(file.FileName);
        string filePath = Path.Combine(_uploadedFilesPath, fileName);

        using (FileStream fileStream = File.Create(filePath))
            await file.CopyToAsync(fileStream);

        return fileName;
    }

    /// <summary>
    /// Downloads a file with the specified filename.
    /// </summary>
    /// <param name="fileName">The name of the file to be downloaded.</param>
    /// <returns>A FileStreamResult for the downloaded file; null if the file is not found.</returns>
    public FileStreamResult? DownloadFile(string fileName)
    {
        string filePath = Path.Combine(_uploadedFilesPath, fileName);

        if (File.Exists(filePath))
        {
            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }

        return null;
    }

    private void CheckUploadsDirectory()
    {
        if (!Directory.Exists(_uploadedFilesPath))
            Directory.CreateDirectory(_uploadedFilesPath);
    }
}