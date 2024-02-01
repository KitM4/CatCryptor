using Moq;
using Xunit;
using CatCryptor.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CatCryptor.Tests.ServicesTests;

public class FileProviderServiceTests
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            new Dictionary<string, string?>()
            {
                { "UploadedFilesPath", "_uploads" }
            }).Build();

    [Fact]
    public async Task UploadingFile_Success()
    {
        // Arrange
        Mock<IFormFile> fileMock = new();
        fileMock.Setup(x => x.FileName).Returns("test.txt");
        fileMock.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        FileProviderService fileProviderService = new(_configuration);

        // Act
        string fileName = await fileProviderService.UploadFile(fileMock.Object);

        // Assert
        Assert.Equal("test.txt", fileName);
    }

    [Fact]
    public void DownloadFile_ReturnsFileStreamResultWhenFileExists()
    {
        // Arrange
        FileProviderService fileProviderService = new(_configuration);
        string? uploadsPath = _configuration.GetValue<string>("UploadedFilesPath") ?? "_uploads";

        string fileName = "test.txt";
        string filePath = Path.Combine(uploadsPath, fileName);
        using (File.Create(filePath)) { }

        // Act
        FileStreamResult? result = fileProviderService.DownloadFile(fileName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("application/octet-stream", result.ContentType);
        Assert.Equal(fileName, result.FileDownloadName);

        result?.FileStream.Dispose();
        File.Delete(filePath);
    }

    [Fact]
    public void DownloadFile_ReturnsNullWhenFileDoesNotExist()
    {
        // Arrange
        FileProviderService fileProviderService = new(_configuration);
        string fileName = "nonexistent.txt";

        // Act
        FileStreamResult? result = fileProviderService.DownloadFile(fileName);

        // Assert
        Assert.Null(result);
    }
}