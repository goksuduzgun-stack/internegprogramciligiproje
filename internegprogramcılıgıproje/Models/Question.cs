using System.ComponentModel.DataAnnotations;

namespace internegprogramcılıgıproje.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Soru başlığı zorunludur.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soru içeriği zorunludur.")]
        public string Content { get; set; } = string.Empty;

        public int ViewCount { get; set; } = 0;

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Kategori İlişkisi
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        // Kullanıcı İlişkisi
        public int UserId { get; set; }
        public virtual AppUser? User { get; set; }

        // Soruya Verilen Cevaplar
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}