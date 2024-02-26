using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileService
{
    public class FileUploader
    {
        private readonly ILogger<FileUploader> _logger;

        public FileUploader(ILogger<FileUploader> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FileUploader))]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "webstorage011_STORAGE")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
