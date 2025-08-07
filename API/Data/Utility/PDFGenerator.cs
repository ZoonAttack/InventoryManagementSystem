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
            page.Margin(40);
            page.Size(PageSizes.A4);
            page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

            page.Header()
                .PaddingBottom(10)
                .Row(row =>
                {
                    row.RelativeColumn().Stack(stack =>
                    {
                        stack.Item().Text($"Invoice").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                        stack.Item().Text($"#{_orderId}").FontSize(14).SemiBold();
                    });

                    row.RelativeColumn().AlignRight().Stack(stack =>
                    {
                        stack.Item().Text("Shipping Address:").Bold();
                        stack.Item().Text(_shippingAddress).FontSize(10);
                    });
                });

            page.Content()
                .Padding(10)
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Column(column =>
                {
                    column.Item().PaddingBottom(5).Text($"Bill To: {_customerName}").SemiBold();

                    column.Item().Element(ComposeTable);

                    // ملخص المجموع
                    column.Item().AlignRight().PaddingTop(10).Stack(stack =>
                    {
                        var subtotal = _orderItems.Sum(i => i.Quantity * i.UnitPrice);
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn().AlignRight().Text("Total (USD):").Bold();
                            row.ConstantColumn(100).AlignRight().Text($"$ {subtotal:F2}").Bold();
                        });
                    });

                });

            page.Footer()
                .AlignCenter()
                .Text("Thank you for your business!")
                .FontSize(10)
                .Italic()
                .FontColor(Colors.Grey.Darken2);
        });
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4); 
                columns.RelativeColumn(1); 
                columns.RelativeColumn(2); 
                columns.RelativeColumn(2); 
            });

            table.Header(header =>
            {
                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Product").Bold();
                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignCenter().Text("Qty").Bold();
                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignRight().Text("Unit Price").Bold();
                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignRight().Text("Subtotal").Bold();
            });

            foreach (var item in _orderItems)
            {
                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).Text(item.ProductName);
                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignRight().Text($"$ {item.UnitPrice:F2}");
                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Padding(5).AlignRight().Text($"$ {(item.Quantity * item.UnitPrice):F2}");
            }

            table.Footer(footer =>
            {
                footer.Cell().ColumnSpan(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        });
    }



}
