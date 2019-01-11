using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public abstract class CommandScalar<TParameters, TResult> : QueryBase<TParameters, TResult>
    {
        private readonly bool _execScalar;

        protected OracleParameter OutputParameter { get; set; }

        protected CommandScalar(ExecutionContext context, bool execScalar = false)
            : base(context)
        {
            _execScalar = execScalar;
        }

        public override TResult Execute(TParameters parameters)
        {
            string query = GetCommandTextAndLog(parameters);
            return SafeExecute(parameters, query, (comm, parameters1) =>
            {
                TResult result;
                BindParameters(comm, parameters1);
                if (_execScalar)
                {
                    result = (TResult) comm.ExecuteScalar();
                }
                else
                {

                    comm.ExecuteNonQuery();
                    result = GetResult();
                }
                return result;
            });
        }

        private TResult GetResult()
        {
            return OutputParameter.GetValue<TResult>();
        }

    }
}
