using System.Text;
using System.Security.Cryptography;

namespace CatCryptor.WebApp.Services;

/// <summary>
/// Service responsible for file encryption and decryption operations.
/// </summary>
public class FileCryptorService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCryptorService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration used to retrieve the path for uploaded files.</param>
    public FileCryptorService(IConfiguration configuration)
    {
        string? uploadedFilesPath = configuration.GetValue<string>("UploadedFilesPath");

        if (uploadedFilesPath != null)
            _uploadedFilesPath = uploadedFilesPath ?? Path.Combine(Directory.GetCurrentDirectory(), "_uploads");
    }

    private readonly string _uploadedFilesPath = string.Empty;

    /// <summary>
    /// Encrypts a file using AES encryption with the provided password.
    /// </summary>
    /// <param name="fileName">The name of the file to be encrypted.</param>
    /// <param name="password">The password used for encryption.</param>
    /// <returns>The filename of the encrypted file.</returns>
    public string EncryptFile(string fileName, string password)
    {
        Aes aes = Aes.Create();
        aes.Key = ComputeHash(password);

        byte[] iv = aes.IV;

        string filePath = Path.Combine(_uploadedFilesPath, fileName);
        string encryptedFilePath = $"{filePath}.crpt";

        using FileStream inputFileStream = new(filePath, FileMode.Open);
        using FileStream outputFileStream = new(encryptedFilePath, FileMode.Create);
        outputFileStream.Write(iv, 0, iv.Length);

        using CryptoStream cryptoStream = new(outputFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        inputFileStream.CopyTo(cryptoStream);

        return $"{fileName}.crpt";
    }

    /// <summary>
    /// Decrypts an encrypted file using AES decryption with the provided password.
    /// </summary>
    /// <param name="fileName">The name of the file to be decrypted.</param>
    /// <param name="password">The password used for decryption.</param>
    /// <returns>The filename of the decrypted file.</returns>
    public string DecryptFile(string fileName, string password)
    {
        Aes aes = Aes.Create();
        aes.Key = ComputeHash(password);

        byte[] iv = new byte[aes.IV.Length];

        string filePath = Path.Combine(_uploadedFilesPath, fileName);
        using FileStream inputFileStream = new(filePath, FileMode.Open);
        inputFileStream.Read(iv, 0, iv.Length);

        using CryptoStream decryptStream = new(inputFileStream, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Read);
        byte[] buffer = new byte[1024];

        string decryptedFilePath = filePath.Replace(".crpt", string.Empty);
        using FileStream outputFileStream = new(decryptedFilePath, FileMode.Create);
        int bytesRead;
        while ((bytesRead = decryptStream.Read(buffer, 0, buffer.Length)) > 0)
            outputFileStream.Write(buffer, 0, bytesRead);

        return fileName.Replace(".crpt", string.Empty);
    }

    private static byte[] ComputeHash(string data) =>
        new PasswordDeriveBytes(Encoding.UTF8.GetBytes(data), new byte[8], "SHA256", 1000).GetBytes(16);
}