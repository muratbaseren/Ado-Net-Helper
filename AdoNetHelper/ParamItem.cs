namespace AdoNetHelper
{
    /// <summary>
    /// Represents a SQL parameter item.
    /// </summary>
    public partial class ParamItem
    {
        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// Gets or sets parameter value.
        /// </summary>
        public object ParamValue { get; set; }
    }

    /// <summary>
    /// Represents a strongly typed SQL parameter item.
    /// </summary>
    /// <typeparam name="T">Type of the parameter value.</typeparam>
    public partial class ParamItem<T>
    {
        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// Gets or sets parameter value.
        /// </summary>
        public T ParamValue { get; set; }
    }
}
