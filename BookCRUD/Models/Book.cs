using System;
using System.ComponentModel.DataAnnotations;

namespace BookCRUD.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, ErrorMessage = "El autor no puede tener más de 100 caracteres")]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede tener más de 200 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Publicación")]
        public DateTime PublicationDate { get; set; }
    }
}