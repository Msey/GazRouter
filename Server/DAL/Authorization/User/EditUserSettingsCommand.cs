using System.IO;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{

    public class EditUserSettingsCommand : CommandNonQuery<EditUserSettingsParameterSet>
    {
        public EditUserSettingsCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-20104", "Пользователь с таким именем и/или логином уже существует");
        }

        protected override void BindParameters(OracleCommand command, EditUserSettingsParameterSet parameters)
        {
            command.AddInputParameter("p_user_id", parameters.Id);

            var xmlSerializer = new XmlSerializer(typeof(UserSettings));
            using (var tw = new StringWriter())
            {
                xmlSerializer.Serialize(tw, parameters.SettingsUser);
                var settingXml = tw.ToString();
                command.AddInputParameter("p_settings", settingXml);

            }
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditUserSettingsParameterSet parameters)
        {
            return "P_USER.Edit_SETTING";
        }
    }

}