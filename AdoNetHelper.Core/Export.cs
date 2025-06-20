using AdoNetHelper.Abstract;
using iText.Kernel.Pdf;
using iText.Layout;
using System.Data;

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
                using (var pdfWriter = new PdfWriter(memoryStream))
                using (var pdfDocument = new PdfDocument(pdfWriter))
                using (var document = new Document(pdfDocument))
                {
                    var pdfTable = new iText.Layout.Element.Table(table.Columns.Count);

                    // Add header
                    foreach (DataColumn column in table.Columns)
                    {
                        pdfTable.AddHeaderCell(column.ColumnName);
                    }

                    // Add rows
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (var item in row.ItemArray)
                        {
                            pdfTable.AddCell(item?.ToString() ?? string.Empty);
                        }
                    }

                    document.Add(pdfTable);
                    await Task.FromResult(() => document.Flush());
                    document.Close();
                }

                return memoryStream.ToArray();
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

        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to a CSV file.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated CSV file.</returns>
        public async Task<byte[]> ToCsvAsync(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            string Escape(string s)
            {
                if (s == null)
                {
                    return string.Empty;
                }
                if (s.Contains("\"") || s.Contains(",") || s.Contains("\n"))
                {
                    return "\"" + s.Replace("\"", "\"\"") + "\"";
                }
                return s;
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            {
                // Header
                string header = string.Join(",", table.Columns.Cast<DataColumn>().Select(c => Escape(c.ColumnName)));
                await writer.WriteLineAsync(header);

                // Rows
                foreach (DataRow row in table.Rows)
                {
                    string line = string.Join(",", row.ItemArray.Select(item => Escape(item?.ToString())));
                    await writer.WriteLineAsync(line);
                }

                await writer.FlushAsync();
                return memoryStream.ToArray();
            }
        }
    }
}
