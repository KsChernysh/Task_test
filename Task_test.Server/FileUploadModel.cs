using System.ComponentModel.DataAnnotations;

namespace Task_test.Server
{
    public class FileUploadModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string UserEmail { get; set; }

        public string FileBytes { get; set; } // тепер тип рядка
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
