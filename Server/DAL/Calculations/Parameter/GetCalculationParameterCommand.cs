using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations.Parameter;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Parameter
{
    public class GetCalculationParameterCommand : CommandScalar<GetCalculationParameterParameterSet, int>
    {
        public GetCalculationParameterCommand(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, GetCalculationParameterParameterSet parameters)
        {
            OutputParameter = command.AddOutputParameter<int>("retval");
            command.AddInputParameter("p1", parameters.Alias);
            command.AddInputParameter("p2", parameters.SysName);
        }

        protected override string GetCommandText(GetCalculationParameterParameterSet parameters)
        {
            return @"
                    Begin
                    :retval :=  rd.P_PARAMETER.GetParameterID
					(          
                        p_alias_name       => :p1      
                        ,p_calculation_sn  => :p2
					);
                        end;";
        }
    }
}
