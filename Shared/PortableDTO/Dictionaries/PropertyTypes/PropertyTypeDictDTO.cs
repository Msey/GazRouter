using System;
using System.Linq;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DTO.Dictionaries.PropertyTypes
{
    [DataContract]
    public class PropertyTypeDictDTO : PropertyTypeDTO
    {
        [DataMember]
        public StateSet StateSetId { get; set; }
        

        //[DataMember]
        //public StateSetBaseDTO StateSet { get; set; }

        //public StateBaseDTO GetState(int val)
        //{
        //    StateBaseDTO state = null;
        //    switch (StateSet.Set)
        //    {
        //        case StatesModel.StateSet.ValveStates:
        //            state = ((StateSetDTO<StateDTO<ValveState>>)StateSet).StateList.SingleOrDefault(s => s.Id == val);
        //            break;
        //        case StatesModel.StateSet.CompUnitStates:
        //            state = ((StateSetDTO<StateDTO<CompUnitState>>)StateSet).StateList.SingleOrDefault(s => s.Id == val);
        //            break;
        //        case StatesModel.StateSet.CompShopStates:
        //            state = ((StateSetDTO<StateDTO<CompShopState>>)StateSet).StateList.SingleOrDefault(s => s.Id == val);
        //            break;
        //    }

        //    if (state != null)
        //        return state;

        //    throw new Exception("Значение не найдено в наборе состояний");
        //}

    }
}