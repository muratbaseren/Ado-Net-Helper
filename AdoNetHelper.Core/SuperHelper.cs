using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace AdoNetHelper
{
    /// <summary>
    /// This is super ado helper class.
    /// </summary>
    public partial class SuperHelper
    {
        public string ConnectionString { get; private set; }

        public SuperHelper(string connectionString)
        {
            if (connectionString == null || string.IsNullOrEmpty(connectionString.Trim()) == true)
            {
                throw new Exception("ConnectionString can not be empty.");
            }
            else
            {
                ConnectionString = connectionString;
            }
        }

        /// <summary>
        /// This is a super method. It creates all Insert, Update, Delete, Select command and run query. Return required result value.
        /// </summary>
        /// <param name="queryType">Query type.</param>
        /// <param name="tableName">Table Name</param>
        /// <param name="columns">Column names.</param>
        /// <param name="parameters">Query values of parameters. For example; insert parameters or update parameters</param>
        /// <param name="whereParameters">Where condition values of parameters. You should use KeyValuePair, key is column name(don't use @ character)</param>
        /// <returns></returns>
        public object CreateAndRunQuery(QueryType queryType, string tableName, string[] columns, object[] parameters, params KeyValuePair<string, object>[] whereParameters)
        {
            SqlConnection baglanti = new SqlConnection(ConnectionString);
            SqlCommand komut = new SqlCommand();
            komut.Connection = baglanti;

            StringBuilder query = new StringBuilder(string.Empty);
            object result = null;

            Action<string[], object[]> parameterAddingAction =
                new Action<string[], object[]>((cols, pars) =>
                {
                    int counter = 0;

                    if (cols != null && cols.Length > 0 &&
                        pars != null && pars.Length > 0)
                    {
                        komut.Parameters.AddRange(
                           pars.Select(x =>
                           {
                               SqlParameter item = new SqlParameter($"@{cols[counter]}", x);
                               counter++;
                               return item;
                           }).ToArray());
                    }
                });


            switch (queryType)
            {
                case QueryType.Insert:
                    query.Append($"INSERT INTO {tableName}(");
                    query.Append(string.Join(",", columns));
                    query.Append(") VALUES (");
                    query.Append(string.Join(",", columns.Select(x => $"@{x} ")));
                    query.Append(")");

                    parameterAddingAction(columns, parameters);
                    break;
                case QueryType.Update:
                    query.Append($"UPDATE {tableName} SET ");
                    query.Append(string.Join(",", columns.Select(x => $"{x}=@{x} ")));

                    parameterAddingAction(columns, parameters);
                    break;
                case QueryType.Delete:
                    query.Append($"DELETE FROM {tableName} ");
                    break;
                case QueryType.Select:
                    query.Append($"SELECT ");
                    query.Append(string.Join(",", columns.Select(x => $"{x} ")));
                    query.Append($"FROM {tableName}");
                    break;
                default:
                    break;
            }

            if (whereParameters != null && whereParameters.Length > 0)
            {
                query.Append(" WHERE ");
                query.Append(string.Join(" AND ", whereParameters.Select(x => $"{x.Key}=@p_{x.Key}")));

                parameterAddingAction(
                    whereParameters.Select(x => $"p_{x.Key}").ToArray(),
                    whereParameters.Select(x => x.Value).ToArray());
            }

            komut.CommandText = query.ToString();

            switch (queryType)
            {
                case QueryType.Insert:
                case QueryType.Update:
                case QueryType.Delete:
                    baglanti.Open();
                    result = komut.ExecuteNonQuery();
                    baglanti.Close();
                    break;

                case QueryType.Select:
                    DataTable dt = new DataTable(tableName);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    adapter.Fill(dt);
                    result = dt;
                    adapter.Dispose();
                    break;

                default:
                    break;
            }

            komut.Dispose();
            baglanti.Dispose();

            return result;
        }

        /// <summary>
        /// Asynchronously creates and executes an Insert, Update, Delete or Select command.
        /// </summary>
        /// <param name="queryType">Query type.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="columns">Column names.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <param name="whereParameters">Parameters for WHERE clause.</param>
        /// <returns>Result object which can be affected row count or <see cref="DataTable"/>.</returns>
        public async Task<object> CreateAndRunQueryAsync(QueryType queryType, string tableName, string[] columns, object[] parameters, params KeyValuePair<string, object>[] whereParameters)
        {
            SqlConnection baglanti = new SqlConnection(ConnectionString);
            SqlCommand komut = new SqlCommand();
            komut.Connection = baglanti;

            StringBuilder query = new StringBuilder(string.Empty);
            object result = null;

            Action<string[], object[]> parameterAddingAction =
                new Action<string[], object[]>((cols, pars) =>
                {
                    int counter = 0;

                    if (cols != null && cols.Length > 0 &&
                        pars != null && pars.Length > 0)
                    {
                        komut.Parameters.AddRange(
                           pars.Select(x =>
                           {
                               SqlParameter item = new SqlParameter($"@{cols[counter]}", x);
                               counter++;
                               return item;
                           }).ToArray());
                    }
                });


            switch (queryType)
            {
                case QueryType.Insert:
                    query.Append($"INSERT INTO {tableName}(");
                    query.Append(string.Join(",", columns));
                    query.Append(") VALUES (");
                    query.Append(string.Join(",", columns.Select(x => $"@{x} ")));
                    query.Append(")");

                    parameterAddingAction(columns, parameters);
                    break;
                case QueryType.Update:
                    query.Append($"UPDATE {tableName} SET ");
                    query.Append(string.Join(",", columns.Select(x => $"{x}=@{x} ")));

                    parameterAddingAction(columns, parameters);
                    break;
                case QueryType.Delete:
                    query.Append($"DELETE FROM {tableName} ");
                    break;
                case QueryType.Select:
                    query.Append($"SELECT ");
                    query.Append(string.Join(",", columns.Select(x => $"{x} ")));
                    query.Append($"FROM {tableName}");
                    break;
                default:
                    break;
            }

            if (whereParameters != null && whereParameters.Length > 0)
            {
                query.Append(" WHERE ");
                query.Append(string.Join(" AND ", whereParameters.Select(x => $"{x.Key}=@p_{x.Key}")));

                parameterAddingAction(
                    whereParameters.Select(x => $"p_{x.Key}").ToArray(),
                    whereParameters.Select(x => x.Value).ToArray());
            }

            komut.CommandText = query.ToString();

            switch (queryType)
            {
                case QueryType.Insert:
                case QueryType.Update:
                case QueryType.Delete:
                    await baglanti.OpenAsync();
                    result = await komut.ExecuteNonQueryAsync();
                    baglanti.Close();
                    break;

                case QueryType.Select:
                    DataTable dt = new DataTable(tableName);
                    await baglanti.OpenAsync();
                    using (var reader = await komut.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                    baglanti.Close();
                    result = dt;
                    break;

                default:
                    break;
            }

            komut.Dispose();
            baglanti.Dispose();

            return result;
        }
    }

    /// <summary>
    /// Query type enumeration used by <see cref="SuperHelper"/>.
    /// </summary>
    public enum QueryType
    {
        /// <summary>Insert query.</summary>
        Insert = 0,
        /// <summary>Update query.</summary>
        Update = 1,
        /// <summary>Delete query.</summary>
        Delete = 2,
        /// <summary>Select query.</summary>
        Select = 3
    }
}
