using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdoNetHelper
{
    public partial class Database
    {
        public string ConnectionString { get; private set; }
        public SqlConnection Connection { get; private set; }
        public SqlCommand Command { get; private set; }


        public Database(string _connectionString)
        {
            ConnectionString = _connectionString;
            Connection = new SqlConnection(ConnectionString);
            Command = Connection.CreateCommand();
        }


        private SqlParameter[] ProcessParameters(params ParamItem[] parameters)
        {
            SqlParameter[] pars = parameters.Select(x => new SqlParameter()
            {
                ParameterName = x.ParamName,
                Value = x.ParamValue
            }).ToArray();

            return pars;
        }


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
    }
}
