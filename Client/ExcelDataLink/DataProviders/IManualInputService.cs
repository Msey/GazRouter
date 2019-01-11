using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.ManualInput;
using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ManualInput.InputStory;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.ManualInput  
{
    [ServiceContract]
    public interface IManualInputService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetInputStateList(GetManualInputStateListParameterSet parameters, AsyncCallback callback, object state);
        List<ManualInputStateDTO> EndGetInputStateList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetInputState(SetManualInputStateParameterSet parameters, AsyncCallback callback, object state);
        void EndSetInputState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetInputStory(GetManualInputStoryParameterSet parameters, AsyncCallback callback, object state);
        List<ManualInputStoryDTO> EndGetInputStory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSiteTree(GetSiteTreeParameterSet parameters, AsyncCallback callback, object state);
        SerializableTuple<TreeDataDTO, Dictionary<Guid, EntityValidationStatus>> EndGetSiteTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetValveSwitchList(GetValveSwitchListParameterSet parameters, AsyncCallback callback, object state);
        List<ValveSwitchDTO> EndGetValveSwitchList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddValveSwitch(AddValveSwitchParameterSet parameters, AsyncCallback callback, object state);
        void EndAddValveSwitch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteValveSwitch(DeleteValveSwitchParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteValveSwitch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitStateList(GetCompUnitStateListParameterSet parameters, AsyncCallback callback, object state);
        List<CompUnitStateDTO> EndGetCompUnitStateList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompUnitState(AddCompUnitStateParameterSet parameters, AsyncCallback callback, object state);
        int EndAddCompUnitState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCompUnitState(EditCompUnitStateParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCompUnitState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters, AsyncCallback callback, object state);
        void EndAddFailureRelatedCompUnitStart(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteFailureRelatedCompUnitStart(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFailureAttachment(AddAttachmentParameterSet<int> parameters, AsyncCallback callback, object state);
        int EndAddFailureAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteFailureAttachment(int parameters, AsyncCallback callback, object state);
        void EndDeleteFailureAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteCompUnitState(int parameters, AsyncCallback callback, object state);
        void EndDeleteCompUnitState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitFailureList(GetFailureListParameterSet parameters, AsyncCallback callback, object state);
        List<CompUnitStateDTO> EndGetCompUnitFailureList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetChemicalTestList(GetChemicalTestListParameterSet parameters, AsyncCallback callback, object state);
        List<ChemicalTestDTO> EndGetChemicalTestList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddChemicalTest(AddChemicalTestParameterSet parameters, AsyncCallback callback, object state);
        int EndAddChemicalTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditChemicalTest(EditChemicalTestParameterSet parameters, AsyncCallback callback, object state);
        void EndEditChemicalTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteChemicalTest(int parameters, AsyncCallback callback, object state);
        void EndDeleteChemicalTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitTestList(GetCompUnitTestListParameterSet parameters, AsyncCallback callback, object state);
        List<CompUnitTestDTO> EndGetCompUnitTestList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompUnitTest(AddCompUnitTestParameterSet parameters, AsyncCallback callback, object state);
        int EndAddCompUnitTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCompUnitTest(EditCompUnitTestParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCompUnitTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteCompUnitTest(int parameters, AsyncCallback callback, object state);
        void EndDeleteCompUnitTest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompUnitTestAttachment(AddAttachmentParameterSet<int> parameters, AsyncCallback callback, object state);
        int EndAddCompUnitTestAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveCompUnitTestAttachment(int parameters, AsyncCallback callback, object state);
        void EndRemoveCompUnitTestAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompUnitTestPoint(AddCompUnitTestPointParameterSet parameters, AsyncCallback callback, object state);
        void EndAddCompUnitTestPoint(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveCompUnitTestPoint(DeleteCompUnitTestPointParameterSet parameters, AsyncCallback callback, object state);
        void EndRemoveCompUnitTestPoint(IAsyncResult result);
    }


    public class ManualInputServiceProxy : DataProviderBase<IManualInputService>
	{
        protected override string ServiceUri
        {
            get { return "/ManualInput/ManualInputService.svc"; }
        }

        public Task<List<ManualInputStateDTO>> GetInputStateListAsync(GetManualInputStateListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ManualInputStateDTO>,GetManualInputStateListParameterSet>(channel, channel.BeginGetInputStateList, channel.EndGetInputStateList, parameters);
        }

        public Task SetInputStateAsync(SetManualInputStateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetInputState, channel.EndSetInputState, parameters);
        }

        public Task<List<ManualInputStoryDTO>> GetInputStoryAsync(GetManualInputStoryParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ManualInputStoryDTO>,GetManualInputStoryParameterSet>(channel, channel.BeginGetInputStory, channel.EndGetInputStory, parameters);
        }

        public Task<SerializableTuple<TreeDataDTO, Dictionary<Guid, EntityValidationStatus>>> GetSiteTreeAsync(GetSiteTreeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SerializableTuple<TreeDataDTO, Dictionary<Guid, EntityValidationStatus>>,GetSiteTreeParameterSet>(channel, channel.BeginGetSiteTree, channel.EndGetSiteTree, parameters);
        }

        public Task<List<ValveSwitchDTO>> GetValveSwitchListAsync(GetValveSwitchListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ValveSwitchDTO>,GetValveSwitchListParameterSet>(channel, channel.BeginGetValveSwitchList, channel.EndGetValveSwitchList, parameters);
        }

        public Task AddValveSwitchAsync(AddValveSwitchParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddValveSwitch, channel.EndAddValveSwitch, parameters);
        }

        public Task DeleteValveSwitchAsync(DeleteValveSwitchParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteValveSwitch, channel.EndDeleteValveSwitch, parameters);
        }

        public Task<List<CompUnitStateDTO>> GetCompUnitStateListAsync(GetCompUnitStateListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompUnitStateDTO>,GetCompUnitStateListParameterSet>(channel, channel.BeginGetCompUnitStateList, channel.EndGetCompUnitStateList, parameters);
        }

        public Task<int> AddCompUnitStateAsync(AddCompUnitStateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddCompUnitStateParameterSet>(channel, channel.BeginAddCompUnitState, channel.EndAddCompUnitState, parameters);
        }

        public Task EditCompUnitStateAsync(EditCompUnitStateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCompUnitState, channel.EndEditCompUnitState, parameters);
        }

        public Task AddFailureRelatedCompUnitStartAsync(AddFailureRelatedUnitStartParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddFailureRelatedCompUnitStart, channel.EndAddFailureRelatedCompUnitStart, parameters);
        }

        public Task DeleteFailureRelatedCompUnitStartAsync(AddFailureRelatedUnitStartParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteFailureRelatedCompUnitStart, channel.EndDeleteFailureRelatedCompUnitStart, parameters);
        }

        public Task<int> AddFailureAttachmentAsync(AddAttachmentParameterSet<int> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddAttachmentParameterSet<int>>(channel, channel.BeginAddFailureAttachment, channel.EndAddFailureAttachment, parameters);
        }

        public Task DeleteFailureAttachmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteFailureAttachment, channel.EndDeleteFailureAttachment, parameters);
        }

        public Task DeleteCompUnitStateAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteCompUnitState, channel.EndDeleteCompUnitState, parameters);
        }

        public Task<List<CompUnitStateDTO>> GetCompUnitFailureListAsync(GetFailureListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompUnitStateDTO>,GetFailureListParameterSet>(channel, channel.BeginGetCompUnitFailureList, channel.EndGetCompUnitFailureList, parameters);
        }

        public Task<List<ChemicalTestDTO>> GetChemicalTestListAsync(GetChemicalTestListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ChemicalTestDTO>,GetChemicalTestListParameterSet>(channel, channel.BeginGetChemicalTestList, channel.EndGetChemicalTestList, parameters);
        }

        public Task<int> AddChemicalTestAsync(AddChemicalTestParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddChemicalTestParameterSet>(channel, channel.BeginAddChemicalTest, channel.EndAddChemicalTest, parameters);
        }

        public Task EditChemicalTestAsync(EditChemicalTestParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditChemicalTest, channel.EndEditChemicalTest, parameters);
        }

        public Task DeleteChemicalTestAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteChemicalTest, channel.EndDeleteChemicalTest, parameters);
        }

        public Task<List<CompUnitTestDTO>> GetCompUnitTestListAsync(GetCompUnitTestListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompUnitTestDTO>,GetCompUnitTestListParameterSet>(channel, channel.BeginGetCompUnitTestList, channel.EndGetCompUnitTestList, parameters);
        }

        public Task<int> AddCompUnitTestAsync(AddCompUnitTestParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddCompUnitTestParameterSet>(channel, channel.BeginAddCompUnitTest, channel.EndAddCompUnitTest, parameters);
        }

        public Task EditCompUnitTestAsync(EditCompUnitTestParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCompUnitTest, channel.EndEditCompUnitTest, parameters);
        }

        public Task DeleteCompUnitTestAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteCompUnitTest, channel.EndDeleteCompUnitTest, parameters);
        }

        public Task<int> AddCompUnitTestAttachmentAsync(AddAttachmentParameterSet<int> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddAttachmentParameterSet<int>>(channel, channel.BeginAddCompUnitTestAttachment, channel.EndAddCompUnitTestAttachment, parameters);
        }

        public Task RemoveCompUnitTestAttachmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveCompUnitTestAttachment, channel.EndRemoveCompUnitTestAttachment, parameters);
        }

        public Task AddCompUnitTestPointAsync(AddCompUnitTestPointParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddCompUnitTestPoint, channel.EndAddCompUnitTestPoint, parameters);
        }

        public Task RemoveCompUnitTestPointAsync(DeleteCompUnitTestPointParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveCompUnitTestPoint, channel.EndRemoveCompUnitTestPoint, parameters);
        }

    }
}
