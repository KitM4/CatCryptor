using Serilog;
using Microsoft.AspNetCore.Mvc;
using CatCryptor.WebApp.Services;
using CatCryptor.WebApp.ViewModels;

namespace CatCryptor.WebApp.Controllers;

/// <summary>
/// Controller for handling file encryption and decryption operations.
/// </summary>
/// <param name="fileCryptor">The service for file encryption operations.</param>
/// <param name="fileProvider">The service for file provider operations.</param>
[ApiController]
public class CatCryptorController(FileCryptorService fileCryptor, FileProviderService fileProvider) : ControllerBase
{
    private readonly FileCryptorService _fileCryptor = fileCryptor;
    private readonly FileProviderService _fileProvider = fileProvider;

    /// <summary>
    /// Encrypts an uploaded file with the provided password.
    /// </summary>
    /// <param name="uploadedFile">The data of the uploaded file along with the password.</param>
    /// <returns>An encrypted file result or an error response.</returns>
    [HttpPost("encrypt")]
    [RequestSizeLimit(104_857_600)]
    public async Task<IActionResult> EncryptUploadedtFile([FromForm] UploadedFile uploadedFile)
    {
        try
        {
            if (uploadedFile.File == null || uploadedFile.File.Length == 0 || string.IsNullOrWhiteSpace(uploadedFile.Password))
            {
                Log.Error("The user sent empty data: {@uploadedFile}", uploadedFile);
                return BadRequest("No file uploaded");
            }

            string uploadedFileName = await _fileProvider.UploadFile(uploadedFile.File);
            string encryptedFileName = _fileCryptor.EncryptFile(uploadedFileName, uploadedFile.Password);

            FileResult? encryptedFile = _fileProvider.DownloadFile(encryptedFileName);
            if (encryptedFile == null)
            {
                Log.Error("File could not be encrypted: {@encryptedFile}", encryptedFile);
                return StatusCode(500, new { ErrorMessage = "File could not be encrypted" });
            }

            Log.Information("Uploading and encrypting the file was successful {@encryptedFile}", encryptedFile);

            return encryptedFile;
        }
        catch (Exception exception)
        {
            Log.Error("Error while working with EncrypUploadedtFile(): {@message}", exception.Message);
            return BadRequest(exception.Message);
        }
    }

    /// <summary>
    /// Decrypts an uploaded file with the provided password.
    /// </summary>
    /// <param name="uploadedFile">The data of the uploaded file along with the password.</param>
    /// <returns>A decrypted file result or an error response.</returns>
    [HttpPost("decrypt")]
    [RequestSizeLimit(104_857_600)]
    public async Task<IActionResult> DecryptUploadedFile([FromForm] UploadedFile uploadedFile)
    {
        try
        {
            if (uploadedFile.File == null || string.IsNullOrWhiteSpace(uploadedFile.Password))
            {
                Log.Error("The user sent empty data: {@uploadedFile}", uploadedFile);
                return BadRequest("No file uploaded");
            }

            string uploadedFileName = await _fileProvider.UploadFile(uploadedFile.File);
            string decryptedFileName = _fileCryptor.DecryptFile(uploadedFileName, uploadedFile.Password);

            FileResult? decryptedFile = _fileProvider.DownloadFile(decryptedFileName);
            if (decryptedFile == null)
            {
                Log.Error("File could not be decrypted: {@decryptedFile}", decryptedFile);
                return StatusCode(500, new { ErrorMessage = "File could not be decrypted" });
            }

            Log.Information("Uploading and decrypting the file was successful {@decryptedFile}", decryptedFile);

            return decryptedFile;
        }
        catch (Exception exception)
        {
            Log.Error("Error while working with EncrypUploadedtFile(): {@message}", exception.Message);
            return BadRequest(exception.Message);
        }
    }
}