using Xunit;
using CatCryptor.WebApp.Services;
using Microsoft.Extensions.Configuration;

namespace CatCryptor.Tests.ServicesTests;

public class FileCryptorServiceTests
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(
            new Dictionary<string, string?>()
            {
                { "UploadedFilesPath", "_uploads" }
            }).Build();

    [Fact]
    public void EncryptFile_Success()
    {
        // Arrange
        if (!Directory.Exists("_uploads"))
            Directory.CreateDirectory("_uploads");

        FileCryptorService fileCryptorService = new(_configuration);
        string fileName = "test.txt";
        string password = "securePassword";

        // Act
        string encryptedFileName = fileCryptorService.EncryptFile(fileName, password);

        // Assert
        Assert.Equal("test.txt.crpt", encryptedFileName);

        string? encryptedFilePath = Path.Combine("_uploads", encryptedFileName);
        File.Delete(encryptedFilePath);
    }

    [Fact]
    public void DecryptFile_Success()
    {
        // Arrange
        if (!Directory.Exists("_uploads"))
            Directory.CreateDirectory("_uploads");

        FileCryptorService fileCryptorService = new(_configuration);
        string fileName = "test.txt.crpt";
        string password = "securePassword";

        string encryptedFilePath = Path.Combine("_uploads", fileName);
        using (File.Create(encryptedFilePath)) { }

        // Act
        string decryptedFileName = fileCryptorService.DecryptFile(fileName, password);

        // Assert
        Assert.Equal("test.txt", decryptedFileName);

        File.Delete(encryptedFilePath);
    }
}