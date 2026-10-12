using System.ComponentModel.DataAnnotations;

namespace internegprogramcılıgıproje.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cevap metni zorunludur.")]
        public string Content { get; set; } = string.Empty;

        public bool IsBestAnswer { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Soru İlişkisi
        public int QuestionId { get; set; }
        public virtual Question? Question { get; set; }

        // Kullanıcı İlişkisi
        public int UserId { get; set; }
        public virtual AppUser? User { get; set; }
    }
}