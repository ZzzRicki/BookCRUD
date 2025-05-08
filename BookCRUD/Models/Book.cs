using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCRUD.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede tener más de 200 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, ErrorMessage = "El autor no puede tener más de 100 caracteres")]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Publicación")]
        public DateTime PublicationDate { get; set; }

        // Nuevos campos - todos opcionales
        [Display(Name = "Ruta del archivo")]
        public string? FilePath { get; set; }

        [Display(Name = "Contenido")]
        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        [Display(Name = "Formato")]
        public BookFormat Format { get; set; } = BookFormat.Text;

        [Display(Name = "URL de la portada")]
        public string? CoverImageUrl { get; set; }
    }

    public enum BookFormat
    {
        [Display(Name = "Texto")]
        Text,
        [Display(Name = "PDF")]
        PDF,
        [Display(Name = "EPUB")]
        EPUB,
        [Display(Name = "HTML")]
        HTML
    }
}