using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasLeaks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasLeaks
{
    public class GetLeaksQuery : QueryReader<GetLeaksParameterSet, List<LeakDTO>>
    {
        public GetLeaksQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetLeaksParameterSet parameters)
        {
            return
                @"  SELECT      t1.Leak_Id
	                            ,t1.Leak_Number                 -- № п/п
                                ,t1.Leak_Place_Km 
	                            ,t1.Leak_Place  
	                            ,t1.Leak_Reason                 -- Причина
	                            ,t1.Volume_Day                  -- Объём в сутки, тыс. м³
                                ,t1.Repair_Activity             -- Мероприятия по устранению
	                            ,t1.Description                 -- Примечания 
	                            ,t1.Contact_Name                -- ФИО, должность ответственного лица
	                            ,t1.Create_Date                 -- Дата создания записи об утечке
	                            ,t1.Discovered_Date             -- Дата обнаружения
	                            ,t1.Repair_Plan_Date            -- Дата устранения  план
	                            ,t1.Repair_Plan_Fact            -- Дата устранения  фактическая
	                            ,t1.User_Name                   -- пользователь
	                            ,t1.Entity_Id                   -- ИД объекта
                                ,e.Entity_Name AS Name          -- Имя объекта
	                            ,se.Entity_name As Short_Path
	                            ,e.Entity_Type_Id
                                ,site.Site_Name 
	
                    FROM        rd.V_Leaks t1
                    LEFT JOIN   rd.V_Entities e on e.Entity_Id = t1.Entity_Id
                    LEFT JOIN   V_Nm_Short_All se on t1.Entity_Id = se.Entity_Id
                    LEFT JOIN   rd.V_SITES site on rd.P_ENTITY.GetSiteId(t1.Entity_Id, t1.Leak_Place_Km) = site.Site_Id
                    WHERE       (t1.Discovered_Date BETWEEN :p1 AND :p2)
                    ORDER BY    t1.Discovered_Date DESC
          ";
        }

        protected override void BindParameters(OracleCommand command, GetLeaksParameterSet parameters)
        {
            command.AddInputParameter("p1", parameters.StartDate);
            command.AddInputParameter("p2", parameters.EndDate);
        }

        protected override List<LeakDTO> GetResult(OracleDataReader reader, GetLeaksParameterSet parameters)
        {
            var leaks = new List<LeakDTO>();
            while (reader.Read())
            {
                var leak =
                    new LeakDTO
                    {
                        Id = reader.GetValue<int>("Leak_Id"),
                        Kilometer = reader.GetValue<double>("Leak_Place_Km"),
                        Place = reader.GetValue<string>("Leak_Place"),
                        Reason = reader.GetValue<string>("Leak_Reason"),
                        VolumeDay = reader.GetValue<double>("Volume_Day"),
                        RepairActivity = reader.GetValue<string>("Repair_Activity"),
                        Description = reader.GetValue<string>("Description"),
                        ContactName = reader.GetValue<string>("Contact_Name"),
                        CreateDate = reader.GetValue<DateTime>("Create_Date"),
                        DiscoverDate = reader.GetValue<DateTime>("Discovered_Date"),
                        EntityType = reader.GetValue<EntityType>("Entity_Type_Id"),
                        RepairPlanDate = reader.GetValue<DateTime?>("Repair_Plan_Date"),
                        UserName = reader.GetValue<string>("User_Name"),
                        EntityId = reader.GetValue<Guid>("Entity_Id"),
                        EntityName = reader.GetValue<string>("Name"),
                        EntityShortPath = reader.GetValue<string>("Short_Path"),
                        SiteName = reader.GetValue<string>("Site_Name"),
                        RepairPlanFact = reader.GetValue<DateTime?>("Repair_Plan_Fact")
                    };
                leaks.Add(leak);
            }
            return leaks;
        }
    }
}