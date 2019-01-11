using System;
using System.Collections.Generic;
using GazRouter.DAL.GasCosts;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.GasCosts.Import;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.GasCosts.Import;
namespace GazRouter.DataServices.GasCosts
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class GasCostsService : ServiceBase, IGasCostsService
    {
        private int GetSeriesId(int year, int month, int day, DTO.Dictionaries.PeriodTypes.PeriodType periodTypeId)
        {
            SeriesData.SeriesDataService sds = new SeriesData.SeriesDataService();
            return sds.AddSerie(new DTO.SeriesData.Series.AddSeriesParameterSet() { Year = year, Month = month, Day = day, PeriodTypeId = periodTypeId }).Id;
        }
        private int GetSeriesId(DateTime date, DTO.Dictionaries.PeriodTypes.PeriodType periodTypeId = DTO.Dictionaries.PeriodTypes.PeriodType.Day)
        {
            return GetSeriesId(date.Year, date.Month, date.Day, periodTypeId);
        }
        public List<GasCostDTO> GetGasCostList(GetGasCostListParameterSet parameter)
        {
            if (!parameter.PrdType.HasValue)
                parameter.PrdType = DTO.Dictionaries.PeriodTypes.PeriodType.Day;

            if ((parameter.SeriesId == null || parameter.SeriesId == -1) && !parameter.EndDate.HasValue)
            {
                parameter.SeriesId = GetSeriesId(parameter.StartDate.Value);
            }
         
            return ExecuteRead<GetGasCostListQuery, List<GasCostDTO>, GetGasCostListParameterSet>(parameter);
        }

        public List<DefaultParamValuesDTO> GetDefaultParamValues(GetGasCostListParameterSet parameter)
        {
            return ExecuteRead<GetDefaultParamValuesQuery, List<DefaultParamValuesDTO>, GetGasCostListParameterSet>(parameter);
        }

        public List<GasCostTypeDTO> GetCostTypeList()
        {
            return ExecuteRead<GetCostTypeListQuery, List<GasCostTypeDTO>>();
        }

        public int AddGasCost(AddGasCostParameterSet parameters)
        {
            try
            {
                if (parameters.SeriesId == -1)
                {
                    parameters.SeriesId = GetSeriesId(parameters.Date);
                }
                int cost = ExecuteRead<AddGasCostCommand, int, AddGasCostParameterSet>(parameters);
                return cost;
            }
            catch (Exception ex)
            {
                Console.Write( ex);
                throw;
            }
          
        }

        public void EditGasCost(EditGasCostParameterSet parameters)
        {
            if (parameters.SeriesId == -1)
            {
                parameters.SeriesId = GetSeriesId(parameters.Date);
            }
            ExecuteNonQuery<EditGasCostCommand, EditGasCostParameterSet>(parameters);
        }

        public void DeleteGasCost(int parameters)
        {
             ExecuteNonQuery<DeleteGasCostCommand,int>(parameters);
        }

        public void SetDefaultParamValues(List<DefaultParamValuesDTO> parameter)
        {
            using (var context = OpenDbContext())
            {
                var cmd = new SetDefaultParamValuesCommand(context);
                parameter.ForEach(cmd.Execute);
            }
        }

        public List<GasCostAccessDTO> GetGasCostAccessList(GetGasCostAccessListParameterSet parameter)
        {
            return ExecuteRead<GetGasCostAccessListQuery, List<GasCostAccessDTO>, GetGasCostAccessListParameterSet>(parameter);
        }

        public void UpdateGasCostAccessList(List<GasCostAccessDTO> parameters)
        {
            using (var context = OpenDbContext())
            {
                var cmd = new SetGasCostAccessCommand(context);
                foreach (var p in parameters)
                {
                    cmd.Execute(
                        new SetGasCostAccessParameterSet
                        {
                            Date = p.Date,
                            SiteId = p.SiteId,
                            Target = Target.Fact,
                            IsRestricted = !p.Fact,
                            PeriodType = p.PeriodType
                        });

                    cmd.Execute(
                        new SetGasCostAccessParameterSet
                        {
                            Date = p.Date,
                            SiteId = p.SiteId,
                            Target = Target.Plan,
                            IsRestricted = !p.Plan,
                            PeriodType = p.PeriodType
                        });

                    cmd.Execute(
                        new SetGasCostAccessParameterSet
                        {
                            Date = p.Date,
                            SiteId = p.SiteId,
                            Target = Target.Norm,
                            IsRestricted = !p.Norm,
                            PeriodType = p.PeriodType
                        });
                }
            }
        }

        public int AddGasCostImportInfo(AddGasCostImportInfoParameterSet parameters)
        {
            return ExecuteRead<AddGasCostImportInfoCommand, int, AddGasCostImportInfoParameterSet>(parameters);
        }

        public void DeleteGasCostImportInfo(int parameters)
        {
            ExecuteNonQuery<DeleteGasCostImportInfoCommand, int>(parameters);
        }

        public void UpdateGasCostsVisibility(UpdateGasCostsVisibilityParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                parameters.GasCostVisibility.ForEach(e => {
                    new AddGasCostVisibilityCommand(context).Execute(new AddGasCostVisibilityParameterSet
                    {
                        SiteId     = e.SiteId,
                        CostType   = e.CostType,
                        Visibility = e.Visibility
                    });
                });
            }
        }

        public void UpdateGasCostVisibility(AddGasCostVisibilityParameterSet parameters)
        {
            ExecuteNonQuery<AddGasCostVisibilityCommand, AddGasCostVisibilityParameterSet>(parameters);
        }
        public List<GasCostVisibilityDTO> GetGasCostsVisibility(Guid? siteId)
        {
            return ExecuteRead<GetGasCostVisibilityListQuery, List<GasCostVisibilityDTO>, Guid?> (siteId);
        }
    }
}