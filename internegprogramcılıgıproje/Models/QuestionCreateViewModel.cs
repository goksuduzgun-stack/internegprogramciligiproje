using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace internegprogramcılıgıproje.Models
{
    public class QuestionCreateViewModel
    {
        [Required(ErrorMessage = "Soru başlığı zorunludur.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5 ile 200 karakter arasında olmalıdır.")]
        [Display(Name = "Soru Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soru içeriği zorunludur.")]
        [MinLength(10, ErrorMessage = "Açıklama en az 10 karakter olmalıdır.")]
        [Display(Name = "Detaylı Açıklama")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
