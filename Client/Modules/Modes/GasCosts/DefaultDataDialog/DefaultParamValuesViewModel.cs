using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.GasCosts.DefaultDataDialog
{
    public class DefaultParamValuesViewModel : DialogViewModel
    {
        public DefaultParamValuesViewModel(Action closeCallback)
            : base(closeCallback)
        {
            SaveCommand = new DelegateCommand(async () =>
            {
                DialogResult = true;
                await
                    new GasCostsServiceProxy().SetDefaultParamValuesAsync(DefaultParamValues.Select(v => v.Dto).ToList());
            });
        }

        public List<DefaultParamValues> DefaultParamValues { get; set; }

        public DelegateCommand SaveCommand { get; set; }
    }
}