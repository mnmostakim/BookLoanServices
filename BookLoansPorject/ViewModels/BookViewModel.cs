using BookLoansPorject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BookLoansPorject.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        [Required, StringLength(150), Display(Name = "Book Title")]
        public string Title { get; set; }
        [Required, StringLength(150), Display(Name = "Author Name")]
        public string Author { get; set; }
        [Required, StringLength(250), Display(Name = "Published In")]
        public string Published { get; set; }
        [Required, StringLength(450), Display(Name = "Short Description")]
        public string Description { get; set; }
        [Required, EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }
        [Required]
        public HttpPostedFileBase BookCover { get; set; }
        public bool IsAvailable { get; set; }
        public virtual List<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
    }
}