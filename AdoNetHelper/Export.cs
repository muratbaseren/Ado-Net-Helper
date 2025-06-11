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
        /// <inheritdoc />
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
    }
}
