using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}