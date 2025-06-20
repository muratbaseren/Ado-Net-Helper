using System.Data;
using System.Threading.Tasks;

namespace AdoNetHelper.Abstract
{
    /// <summary>
    /// Defines export functionality.
    /// </summary>
    public interface IExport
    {
        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to a PDF document.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated PDF file.</returns>
        Task<byte[]> ToPdfAsync(DataTable table);

        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to an HTML document.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated HTML file.</returns>
        Task<byte[]> ToHtmlAsync(DataTable table);

        /// <summary>
        /// Converts the supplied <see cref="DataTable"/> to a CSV file.
        /// </summary>
        /// <param name="table">Table containing data to be converted.</param>
        /// <returns>Byte array of the generated CSV file.</returns>
        Task<byte[]> ToCsvAsync(DataTable table);
    }
}
