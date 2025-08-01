namespace ProductsManagement.Data
{
    public class Invoice
    {
        public int Id { get; set; }

        // Remove OrderId from here
        public string? PdfPath { get; set; }

        public DateTime IssuedAt { get; set; }

        // Navigation back (optional)
        public Order Order { get; set; }
    }

}
