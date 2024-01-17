namespace CatCryptor.WebApp.Services;

/// <summary>
/// Service responsible for cleaning up files based on their creation time.
/// </summary>
public class FileCleanupService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCleanupService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration used to retrieve the path for uploaded files.</param>
    public FileCleanupService(IConfiguration configuration)
    {
        string? uploadedFilesPath = configuration.GetValue<string>("UploadedFilesPath");

        if (uploadedFilesPath != null)
            _uploadedFilesPath = uploadedFilesPath ?? Path.Combine(Directory.GetCurrentDirectory(), "_uploads");
    }

    private readonly string _uploadedFilesPath = string.Empty;

    /// <summary>
    /// Cleans up files in the specified directory based on their creation time.
    /// Files created more than an hour ago will be deleted.
    /// </summary>
    public void CleanupFiles()
    {
        try
        {
            string[] files = Directory.GetFiles(_uploadedFilesPath);
            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);

                if (fileInfo.CreationTimeUtc < DateTime.UtcNow.AddHours(-1))
                    fileInfo.Delete();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}