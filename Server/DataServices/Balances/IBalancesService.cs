using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.BalanceMeasurings;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Balances.MiscTab;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;


namespace GazRouter.DataServices.Balances
{
    [Service("Балансы")]
    [ServiceContract]
    public interface IBalancesService
    {

#region GASOWNERS

        [ServiceAction("Получение списка поставщиков")]
        [OperationContract]
        List<GasOwnerDTO> GetGasOwnerList(int? parameters);

		[ServiceAction("Добавление поставщика")]
        [OperationContract]
        int AddGasOwner(AddGasOwnerParameterSet parameters);

		[ServiceAction("Редактирование поставщика")]
        [OperationContract]
		void EditGasOwner(EditGasOwnerParameterSet parameters);

		[ServiceAction("Удаление поставщика")]
        [OperationContract]
		void DeleteGasOwner(int parameters);

		[ServiceAction("Редактирование Порядка сортировки")]
		[OperationContract]
		void SetGasOwnerSortOrder(SetGasOwnerSortOrderParameterSet parameters);


        [ServiceAction("Список отключенных поставщиков для точек приема/сдачи газа")]
        [OperationContract]
        List<GasOwnerDisableDTO> GetGasOwnerDisableList();


        [ServiceAction("Включение/отключение поставщика для точки приема/сдачи газа")]
        [OperationContract]
        void SetGasOwnerDisable(SetGasOwnerDisableParameterSet parameters);


        [ServiceAction("Добавление поставщика в ГТС")]
        [OperationContract]
        void SetGasOwnerSystem(SetGasOwnerSystemParameterSet parameters);

#endregion









#region CONTRACT

        [ServiceAction("Получение контракта")]
        [OperationContract]
        ContractDTO GetContract(GetContractListParameterSet parameters);

#endregion



#region BALANCE VALUES

        [ServiceAction("Получение балансовых значений (объемов)")]
        [OperationContract]
        List<BalanceValueDTO> GetBalanceValues(int parameters);


        [ServiceAction("Сохранение балансовых значений (объемов)")]
        [OperationContract]
        void SaveBalanceValues(SaveBalanceValuesParameterSet parameters );


        [ServiceAction("Сохранение балансового значения")]
        [OperationContract]
        void SetBalanceValue(SetBalanceValueParameterSet parameters);


        [ServiceAction("Очистка балансовых значений")]
        [OperationContract]
        void ClearBalanceValues(ClearBalanceValuesParameterSet parameters);


        #endregion


        #region VALUE SWAPS

        [ServiceAction("Получение данных по замещению газа")]
        [OperationContract]
        List<SwapDTO> GetValueSwaps(int parameters);

        #endregion





        #region ROUTES

        [ServiceAction("Сохранение маршрута")]
        [OperationContract]
        int SetRoute(SetRouteParameterSet parameters);

        [ServiceAction("Получение списка маршрутов")]
        [OperationContract]
        List<RouteDTO> GetRoutesList(GetRouteListParameterSet parameters);

        [ServiceAction("Добавление части нового маршрута")]
        [OperationContract]
        int AddRouteSection(AddRouteSectionParameterSet parameters);
        
        [ServiceAction("Добавление всех маршрутов")]
        [OperationContract]
        void AddAllRoutes(GetRouteListParameterSet parameters);

#endregion


#region ROUTE EXCEPTIONS

        [ServiceAction("Добавление исключения для маршрута")]
        [OperationContract]
        int AddRouteException(AddRouteExceptionParameterSet parameters);


        [ServiceAction("Редактирование исключения для маршрута")]
        [OperationContract]
        void EditRouteException(EditRouteExceptionParameterSet parameters);


        [ServiceAction("Удаление исключения для маршрута")]
        [OperationContract]
        void DeleteRouteException(int parameters);

#endregion



 #region DOCS

        [ServiceAction("Получение списка файлов контракта")]
        [OperationContract]
        List<DocDTO> GetDocList(int? parameters);

        [ServiceAction("Добавление файла к контракту")]
        [OperationContract]
        int AddDoc(AddDocParameterSet parameters);

        [ServiceAction("Редактирование файла к контракту")]
        [OperationContract]
        void EditDoc(EditDocParameterSet parameters);

        [ServiceAction("Удаление файла из контракта")]
        [OperationContract]
        void DeleteDoc(int parameters);

#endregion



#region TRANSPORT

        [ServiceAction("Получение детализации ТТР")]
        [OperationContract]
        List<TransportDTO> GetTransportList(HandleTransportListParameterSet parameters);

        [ServiceAction("Расчет ТТР")]
        [OperationContract]
        void CalculateTransportList(HandleTransportListParameterSet parameters);

        [ServiceAction("Очистка результат расчета ТТР")]
        [OperationContract]
        void CLearTransportResults(HandleTransportListParameterSet parameters);

#endregion



        #region SORT ORDER

        [ServiceAction("Получить порядок сортировки")]
        [OperationContract]
        List<BalSortOrderDTO> GetSortOrderList();


        [ServiceAction("Установить порядок сортировки")]
        [OperationContract]
        void SetSortOrder(SetBalSortOrderParameterSet parameters);

#endregion




#region MISC TAB

        [ServiceAction("Добавить объект на вкладку СПРАВОЧНО")]
        [OperationContract]
        List<CommonEntityDTO> GetMiscTabEntityList(int parameters);


        [ServiceAction("Добавить объект на вкладку СПРАВОЧНО")]
        [OperationContract]
        void AddMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters);


        [ServiceAction("Удалить объект с вкладки СПРАВОЧНО")]
        [OperationContract]
        void RemoveMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters);

#endregion



#region BALANCE GROUPS

        [ServiceAction("Получить список балансовых групп")]
        [OperationContract]
        List<BalanceGroupDTO> GetBalanceGroupList(int parameters);


        [ServiceAction("Добавить балансовую группу")]
        [OperationContract]
        int AddBalanceGroup(AddBalanceGroupParameterSet parameters);


        [ServiceAction("Редактировать балансовую группу")]
        [OperationContract]
        void EditBalanceGroup(EditBalanceGroupParameterSet parameters);


        [ServiceAction("Удалить балансовую группу")]
        [OperationContract]
        void RemoveBalanceGroup(int parameters);

        #endregion



        #region ГАЗОРАСПРЕДЕЛИТЕЛЬНЫЕ ОРГАНИЗАЦИИ

        [ServiceAction("Получить список ГРО")]
        [OperationContract]
        List<DistrNetworkDTO> GetDistrNetworkList();


        [ServiceAction("Добавить ГРО")]
        [OperationContract]
        int AddDistrNetwork(AddDistrNetworkParameterSet parameters);


        [ServiceAction("Редактировать ГРО")]
        [OperationContract]
        void EditDistrNetwork(EditDistrNetworkParameterSet parameters);


        [ServiceAction("Удалить ГРО")]
        [OperationContract]
        void RemoveDistrNetwork(int parameters);

        #endregion



        #region INPUT STATES

        [ServiceAction("Получение статуса ввода")]
        [OperationContract]
        List<InputStateDTO> GetInputStateList(GetInputStateListParameterSet parameters);

        [ServiceAction("Изменение статуса ввода")]
        [OperationContract]
        void SetInputState(SetInputStateParameterSet parameters);

        #endregion


        #region DAY BALANCE

        [ServiceAction("Получить данные для формирования суточного баланса")]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        [OperationContract]
        DayBalanceDataDTO GetDayBalanceData(GetDayBalanceDataParameterSet parameters);

        #endregion



        #region MONTH BALANCE ALGORITHMS

        [ServiceAction("Формирование значения для месячного баланса на основе данных суточного баланса")]
        [OperationContract]
        void RunDivideVolumeAlgorithm(DivideVolumeAlgorithmParameterSet parameters);


        [ServiceAction("Формирование значения для месячного баланса на основе данных по поставщикам за сутки")]
        [OperationContract]
        void RunOwnersDaySumAlgorithm(OwnersDaySumAlgorithmParameterSet parameters);


        [ServiceAction("Перенос запаса газа (конец -> начало)")]
        [OperationContract]
        void RunMoveGasSupplyAlgorithm(int parameters);

        [ServiceAction("Формирование балансового значения СТН за месяц")]
        [OperationContract]
        void MoveAuxCosts(MoveAuxCostsParameterSet parameters);

        [ServiceAction("Перенос значений из одной версии в другую")]
        [OperationContract]
        void MoveValuesToOtherVersion(MoveValuesToOtherVersionParameterSet parameters);

        #endregion



        [ServiceAction("Получение суточных измерений по расходам за указанный месяц")]
        [OperationContract]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))]
        [ServiceKnownType(typeof(PropertyValueStringDTO))]
        [ServiceKnownType(typeof(PropertyValueDateDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> GetBalanceMeasurings(GetBalanceMeasuringsParameterSet parameters);

    }
}
