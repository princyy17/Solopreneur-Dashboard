using System.ComponentModel.DataAnnotations.Schema;
using FreelanceManager.Enums;

namespace FreelanceManager.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public Currency CurrencyName { get; set; }
        public double Hours { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
    }
}
