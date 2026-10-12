using System.ComponentModel.DataAnnotations;

namespace internegprogramcılıgıproje.Models
{
    public class AnswerCreateViewModel
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Cevap metni boş bırakılamaz.")]
        [MinLength(5, ErrorMessage = "Cevabınız en az 5 karakter olmalıdır.")]
        [Display(Name = "Cevabınız")]
        public string Content { get; set; } = string.Empty;
    }
}