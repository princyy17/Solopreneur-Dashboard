using System.ComponentModel.DataAnnotations;
using FreelanceManager.Enums;

namespace FreelanceManager.ViewModels.InvoiceVM
{
    public class AddInvoiceVM
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        [MinLength(2, ErrorMessage = "Name must be greater than one charachters")]
        [Required(ErrorMessage = "You must enter a name")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        [Required]
        public InvoiceStatus Status { get; set; }
        [Required]
        public Currency CurrencyName { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public double Hours { get; set; }
        [Required]
        public string? Notes { get; set; }
        [Required]
        public int ProjectId { get; set; }

        internal void Update(int id, Invoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}
