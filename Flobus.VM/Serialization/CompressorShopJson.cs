using System;
using System.Windows;
using GazRouter.Flobus.VM.Model;

namespace GazRouter.Flobus.VM.Serialization
{
    public class CompressorShopJson
    {
        public CompressorShopJson()
        {
        }

        public CompressorShopJson(CompressorShop shop)
        {
            Id = shop.Id;
            Position = shop.Position;
        }

        public Guid Id { get; set; }
        public Point Position { get; set; }

        public static CompressorShopJson FromModel(CompressorShop compressorShop)
        {
            return new CompressorShopJson(compressorShop);
        }
    }
}