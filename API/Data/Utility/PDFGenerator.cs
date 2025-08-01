// InvoiceDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shared.DTOs;

public class PDFGenerator : IDocument
{
    private readonly string _customerName;
    private readonly string _orderId;
    private readonly List<PrintableOrderItem> _orderItems;
    private readonly string _shippingAddress;
    public PDFGenerator(string customerName, string orderId, List<PrintableOrderItem> orderItems, string shippingAddress)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        _customerName = customerName;
        _orderId = orderId;
        _orderItems = orderItems ?? new List<PrintableOrderItem>();
        _shippingAddress = shippingAddress;
    }

     public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    [Obsolete]
    public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Stack(stack =>
                    {
                        stack.Item().Text($"Invoice #{_orderId}").FontSize(20).Bold();
                    });

                    row.RelativeColumn().AlignRight().Stack(stack =>
                    {
                        stack.Item().Text("Shipping Address:").Bold();
                        stack.Item().Text(_shippingAddress).FontSize(10);
                    });
                });

                page.Content().Element(ComposeTable);

                page.Footer().AlignRight().Text(text =>
                {
                    var total = _orderItems.Sum(i => i.Quantity * i.UnitPrice);
                    text.Span($"Total: {total:C}").Bold().FontSize(14);
                });
            });
        }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3); // Product
                columns.RelativeColumn(1); // Qty
                columns.RelativeColumn(2); // Price
                columns.RelativeColumn(2); // Subtotal
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Text("Product").Bold();
                header.Cell().Text("Qty").Bold();
                header.Cell().Text("Unit Price").Bold();
                header.Cell().Text("Subtotal").Bold();
            });

            // Rows
            foreach (var item in _orderItems)
            {
                table.Cell().Text(item.ProductName);
                table.Cell().Text(item.Quantity.ToString());
                table.Cell().Text($"{item.UnitPrice:C}");
                table.Cell().Text($"{(item.Quantity * item.UnitPrice):C}");
            }
        });
    }
}
