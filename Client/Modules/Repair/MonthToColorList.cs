using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Common;

namespace GazRouter.Repair
{
    public sealed class MonthToColorList
    {
        private const string StorageName = "RepairMonthToColorList";

        public MonthToColorList()
        {
            var colorList = IsolatedStorageManager.Get<List<MonthToColor>>(StorageName);
            if (colorList != null)
            {
                ColorList = colorList.Select(c => new MonthToColorWrap(c)).ToList();
            }
            else
            {
                ColorList = GetDafault().Select(c => new MonthToColorWrap(c)).ToList();
            }

            ColorList.ForEach(c => c.PropertyChanged += (sender, args) =>
            {
                Save();
                OnColorChanged();
            });
        }

        public event EventHandler ColorChanged;

        public List<MonthToColorWrap> ColorList { get; set; }

        public void Save()
        {
            IsolatedStorageManager.Set(StorageName, ColorList.Select(c => c.MonthToColor).ToList());
        }


        public Color GetColor(int month)
        {
            if (month < 1 && month > 12) return Colors.Transparent;

            return ColorList.Single(c => c.MonthToColor.Month == month).Color;
        }

        private static IEnumerable<MonthToColor> GetDafault()
        {
            yield return new MonthToColor(1, Color.FromArgb(0xff, 0, 128, 255));
            yield return new MonthToColor(2, Color.FromArgb(0xff, 128, 255, 255));
            yield return new MonthToColor(3, Color.FromArgb(0xff, 254, 1, 235));
            yield return new MonthToColor(4, Color.FromArgb(0xff, 255, 128, 128));
            yield return new MonthToColor(5, Color.FromArgb(0xff, 255, 0, 0));
            yield return new MonthToColor(6, Color.FromArgb(0xff, 128, 255, 128));
            yield return new MonthToColor(7, Color.FromArgb(0xff, 34, 177, 76));
            yield return new MonthToColor(8, Color.FromArgb(0xff, 64, 128, 128));
            yield return new MonthToColor(9, Color.FromArgb(0xff, 255, 201, 14));
            yield return new MonthToColor(10, Color.FromArgb(0xff, 255, 127, 39));
            yield return new MonthToColor(11, Color.FromArgb(0xff, 185, 122, 87));
            yield return new MonthToColor(12, Color.FromArgb(0xff, 128, 128, 64));
        }

        private void OnColorChanged()
        {
            ColorChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}