using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{


    public class GetLoadedFilesCommand : QueryReader<GetLoadedFilesParam, List<LoadedFile>>
    {
        public GetLoadedFilesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetLoadedFilesParam param)
        {
            switch (param.LoadedFilesType)
            {
                case LoadedFilesType.Input:
                    return
                        @"select * from table(integro.p_md_loaddata.get_loaded_files)";
                case LoadedFilesType.Output:
                    return
                        @"select * from table(integro.p_md_loaddata.get_asdu_requests)";
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        protected override List<LoadedFile> GetResult(OracleDataReader reader, GetLoadedFilesParam param)
        {
            var result = new List<LoadedFile>();
            while (reader.Read())
            {
                var entry =
                    new LoadedFile
                    {
                        Key = reader.GetValue<string>("cid"),
                        FileName = reader.GetValue<string>("cfilename"),
                        LoadDate = reader.GetValue<DateTime>("dloaddate"),
                        Status = (LoadedFileStatus)reader.GetValue<int>("nstatus"),
                        Name = reader.GetValue<string>("cname")
                    };
                result.Add(entry);
            }
            return result;
        }
    }
}