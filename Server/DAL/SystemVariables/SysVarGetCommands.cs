using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.SystemVariables;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SystemVariables
{
    /// <summary>
    /// список системных переменных
    /// </summary>
    public class GetIusVariableListQuery : QueryReader<List<IusVariableDTO>>
    {
        public GetIusVariableListQuery(ExecutionContext context): base(context){ }

        protected override string GetCommandText()
        {
            return
                @"Select DESCRIPTION ,VALUE ,code as SYSNAME	
                    From  V_SYS_PARAMS
                    Where nvl( hidden, 0) = 0";
        }

        protected override List<IusVariableDTO> GetResult(OracleDataReader reader)
        {
            var variableList = new List<IusVariableDTO>();
            while (reader.Read())
            {
                var iusVar =
                    new IusVariableDTO
                        { 
                         
                        Description = reader.GetValue<string>("DESCRIPTION"),
                        Value = reader.GetValue<string>("VALUE"),
                        Name = reader.GetValue<string>("SYSNAME")
                    };
                variableList.Add(iusVar);
            }
            return variableList;
        }
    }

    /// <summary>
    /// значение системной переменной
    /// </summary>
    public class GetIusVariableValueQuery : QueryReader<string, string>
    {
        public GetIusVariableValueQuery(ExecutionContext context) : base(context){}


        protected override string GetCommandText(string parameters)
        {
            return
                @"Select 	VALUE
                    From  	V_SYS_PARAMS
                    Where   UPPER(code) = UPPER(:p1)";
        }

        protected override void BindParameters(OracleCommand command, string param)
        {
            command.AddInputParameter("p1", param);
        }
        protected override string GetResult(OracleDataReader reader, string param)
        {
            string outValue = String.Empty;
            if (reader.Read())
            {
                outValue = reader.GetValue<string>("VALUE");
            }
            return outValue;
        }
    }
}