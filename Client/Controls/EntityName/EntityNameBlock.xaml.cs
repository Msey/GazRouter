using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GazRouter.Controls.Converters;
using GazRouter.Controls.Dialogs.CompUnitEfficiency.Dialog;
using GazRouter.Controls.Dialogs.ObjectDetails;
using GazRouter.Controls.Trends;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using Telerik.Windows;
using Telerik.Windows.Controls;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.Controls.EntityName
{
    public partial class EntityNameBlock
    {
        public EntityNameBlock()
        {
            InitializeComponent();

            IconImg.Source = new BitmapImage(new Uri("/Common;component/Images/16x16/object2.png", UriKind.Relative));

            IconImg.Visibility = Visibility.Collapsed;
            Menu.Visibility = Visibility.Collapsed;
            TypeTxt.Visibility = Visibility.Collapsed;
            Highlight.Visibility = Visibility.Collapsed;
            
        }

        
        // отобразить форму с паспортной информацией
        private void OnReferencedData(object sender, RadRoutedEventArgs e)
        {
            new ObjectDetailsView { DataContext = new ObjectDetailsViewModel(Entity.Id, Entity.EntityType)}.ShowDialog();
        }




        #region Entity
        public CommonEntityDTO Entity
        {
            get { return GetValue(EntityProperty) as CommonEntityDTO; }
            set { SetValue(EntityProperty, value); }
        }

        
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register(
                "Entity",
                typeof(CommonEntityDTO),
                typeof(EntityNameBlock),
                new PropertyMetadata(OnEntityPropertyChanged));

        
        private static void OnEntityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl?.Entity != null)
            {
                ctrl.NameTxt.Text = ctrl.GetName();
                ctrl.TypeTxt.Text = new EntityTypeToNameConverter().Convert(ctrl.Entity.EntityType) + (ctrl.Entity.EntityType == EntityType.Pipeline && ctrl.Kilometer!= null ? (" [Километр : " + ctrl.Kilometer +"]") : "");
                ctrl.IconImg.Source = EntityTypeToImageSourceConverter.Convert(ctrl.Entity.EntityType);
                ctrl.FillMenu();
            }
        }

        #endregion

        public System.DateTime Period 
        {
            get { return (System.DateTime)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        public static readonly DependencyProperty PeriodProperty =
          DependencyProperty.Register(
              "Period",
              typeof(System.DateTime),
              typeof(EntityNameBlock),
              new PropertyMetadata(OnEntityPropertyChanged));
        public double? Kilometer
        {
            get { return (double?)GetValue(KilometerProperty); }
            set { SetValue(KilometerProperty, value); }
        }

        public static readonly DependencyProperty KilometerProperty =
          DependencyProperty.Register(
              "Kilometer",
              typeof(double?),
              typeof(EntityNameBlock),
              new PropertyMetadata(OnEntityPropertyChanged));

        public string GetName()
        {
            if (!string.IsNullOrEmpty(Alias)) return Alias;
            if (Entity == null) return "";

            return UseShortPathAsName ?  Entity.ShortPath : Entity.Name;
        }

        // Контекстное меню
        private void FillMenu()
        {
            Menu.Items.Clear();
            var refDataItem = new RadMenuItem {Header = "Паспорт..."};
            refDataItem.Click += OnReferencedData;
            Menu.Items.Add(refDataItem );

            if (Entity.EntityType == EntityType.CompUnit)
            {
                var workPointItem = new RadMenuItem { Header = "Рабочая точка..." };
                workPointItem.Click +=
                    (obj, e) =>
                        (new CompUnitEfficiencyView {DataContext = new CompUnitEfficiencyViewModel(Entity, Period) }).ShowDialog();
                Menu.Items.Add(workPointItem);
            }


            // Тренды
            var trendsItem = new RadMenuItem {Header = "Тренды"};
            var trends = TrendsHelper.GetTrendDict(Entity);
            foreach (var t in trends)
            {
                var tItem = new RadMenuItem {Header = t.Key};
                tItem.Click += (obj, e) => t.Value();
                trendsItem.Items.Add(tItem);
            }
            if (trendsItem.Items.Any())
                Menu.Items.Add(trendsItem);
            
        }


        #region Display Icon

        // Отображать иконку типа
        public bool DisplayIcon
        {
            get { return (bool) GetValue(DisplayIconProperty); }
            set { SetValue(DisplayIconProperty, value); }
        }


        public static readonly DependencyProperty DisplayIconProperty =
            DependencyProperty.Register(
                "DisplayIcon",
                typeof(bool),
                typeof(EntityNameBlock),
                new PropertyMetadata(OnDisplayIconPropertyChanged));

        private static void OnDisplayIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl != null)
                ctrl.IconImg.Visibility = ctrl.DisplayIcon ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion


        #region Display Type

        // Отображать тип объекта под названием
        public bool DisplayType
        {
            get { return (bool)GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }


        public static readonly DependencyProperty DisplayTypeProperty =
            DependencyProperty.Register(
                "DisplayType",
                typeof(bool),
                typeof(EntityNameBlock),
                new PropertyMetadata(OnDisplayTypePropertyChanged));

        private static void OnDisplayTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl != null)
                ctrl.TypeTxt.Visibility = ctrl.DisplayType ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion



        #region IsMenuEnable

        // Включить контекстное меню
        public bool IsMenuEnable
        {
            get { return (bool)GetValue(IsMenuEnableProperty); }
            set { SetValue(IsMenuEnableProperty, value); }
        }


        public static readonly DependencyProperty IsMenuEnableProperty =
            DependencyProperty.Register(
                "IsMenuEnable",
                typeof(bool),
                typeof(EntityNameBlock),
                new PropertyMetadata(OnIsMenuEnablePropertyChanged));

        private static void OnIsMenuEnablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl != null)
                ctrl.Menu.Visibility = ctrl.IsMenuEnable ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion


        #region UseShortPathAsName

        // Если этот признак выставлен в true, то отображает короткий путь как наименование объекта

        public bool UseShortPathAsName
        {
            get { return (bool)GetValue(UseShortPathAsNameProperty); }
            set { SetValue(UseShortPathAsNameProperty, value); }
        }


        public static readonly DependencyProperty UseShortPathAsNameProperty =
            DependencyProperty.Register(
                "UseShortPathAsName",
                typeof(bool),
                typeof(EntityNameBlock),
                new PropertyMetadata(UseShortPathAsNamePropertyChanged));

        private static void UseShortPathAsNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl != null)
                ctrl.NameTxt.Text = ctrl.GetName();
        }

        #endregion


        #region Alias

        // Используется если нужно назвать объект как-то иначе, не так как он называется в базе

        public string Alias
        {
            get { return (string)GetValue(AliasProperty); }
            set { SetValue(AliasProperty, value); }
        }


        public static readonly DependencyProperty AliasProperty =
            DependencyProperty.Register(
                "Alias",
                typeof(string),
                typeof(EntityNameBlock),
                new PropertyMetadata(OnAliasPropertyChanged));

        private static void OnAliasPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EntityNameBlock;
            if (ctrl != null)
                ctrl.NameTxt.Text = ctrl.GetName();
        }

        #endregion

        private void NameTxt_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Highlight.Visibility = Visibility.Visible;
        }

        private void NameTxt_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Highlight.Visibility = Visibility.Collapsed;
        }
    }
   
}
