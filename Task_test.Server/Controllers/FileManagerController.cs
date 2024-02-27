using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

[Route("api")]
public class FileUploadController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FileUploadController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class FileUploadModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string UserEmail { get; set; }

        // Add any other properties you need for the model
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    [HttpPost("uploadBytes")]
    public async Task<IActionResult> UploadFileBytes([FromBody] FileUploadModel model)
    {
        // Validate file type
        if (model.FileBytes != null && model.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            // Validate email
            if (IsValidEmail(model.UserEmail))
            {
                // Get file extension
                string fileExtension = Path.GetExtension(model.FileName);

                // Upload file to Blob Storage
                var blobContainer = GetBlobContainer();
                var blobName = Guid.NewGuid().ToString() + fileExtension; // Use the same file extension
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);

                using (var stream = new MemoryStream(model.FileBytes))
                {
                    await blockBlob.UploadFromStreamAsync(stream);
                }

                // Return a success response
                return Ok(new { Message = "File uploaded successfully" });
            }
            else
            {
                ModelState.AddModelError("Email", "Invalid email address");
            }
        }
        else
        {
            ModelState.AddModelError("File", "Invalid file type. Please upload a .docx file");
        }

        // If validation fails, return BadRequest with error messages
        return BadRequest(ModelState);
    }

    private CloudBlobContainer GetBlobContainer()
    {
        var connectionString = _configuration.GetConnectionString("AzureBlobStorageConnection");
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var containerName = "file-storage"; // Replace with your actual container name
        var container = blobClient.GetContainerReference(containerName);

        return container;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
