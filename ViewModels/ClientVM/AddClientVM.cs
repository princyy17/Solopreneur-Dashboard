using System.ComponentModel.DataAnnotations;
using FreelanceManager.Enums;

namespace FreelanceManager.ViewModels.ClientVM
{
    public class AddClientVM
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MinLength(2, ErrorMessage = "Name must be greater than one charachters")]
        [Required(ErrorMessage = "You must enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter a Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter a Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "You must enter a Contact Person")]
        public string ContactPerson { get; set; }
        public string Address { get; set; }

        [Required]
        public ClientStatus Status { get; set; }
        public Currency CurrencyName { get; internal set; }
        public int ProjectId { get; internal set; }
        public double Hours { get; internal set; }
        public string? Notes { get; internal set; }
        public DateTime InvoiceDate { get; internal set; }
        public DateTime DueDate { get; internal set; }
    }
}
