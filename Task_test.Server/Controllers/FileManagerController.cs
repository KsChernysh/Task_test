using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

[Route("filemanager")]
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
        public string userEmail { get; set; }

        
        public string file { get; set; }
       
    }

    [HttpPost("uploadBytes")]
    public async Task<IActionResult> UploadFileBytes([FromBody] FileUploadModel model)
    {
        // Validate file type
        if (model.file != null)// && model.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            // Validate email
            if (IsValidEmail(model.userEmail))
            {

                // Upload file to Blob Storage
                var blobContainer = GetBlobContainer();
                var blobName = $"{model.userEmail}_{Guid.NewGuid().ToString()}.docx";
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);
                blockBlob.Metadata.Add("email", model.userEmail);
                byte[] fileData = Convert.FromBase64String(model.file);

                using (var stream = new MemoryStream(fileData))
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
