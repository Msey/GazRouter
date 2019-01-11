using System;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public class IntegrityConstraintException : Exception
    {
        public string ConstraintName { get; set; }
    
        
        public IntegrityConstraintException(string message, OracleException ex, string constraintName) : base(message, ex)
        {
            ConstraintName = constraintName;
        }
    }
}
