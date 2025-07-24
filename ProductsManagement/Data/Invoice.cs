namespace ProductsManagement.Data
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string PdfPath { get; set; }

        public DateTime IssuedAt { get; set; }
    }
}
