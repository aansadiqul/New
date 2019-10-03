using System.ComponentModel.DataAnnotations;

namespace ABD.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}