using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.Folders
{

	public class GetFolderListQuery : QueryReader<int, List<FolderDTO>>
	{
		public GetFolderListQuery(ExecutionContext context)
			: base(context)
		{ }

		protected override string GetCommandText(int parameter)
		{
			return @"   SELECT      vf.folder_id,
						            vf.folder_name,
						            vf.sort_order,
						            vf.user_id,
						            usr.name AS user_name,
						            vf.parent_folder_id
					    FROM        v_folders vf 
                        JOIN        v_users usr ON usr.user_id = vf.user_id
                        WHERE       vf.user_id = :userid
                        ORDER BY    vf.folder_name";
		}

		protected override void BindParameters(OracleCommand command, int parameters)
		{
			command.AddInputParameter(":userid", parameters);
		}

        protected override List<FolderDTO> GetResult(OracleDataReader reader, int parameters)
		{
			var result = new List<FolderDTO>();
			while (reader.Read())
			{
                result.Add(new FolderDTO
				{
					Id = reader.GetValue<int>("folder_id"),
					Name = reader.GetValue<string>("folder_name"),
					ParentId = reader.GetValue<int?>("parent_folder_id"),
					CreatorUserId = reader.GetValue<string>("cre_user_id"),
					SortOrder = reader.GetValue<int>("sort_order")
				});
			}
			return result;
		}
	}
}
