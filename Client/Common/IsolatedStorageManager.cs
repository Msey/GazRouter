using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.Targets;
using Utils.Extensions;

namespace GazRouter.Common
{
    public static class IsolatedStorageManager
    {
        private static DictionaryRepositoryDTO _dictionaryRepository;

        
        public static SchemaDialogConfig SchemaValveStatesDialogConfig
        {
            get { return Get<SchemaDialogConfig>("SchemaValveStatesDialogConfig") ?? new SchemaDialogConfig(); }
            set { Set("SchemaValveStatesDialogConfig", value); }
        }

        public static SchemaDialogConfig SchemaModelTreeDialogContent
        {
            get { return Get<SchemaDialogConfig>("SchemaModelTreeDialogContent") ?? new SchemaDialogConfig(); }
            set { Set("SchemaModelTreeDialogContent", value); }
        }

        public static SchemaDialogConfig SchemaFindObjectDialogContent
        {
            get { return Get<SchemaDialogConfig>("SchemaFindObjectDialogContent") ?? new SchemaDialogConfig(); }
            set { Set("SchemaFindObjectDialogContent", value); }
        }


        public static Target GasCostsLastTarget
        {
            get { return Get<Target>("GasCostLastTarget"); }
            set { Set("GasCostLastTarget", value); }
        }



        public static string UserName
        {
            get { return Get<string>("UserName") ?? string.Empty; }
            set { Set("UserName",value); }
        }

        public static void Clear()
        {
            IsolatedStorageSettings.ApplicationSettings.Clear();
            IsolatedStorageSettings.ApplicationSettings.Save();
        }




        public static string LastModule
        {
            get { return Get<string>("LastModule"); }
            set { Set("LastModule", value); }
        }

        public static string LastView
        {
            get { return Get<string>("LastView"); }
            set { Set("LastView", value); }
        }

        public static bool AutoLogin
        {
            get
            {
                return Get<bool>("AutoLogin");
            }
            set
            {
                Set("AutoLogin", value);
            }
        }

		public static List<Guid> FavoritesList
		{
			get
			{
				var t1= Get<List<Guid>>("FavoritesList");

				return t1 ?? new List<Guid>();
			}
			set
			{
				Set("FavoritesList", value);
			}
		}

        public static DTO.Dictionaries.PipelineTypes.PipelineType[] PipelineTypesForPipelineConsumption
        {
            get
            {
                return Get<DTO.Dictionaries.PipelineTypes.PipelineType[]>("PipelineTypesForPipelineConsumption");
            }
            set
            {
                Set("PipelineTypesForPipelineConsumption", value);
            }
        }






        public static void Set<T>(string key, T value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key))
                settings[key] = value;
            else
            {
                settings.Add(key,value);
            }
            settings.Save();
        }

        public static T Get<T>(string key)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            T value;
            settings.TryGetValue(key, out value);
            return value;
        }

        public static SchemaParams LoadSchemaParams(int schemeId)
        {
            return Get<SchemaParams>($"schema_params_{schemeId}");
        }

        public static void SaveSchemaParams(int schemeId, SchemaParams schemaParams)
        {
            Set($"schema_params_{schemeId}", schemaParams);
        }
    }

    public class SchemaParams
    {
        public double Zoom { get; set; }
        public Point Position { get; set; }
    }

    public class SchemaDialogConfig
    {
        public SchemaDialogConfig()
        {
            Height = 500;
            Width = 600;
        }

        public double Height { get; set; }
        public double Width { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public bool IsOpen { get; set; }
    }
}