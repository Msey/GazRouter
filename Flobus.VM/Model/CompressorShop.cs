using System;
using System.Collections.Generic;
using System.Windows;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.VM.Model
{
    public class CompressorShop : EntityBase<CompShopDTO, Guid>, ISearchable, ICompressorShop
    {
        private bool _isFound;
        private object _data;

        public CompressorShop(CompShopDTO compShopDTO, Point position) : base(compShopDTO)
        {
            CompressorUnits = new List<CompressorUnit>();
            Position = position;

            CompressorShopMeasuring = new CompressorShopMeasuring(Dto,this);
        }

        public List<CompressorUnit> CompressorUnits { get; }

        public CompressorShopMeasuring CompressorShopMeasuring
        {
            get { return Data as CompressorShopMeasuring; }
            private set { Data = value; }
        }

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }

        public object Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }
    }
}