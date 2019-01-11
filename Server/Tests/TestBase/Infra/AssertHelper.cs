using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBase.Infra
{
    public static class AssertHelper
    {
        public static void CheckDictionary<TItem, TEnum >(List<TItem> dict, Func<TItem, TEnum> funct) where TEnum : struct, IComparable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }


            Assert.IsNotNull(dict);
            var enumValues = Enum.GetValues(typeof(TEnum));
            var expectedcount = enumValues.Length;
            //if (Enum.IsDefined(typeof(TEnum), 0) ) expectedcount--;
           

            foreach (var value in enumValues)
            {

                if (dict.All(c => funct(c).CompareTo((TEnum)value) != 0))
                {
                    if (((int)value) != 0)
                        Assert.Fail("Элемент не определен в БД : {0}", ((int)value));
                    else
                    {
                        expectedcount --;
                    }
                }

            }
            Assert.AreEqual(expectedcount, dict.Count, "Количество элементов в словаре БД и в Enum различается");
        }


        public static void IsNotEmpty(ICollection collection ) 
        {
            Assert.IsFalse(collection.Count == 0, "Collection is empty");
           
        }

        public static void IsEmpty(ICollection collection)
        {
            Assert.IsTrue(collection.Count == 0, "Collection is not empty");

        }
    }
}