using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.Integro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.Integro;
using System.Text.RegularExpressions;

namespace DataExchange.Integro.ASSPOOTI
{
    public class MappingHelper
    {
        public static string ToOracle(Guid guid)
        {
            return BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
        }
        public static async Task<ICollection<MappingEntityTreeItem>> LoadTreeItems(Guid? parentId, List<int> entityTypes, LockableViewModel viewModel)
        {
            //var mappingEntitiesDto = await new ObjectModelServiceProxy().GetMappingEntitiesListAsync(new GetMappingEntitiesParameterSet
            //{
            //    ParentId = parentId,
            //    EntityTypes = entityTypes
            //});
            //var treeItems = new List<MappingEntityTreeItem>();
            //foreach (var dto in mappingEntitiesDto)
            //{
            //    treeItems.Add(new MappingEntityTreeItem(viewModel)
            //    {
            //        Name = dto.Name,
            //        Id = dto.Id,
            //        Childrens = new List<MappingEntityTreeItem>() { new MappingEntityTreeItem(viewModel) { Name = "Foo" } }
            //    });
            //}
            var treeItems = new List<MappingEntityTreeItem>();
            return treeItems;
        }

        private static string ParseAsspootiParametrGid(string parametrGid)
        {
            //var index = NumIndexOf(parametrGid, ';', 4);            
            var param = parametrGid.Replace("\"", "");
            param = param.Replace("'", "").Trim();
            var lastIndex = param.LastIndexOf(';');
            var count = param.Count();
            if (lastIndex == count-1)
                param = param.Substring(0, lastIndex);
            //parametrGid = Regex.Replace(parametrGid, "['\"']", string.Empty).Trim();
            //var charsToRemove = new string[] { "'", "\""};
            //foreach (var c in charsToRemove)
            //{
            //    parametrGid = parametrGid.Replace(c, string.Empty);
            //}
            var elements = param.Split(';');
            var elementCount = elements.Count() > 4 ? 4 : elements.Count();
            if (elementCount < 3)
                return parametrGid;

            string result = string.Empty;
            if (elementCount == 3)
                result = $"'{GetFormat18(elements[0])}';'{elements[1]}';'{elements[2]}';";
            else
                result = $"'{elements[0]}';'{GetFormat18(elements[1])}';'{elements[2]}';'{elements[3]}';";

            //if (index > 0)
            //    result = parametrGid.Substring(0, index + 1);
            //else
            //    result = parametrGid;
            return result.Trim();
        }

        private static string GetFormat18(string str)
        {
            str = str + "                    ";
            return str.Substring(0, 18);
        }
        public static int NumIndexOf(string input, char value, int n)
        {
            int i = -1;
            do
            {
                i = input.IndexOf(value, i + 1);
                n--;
            }
            while (i != -1 && n > 0);
            return i;
        }

        public static List<AddEditSummaryPParameterSet> ParsingSummaryFile(FileStream fileStream, SummaryItem selectedSummary)
        {
            var result = new List<AddEditSummaryPParameterSet>();
            var resultDublicate = new List<AddEditSummaryPParameterSet>();
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                int i = 0;
                while (streamReader.Peek() >= 0)
                {
                    var line = streamReader.ReadLine();
                    var s = line.Split(new string[] {"\t" }, StringSplitOptions.None); // 0 - EntityId , 1 - PropertyTypeId, 2 - ParametrGid, 3 - Entity Name,
                    if (s.Length < 4)
                        continue;
                    Guid entityId; // 0 - EntityId
                    Guid.TryParse(s[0], out entityId);
                    if (entityId == null || Guid.Empty.Equals(entityId))
                        continue;
                    var summaryParameterId = Guid.NewGuid();
                    var newItem = new AddEditSummaryPParameterSet();
                    newItem.SummaryId = selectedSummary.Dto.Id;
                    newItem.SummaryParamId = (Guid)summaryParameterId;                    
                    int propertyTypeId;
                    int.TryParse(s[1], out propertyTypeId);
                    if (propertyTypeId <= 0 )
                        continue;
                    if (selectedSummary.Dto.SystemId == (int)MappingSourceType.ASSPOOTI)
                        newItem.ParametrGid = ParseAsspootiParametrGid(s[2]);
                    else
                        newItem.ParametrGid = s[2];
                    //
                    if (string.IsNullOrEmpty(newItem.ParametrGid))
                        continue;
                    var index = s[3].IndexOf('/');
                    var name = s[3].Substring(index+1, s[3].Length - index-1);
                    if (name.Length > 120)
                        name = name.Substring(0, 120);
                    var entityName = name;
                    newItem.Name = entityName;
                    newItem.Aggregate = string.Empty;
                    newItem.ContentList = new List<AddEditSummaryPContentParameterSet>()
                                    {
                        new AddEditSummaryPContentParameterSet()  {
                                        SummaryParamId = (Guid)summaryParameterId,
                                        EntityId = entityId.Convert(), // для базы
                                        EntityIdOriginal = entityId,
                                        PropertyTypeId = propertyTypeId
                                    }
                    };
                    if (result.Any(a => a.SummaryId == newItem.SummaryId &&
                                        a.ParametrGid == newItem.ParametrGid &&
                                        a.ContentList != null && a.ContentList.Any(c => c.EntityId == newItem.ContentList.First().EntityId &&
                                                                                        c.PropertyTypeId == newItem.ContentList.First().PropertyTypeId))
                                        )
                        resultDublicate.Add(newItem);
                    else
                        result.Add(newItem);
                }

            }
            return result;
        }

        public static List<MappingDescriptorDto> ParsingDescriptorFile(FileStream fileStream)
        {
            var result = new List<MappingDescriptorDto>();
            using (var streamReader = new StreamReader(fileStream,Encoding.UTF8))
            {
                int i = 0;
                while (streamReader.Peek() >= 0)
                {
                    var line = streamReader.ReadLine();
                    if (!line.Contains("'"))
                        continue;
                    var s = line.Split(new string[] { ";" }, StringSplitOptions.None);

                    var newItem = new MappingDescriptorDto();
                    newItem.Id = ++i;
                    newItem.FullLine = line;
                    newItem.Key = string.Format("{0};{1};{2};",s[0],s[1],s[2]);
                    newItem.Description = newItem.Key + s[3].Substring(0, s[3].IndexOf('='));
                    result.Add(newItem);
                }

            }
            return result;
        }
    }
}
