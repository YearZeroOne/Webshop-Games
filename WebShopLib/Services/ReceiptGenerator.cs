using System;
using System.IO;
using System.Text;
using iText.Html2pdf;
using iText.Kernel.Pdf;

namespace WebShopLib.Services;

public static class ReceiptGenerator
{
    /// <summary>
    /// Generates a PDF document from the given HTML content and saves it to the given destination path.
    /// </summary>
    /// <param name="htmlContent">The HTML content to generate the PDF from.</param>
    /// <param name="destinationPath">The path to save the generated PDF to.</param>
    /// <returns>The generated PDF document.</returns>
    public static PdfDocument GenerateReceiptPdf(string htmlContent, string destinationPath)
    {
        if (string.IsNullOrEmpty(htmlContent))
        {
            throw new ArgumentNullException(nameof(htmlContent));
        }

        if (string.IsNullOrEmpty(destinationPath))
        {
            throw new ArgumentNullException(nameof(destinationPath));
        }

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent));
        using var fileStream = new FileStream(destinationPath, FileMode.Create);
        using var pdfWriter = new PdfWriter(fileStream);
        using var pdfDocument = new PdfDocument(pdfWriter);
        return pdfDocument;
    }

    /// <summary>
    /// Generates an HTML receipt based on the given parameters.
    /// </summary>
    /// <param name="date">The date of the receipt.</param>
    /// <param name="customerName">The name of the customer.</param>
    /// <param name="items">A list of items purchased.</param>
    /// <returns>A string containing the HTML of the receipt.</returns>
    public static string GenerateReceiptHtml(string date, string customerName, List<(string Product, int Quantity, decimal Price)> items)
    {
        string path = Directory.GetCurrentDirectory();
        path = Path.GetFullPath(Path.Combine(path, @"..\"));
        path += @"WebShopLib\Services\ReceiptGenerator\ReceiptTemplate.html";
        var htmlTemplate = File.ReadAllText(path);
        var itemsHtml = new StringBuilder();

        foreach (var item in items)
        {
            var total = item.Quantity * item.Price;
            itemsHtml.Append($"<tr><td>{item.Product}</td><td>{item.Quantity}</td><td>{item.Price:C}</td><td>{total:C}</td></tr>");
        }

        var totalAmount = items.Sum(x => x.Quantity * x.Price);
        htmlTemplate = htmlTemplate.Replace("{{date}}", date)
                                   .Replace("{{customerName}}", customerName)
                                   .Replace("{{items}}", itemsHtml.ToString())
                                   .Replace("{{total}}", $"{totalAmount:C}");

        return htmlTemplate;
    }

}
