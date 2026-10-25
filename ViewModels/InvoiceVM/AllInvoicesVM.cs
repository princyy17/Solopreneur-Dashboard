using System.Data.SqlTypes;
using FreelanceManager.Enums;

namespace FreelanceManager.ViewModels.InvoiceVM
{
    public class AllInvoicesVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public Currency CurrencyName { get; set; }
        public decimal Amount { get; set; }
        public double Hours { get; set; }
        public double HourlyRate { get; set; }
        public bool IsDeleted { get; set; }

        public string? StatusColor { get; set; }

    }
}
