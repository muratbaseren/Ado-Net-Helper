using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using AdoNetHelper.Abstract;

namespace AdoNetHelper
{
    /// <summary>
    /// Provides exporting utilities.
    /// </summary>
    public class Export : IExport
    {
        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to a PDF document.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated PDF file.</returns>
        public async Task<byte[]> ToPdfAsync(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();
                PdfPTable pdfTable = new PdfPTable(table.Columns.Count);

                // Header cells
                foreach (DataColumn column in table.Columns)
                {
                    pdfTable.AddCell(new Phrase(column.ColumnName));
                }

                // Data cells
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        pdfTable.AddCell(new Phrase(item?.ToString()));
                    }
                }

                document.Add(pdfTable);
                document.Close();
                writer.Close();

                return await Task.FromResult(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to an HTML document.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated HTML file.</returns>
        public async Task<byte[]> ToHtmlAsync(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            {
                await writer.WriteLineAsync("<html><body><table>");

                // Header
                await writer.WriteAsync("<tr>");
                foreach (DataColumn column in table.Columns)
                {
                    await writer.WriteAsync($"<th>{column.ColumnName}</th>");
                }
                await writer.WriteLineAsync("</tr>");

                // Rows
                foreach (DataRow row in table.Rows)
                {
                    await writer.WriteAsync("<tr>");
                    foreach (var item in row.ItemArray)
                    {
                        await writer.WriteAsync($"<td>{item}</td>");
                    }
                    await writer.WriteLineAsync("</tr>");
                }

                await writer.WriteLineAsync("</table></body></html>");
                await writer.FlushAsync();
                return memoryStream.ToArray();
            }
        }
    }
}
