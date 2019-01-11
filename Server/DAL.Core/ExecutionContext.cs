using System;
using System.Data;
using GazRouter.Log;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Core
{
    public abstract class ExecutionContext : IDisposable
    {
        private readonly OracleConnection _connection;
        protected readonly Guid ContextId;
        private bool _needEndTransaction;
        private OracleTransaction _transaction;

        public MyLogger Logger { get; }
        public string AppHostName { get; private set; }

        protected ExecutionContext(string userIdentifier, string connectionString, string appHostName, MyLogger logger)
        {
            ContextId = Guid.NewGuid();

            AppHostName = appHostName;
            UserIdentifier = userIdentifier.ToLower();
            Logger = logger;

            _connection = new OracleConnection(connectionString);
            _connection.Open();
            Transaction = _connection.BeginTransaction();
            Logger.WriteContextAction(ContextId, "opened");
            _needEndTransaction = true;
            
        }

        public string UserIdentifier { get; private set; }

        protected internal OracleTransaction Transaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                _needEndTransaction = true;
            }
        }

        internal OracleConnection Connection
        {
            get { return _connection; }
        }

        internal OracleCommand CreateCommand(string commandText, bool isStoredProcedure)
        {
            return new OracleCommand(commandText, _connection)
            {
                BindByName = true,
                CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text
            };
        }

        protected virtual void EndTransaction()
        {
            Transaction.Dispose();
        }
        
        public void Dispose()
        {
            if (_needEndTransaction)
                EndTransaction();
            _connection.Close();
        }

        internal void Rollback()
        {
            Transaction.Rollback();
            Logger.WriteContextAction(ContextId, "rollbacked");
            _needEndTransaction = false;
        }
    }
}