using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.ManualInput.CompUnitStates;


namespace GazRouter.ManualInput.CompUnits
{
    public class AddFailureDependencesViewModel : ViewModelBase
    {


        public AddFailureDependencesViewModel(List<Dependency> dependencyList, bool findFailures)
        {
            DependencyList = dependencyList;
            FindFailures = findFailures;
            //OnPropertyChanged(() => DependencyList);
        }


        public List<Dependency> DependencyList { get; set; }

        public bool FindFailures { get; set; }
    }


    public class Dependency
    {
        public string UnitName { get; set; }
        public string UnitType { get; set; }

        public CompUnitStateDTO Detail { get; set; }

        public bool IsDepend { get; set; }
    }

   
}