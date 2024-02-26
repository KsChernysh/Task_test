using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Task_test.Server;

[Route("api")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FileController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Required]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
    {
        try
        {
            // Validate email using DataAnnotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("Please provide a file.");
            }

            // Validate file type (example for .docx)
            if (!model.File.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid file type. Only .docx files are allowed.");
            }
            // Validate file size (example for 10MB)
            if (model.File.Length > 10 * 1024 * 1024)
            {
                return BadRequest("File size exceeds the limit of 10MB.");
            }

            // Create a blob container (replace 'webstorage011' with your storage account name)
            var storageAccount = CloudStorageAccount.Parse(_configuration["AzureBlobStorage:ConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("file-storage");
            await container.CreateIfNotExistsAsync();
            // Generate SAS token using a stored access policy
            var sasToken = GenerateSasTokenForContainer("file-storage", Email, SharedAccessBlobPermissions.Write);

            // Save the file to Azure Blob Storage using SAS token
            var containerUri = new Uri($"https://webstorage011.blob.core.windows.net/file-storage/{Email}/");
            var blobName = Guid.NewGuid().ToString() + ".docx";
            var blockBlob = new CloudBlockBlob(new Uri(containerUri, blobName), new StorageCredentials(sasToken));

            using (var stream = model.File.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }

            // Notify user or perform any additional actions (e.g., sending an email)
            SendEmail(Email, $"File {blobName} successfully uploaded. Access it using the following link: {blockBlob.Uri}");

            return Ok(new { Message = "File uploaded successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Helper method to generate SAS token for the container
    private string GenerateSasTokenForContainer(string containerName, string email, SharedAccessBlobPermissions permissions)
    {
        var container = new CloudBlobContainer(new Uri($"https://webstorage011.blob.core.windows.net/{containerName}/{email}"));

        // Create a stored access policy with the desired permissions
        var sharedAccessPolicy = new SharedAccessBlobPolicy
        {
            Permissions = permissions,
            SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1) // Set the desired expiration time
        };

        // Generate SAS token for the container using the stored access policy
        var sasToken = container.GetSharedAccessSignature(sharedAccessPolicy);

        return sasToken;
    }

    // Placeholder method for sending email (replace with actual implementation)
    private void SendEmail(string userEmail, string message)
    {
        try
        {
            var smtpClient = new SmtpClient("your-smtp-server.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com"),
                Subject = "File Upload Notification",
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(userEmail);

            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            // Handle exceptions (log or throw)
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}
