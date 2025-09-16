using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;

namespace Data
{
    public abstract class Base : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Base));

        public IConfiguration Configuration { get; set; }

        protected DbConnection dbConnection { get; set; } = null;
        protected DbTransaction dbTransaction { get; set; } = null;
        protected DbDataReader dbDataReader { get; set; } = null;
        protected DbCommand dbCommand { get; set; } = null;
        private string Connection;

        protected string ConnectionName="Connection";

        #region Constructors

        protected Base()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            Connection = Configuration[$"ConnectionStrings:{ConnectionName}"] ?? string.Empty;
            try
            {
                this.dbConnection = new SqlConnection(Connection);
            }
            catch (Exception ex)
            {
                log.Error(" Constructor Error on: " + ConnectionName, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        public Base(DbConnection DbConnection, DbTransaction Transaction)
        {
            try
            {
                dbConnection = (DbConnection)DbConnection;
                dbTransaction = (DbTransaction)Transaction;
                Connection = dbTransaction.Connection.ConnectionString;
                dbConnection = dbTransaction.Connection;
            }
            catch (Exception ex)
            {
                log.Error(" Constructor ,  Transaction  Error  ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion Constructors

        #region Transactions

        public void OpenTransaction()
        {
            try
            {
                dbConnection.Open();
                dbTransaction = this.dbConnection.BeginTransaction(IsolationLevel.Serializable);
            }
            catch (Exception ex)
            {
                log.Error("OpenTransaction Error on Query: ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (this.dbTransaction != null)
                {
                    dbTransaction.Commit();
                    dbTransaction.Dispose();
                    dbTransaction = null;
                    this.DisposeConnection();
                }
            }
            catch (Exception ex)
            {
                log.Error("CommintTransaction Error on Query: ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (this.dbTransaction != null)
                {
                    dbTransaction.Rollback();
                    dbTransaction.Dispose();
                    dbTransaction = null;
                    this.DisposeConnection();
                }
            }
            catch (Exception ex)
            {
                log.Error("RollbackTransaction Error on Query: ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion Transactions

        #region Disposes

        public void Dispose()
        {
            DisposeConnection();
        }

        protected void DisposeConnection()
        {
            try
            {
                if ((this.dbConnection != null) && (this.dbTransaction == null))
                {
                    if (this.dbConnection.State != ConnectionState.Closed)
                        this.dbConnection.Close();

                    this.dbConnection.Dispose();
                    this.dbConnection = null;
                }
            }
            catch (Exception ex)
            {
                log.Error(" DisposeConnection  Error  ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        protected void DisposeReader(IDataReader drReader)
        {
            try
            {
                if (drReader != null)
                {
                    if (!drReader.IsClosed)
                        drReader.Close();

                    drReader.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.Error(" DisposeReader  Error  ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        protected void DisposeCommand()
        {
            if (this.dbCommand != null)
                this.dbCommand.Dispose();

            this.dbCommand = null;
        }

        #endregion Disposes

        #region Methods

        protected DbCommand GetQueryCommand(string query)
        {
            DbCommand QueryCommand = this.dbConnection.CreateCommand();
            QueryCommand.Connection = this.dbConnection;
            QueryCommand.CommandType = CommandType.Text;
            QueryCommand.CommandText = query;
            QueryCommand.Transaction = this.dbTransaction;
            return QueryCommand;
        }

        protected IDataReader GenericList(string query)
        {
            IDataReader Reader = null;
            DbCommand CommandList = null;
            try
            {
                CommandList = GetQueryCommand(query);
                Reader = CommandList.ExecuteReader();
            }
            catch (Exception ex)
            {
                log.Error("GenericList Error on Query: " + query, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return Reader;
        }

        protected List<T> SqlList<T>(string? query = null) where T : Entities.Base, new()
        {
            if (query is null)
            {
                var type = typeof(T);
                string tableName = $"{type.GetCustomAttribute<TableAttribute>()?.Schema}.{type.GetCustomAttribute<TableAttribute>()?.Name}" ;

                var columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetCustomAttribute<NotMappedAttribute>() == null)
                    .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name)
                    .ToArray();

                query = $"SELECT {string.Join(",", columns)} from {tableName}";
            }

            List<T> objCollection = new List<T>();
            IDataReader dataReader = null;
            try
            {
                dbConnection.Open();
                dataReader = GenericList(query);
                while (dataReader.Read())
                    objCollection.Add(this.Load<T>(dataReader));
            }
            catch (Exception ex)
            {
                log.Error("SqlList Error on Query: " + query, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                DisposeReader(dataReader);
                DisposeConnection();
            }
            return objCollection;
        }

        protected T SqlSearch<T>(string query) where T : Entities.Base, new()
        {
            IDataReader dataReader = null;
            T BEEntidad = null;
            try
            {
                dbConnection.Open();
                dataReader = GenericList(query);
                if (dataReader.Read())
                    BEEntidad = this.Load<T>(dataReader);
            }
            catch (Exception ex)
            {
                log.Error("SqlSearch Error on Query: " + query, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                DisposeReader(dataReader);
                DisposeConnection();
            }
            return BEEntidad;
        }

        protected DbParameter NewParameter(DbCommand Command, string Name, DbType Type, object Value, ParameterDirection Direction = ParameterDirection.Input)
        {
            DbParameter Parameter = Command.CreateParameter();

            Parameter.ParameterName = Name;
            Parameter.DbType = Type;
            Parameter.Direction = Direction;
            Parameter.Value = Value;

            return Parameter;
        }

        protected void AddInParameter(DbCommand Command, string ParameterName, DbType Type, object Value)
        {
            if (Command.CommandText.Contains(ParameterName))
                Command.Parameters.Add(NewParameter(Command, ParameterName, Type, Value));
        }

        protected object Value(string strQuery)
        {
            object objResult = null;
            DbCommand cmdCommand = GetQueryCommand(strQuery);

            try
            {
                dbConnection.Open();
                objResult = cmdCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error("SqlValue Error on Query: " + strQuery, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                DisposeConnection();
            }
            return objResult;
        }

        protected string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected T Load<T>(IDataReader dataReader) where T : Entities.Base, new()
        {
            if (dataReader == null) throw new ArgumentNullException(nameof(dataReader));

            var obj = new T();
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .ToList();

            var columnNames = Enumerable.Range(0, dataReader.FieldCount)
                                        .Select(dataReader.GetName)
                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in props)
            {
                if (!columnNames.Contains(prop.Name)) continue;

                var value = dataReader[prop.Name];
                if (value == DBNull.Value) continue;

                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                try
                {
                    object safeValue = Convert.ChangeType(value, targetType);
                    prop.SetValue(obj, safeValue);
                }
                catch (Exception ex)
                {
                    log.Error("Error loading ",ex);
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }

            return obj;

        }

        #endregion Methods

        #region PublicMethods

        public List<T> List<T>() where T : Entities.Base, new()
        {
            try
            {
                return SqlList<T>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion PublicMethods  
    }
}