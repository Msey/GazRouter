using System.IO;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{

	public class AddUserCommand : CommandScalar<AddUserParameterSet, int>
	{
		public AddUserCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
			IntegrityConstraints.Add("ORA-20104", "Пользователь с таким именем и/или логином уже существует");

		}
		
	    protected override void BindParameters(OracleCommand command, AddUserParameterSet parameters)
	    {
            OutputParameter = command.AddReturnParameter<int>("user_id");
            command.AddInputParameter("p_login", parameters.Login.ToLower());
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_name", parameters.FullName);
            command.AddInputParameter("p_phone", parameters.Phone);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            
	        using (TextWriter tw = new StringWriter())
	        {
                var xmlSerializer = new XmlSerializer(typeof(UserSettings));
	            xmlSerializer.Serialize(tw, parameters.SettingsUser);
                command.AddInputParameter("p_settings", tw.ToString());
                tw.Close();
	        }
        }
		
		protected override string GetCommandText(AddUserParameterSet parameters)
	    {
            return "P_USER.AddF";
		}
	}

}

