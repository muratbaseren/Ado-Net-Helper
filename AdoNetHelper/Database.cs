using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AdoNetHelper
{
    /// <summary>
    /// Provides basic database helper methods.
    /// </summary>
    public partial class Database
    {
        /// <summary>
        /// Gets the connection string used by this instance.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the SQL connection object.
        /// </summary>
        public SqlConnection Connection { get; private set; }

        /// <summary>
        /// Gets the SQL command object.
        /// </summary>
        public SqlCommand Command { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class using the specified connection string.
        /// </summary>
        /// <param name="_connectionString">Connection string for the database.</param>
        public Database(string _connectionString)
        {
            ConnectionString = _connectionString;
            Connection = new SqlConnection(ConnectionString);
            Command = Connection.CreateCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class using individual connection information.
        /// </summary>
        /// <param name="server">Server name.</param>
        /// <param name="database">Database name.</param>
        /// <param name="userId">User id.</param>
        /// <param name="password">Password.</param>
        public Database(string server, string database, string userId, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                UserID = userId,
                Password = password
            };

            ConnectionString = builder.ConnectionString;
            Connection = new SqlConnection(ConnectionString);
            Command = Connection.CreateCommand();
        }


        /// <summary>
        /// Converts <see cref="ParamItem"/> objects to <see cref="SqlParameter"/> instances.
        /// </summary>
        /// <param name="parameters">Parameter collection.</param>
        /// <returns>Array of SQL parameters.</returns>
        private SqlParameter[] ProcessParameters(params ParamItem[] parameters)
        {
            SqlParameter[] pars = parameters.Select(x => new SqlParameter()
            {
                ParameterName = x.ParamName,
                Value = x.ParamValue
            }).ToArray();

            return pars;
        }


        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="query">SQL query text.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <returns>Affected row count.</returns>
        public virtual int RunQuery(string query, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = query;
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            int result = 0;

            Connection.Open();
            result = Command.ExecuteNonQuery();
            Connection.Close();

            return result;
        }


        /// <summary>
        /// Executes a stored procedure and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="procName">Stored procedure name.</param>
        /// <param name="parameters">Procedure parameters.</param>
        /// <returns>Result table.</returns>
        public virtual DataTable RunProc(string procName, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = procName;
            Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(dt);

            return dt;
        }

        /// <summary>
        /// Executes a table valued function and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="functionName">Function name.</param>
        /// <param name="parameters">Function parameters.</param>
        /// <returns>Result table.</returns>
        public virtual DataTable RunFunction(string functionName, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();

            string[] paramNames = parameters != null && parameters.Length > 0
                ? parameters.Select(x => x.ParamName).ToArray()
                : new string[0];

            Command.CommandText = $"SELECT * FROM {functionName}({string.Join(",", paramNames)})";
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(dt);

            return dt;
        }


        /// <summary>
        /// Executes a SELECT command and returns the results in a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="query">SQL query text.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <returns>Result table.</returns>
        public virtual DataTable GetTable(string query, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = query;
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            SqlDataAdapter da = new SqlDataAdapter(Command);

            // Adaptor : otomatik bağlantı açar. Verileri çeker(sorguyu çalıştırır) ve bir datatable 'a doldurur ve bağlantıyı otomatik kapatır.

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        /// <summary>
        /// Creates a backup file for the specified database.
        /// </summary>
        /// <param name="databaseName">Name of the database to backup.</param>
        /// <param name="filePath">Full path of the backup file.</param>
        public virtual void Backup(string databaseName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            Command.Parameters.Clear();
            Command.CommandText = $"BACKUP DATABASE [{databaseName}] TO DISK = @filePath";
            Command.CommandType = CommandType.Text;
            Command.Parameters.AddWithValue("@filePath", filePath);

            Connection.Open();
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        /// <summary>
        /// Restores the specified database from a backup file.
        /// </summary>
        /// <param name="databaseName">Name of the database to restore.</param>
        /// <param name="filePath">Path of the backup file.</param>
        public virtual void Restore(string databaseName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            Command.Parameters.Clear();
            Command.CommandText = $"RESTORE DATABASE [{databaseName}] FROM DISK = @filePath WITH REPLACE";
            Command.CommandType = CommandType.Text;
            Command.Parameters.AddWithValue("@filePath", filePath);

            Connection.Open();
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        /// <summary>
        /// Asynchronously executes a non-query SQL command.
        /// </summary>
        /// <param name="query">SQL query text.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <returns>Affected row count.</returns>
        public virtual async Task<int> RunQueryAsync(string query, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = query;
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            int result = 0;

            await Connection.OpenAsync();
            result = await Command.ExecuteNonQueryAsync();
            Connection.Close();

            return result;
        }

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="procName">Stored procedure name.</param>
        /// <param name="parameters">Procedure parameters.</param>
        /// <returns>Result table.</returns>
        public virtual async Task<DataTable> RunProcAsync(string procName, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = procName;
            Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            DataTable dt = new DataTable();

            await Connection.OpenAsync();
            using (var reader = await Command.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            Connection.Close();

            return dt;
        }

        /// <summary>
        /// Asynchronously executes a table valued function and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="functionName">Function name.</param>
        /// <param name="parameters">Function parameters.</param>
        /// <returns>Result table.</returns>
        public virtual async Task<DataTable> RunFunctionAsync(string functionName, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();

            string[] paramNames = parameters != null && parameters.Length > 0
                ? parameters.Select(x => x.ParamName).ToArray()
                : new string[0];

            Command.CommandText = $"SELECT * FROM {functionName}({string.Join(",", paramNames)})";
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            DataTable dt = new DataTable();

            await Connection.OpenAsync();
            using (var reader = await Command.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            Connection.Close();

            return dt;
        }

        /// <summary>
        /// Asynchronously executes a SELECT command and returns the results as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="query">SQL query text.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <returns>Result table.</returns>
        public virtual async Task<DataTable> GetTableAsync(string query, params ParamItem[] parameters)
        {
            Command.Parameters.Clear();
            Command.CommandText = query;
            Command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Length > 0)
            {
                Command.Parameters.AddRange(ProcessParameters(parameters));
            }

            DataTable dt = new DataTable();

            await Connection.OpenAsync();
            using (var reader = await Command.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            Connection.Close();

            return dt;
        }

        /// <summary>
        /// Creates an empty copy of the specified table.
        /// </summary>
        /// <param name="sourceTable">Source table name.</param>
        /// <param name="newTable">Name of the new table.</param>
        public virtual async Task CloneTableStructureAsync(string sourceTable, string newTable)
        {
            if (string.IsNullOrWhiteSpace(sourceTable))
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }

            if (string.IsNullOrWhiteSpace(newTable))
            {
                throw new ArgumentNullException(nameof(newTable));
            }

            Command.Parameters.Clear();
            Command.CommandText = $"SELECT * INTO [{newTable}] FROM [{sourceTable}] WHERE 1 = 0";
            Command.CommandType = CommandType.Text;

            await Connection.OpenAsync();
            await Command.ExecuteNonQueryAsync();
            Connection.Close();
        }

        /// <summary>
        /// Creates a copy of the specified table including its data.
        /// </summary>
        /// <param name="sourceTable">Source table name.</param>
        /// <param name="newTable">Name of the new table.</param>
        public virtual async Task CloneTableWithDataAsync(string sourceTable, string newTable)
        {
            if (string.IsNullOrWhiteSpace(sourceTable))
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }

            if (string.IsNullOrWhiteSpace(newTable))
            {
                throw new ArgumentNullException(nameof(newTable));
            }

            Command.Parameters.Clear();
            Command.CommandText = $"SELECT * INTO [{newTable}] FROM [{sourceTable}]";
            Command.CommandType = CommandType.Text;

            await Connection.OpenAsync();
            await Command.ExecuteNonQueryAsync();
            Connection.Close();
        }
    }
}
