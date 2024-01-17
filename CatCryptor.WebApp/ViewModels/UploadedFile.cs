namespace CatCryptor.WebApp.ViewModels;

/// <summary>
/// Represents an uploaded file along with an optional password for file-related operations.
/// </summary>
public class UploadedFile
{
    /// <summary>
    /// Gets or sets the uploaded file.
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Gets or sets the password associated with the uploaded file.
    /// </summary>
    public string? Password { get; set; }
}