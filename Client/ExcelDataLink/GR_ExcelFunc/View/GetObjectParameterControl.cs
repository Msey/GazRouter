using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDna.Integration;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using GR_ExcelFunc.Model;

namespace GR_ExcelFunc.View
{
    [ComVisible(true)]
    public partial class GetObjectParameterControl : UserControl, ISelectObjectParameterView
    {
        private bool _isUpdateEntityTypeList;
        public GetObjectParameterControl()
        {
            InitializeComponent();

            SiteListCB.DisplayMember = "Name";
            SiteListCB.DropDownStyle = ComboBoxStyle.DropDownList;
            SiteListCB.SelectedIndexChanged += SiteListCbSelecteIndexChanged;

            EntityTypeCB.DisplayMember = "Name";
            EntityTypeCB.DropDownStyle = ComboBoxStyle.DropDownList;
            EntityTypeCB.SelectedIndexChanged += EntityTypeCbSelecteIndexChanged;

            EntityGridView.AutoGenerateColumns = false;
            
            EntityGridView.ColumnCount = 2;
            EntityGridView.Columns[0].HeaderText = "Наименование";
            EntityGridView.Columns[0].DataPropertyName = "ShortPath";
            EntityGridView.Columns[0].ReadOnly = true;
            EntityGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EntityGridView.Columns[1].HeaderText = "Тип";
            EntityGridView.Columns[1].DataPropertyName = "EntityType";
            EntityGridView.Columns[1].ReadOnly = true;

            

            EntityGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EntityGridView.MultiSelect = false;

            EntityGridView.SelectionChanged += EntityGridViewSelection;

            PeriodTypeCB.DisplayMember = "Name";
            PeriodTypeCB.DropDownStyle = ComboBoxStyle.DropDownList;

            PropertyTypeCB.DisplayMember = "Name";
            PropertyTypeCB.DropDownStyle = ComboBoxStyle.DropDownList;

            _isUpdateEntityTypeList = true;


            var time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,0,0);
            startTimePicker1.CustomFormat = "dd.MM.yyyy";
            startTimePicker1.Value = DateTime.Now.AddDays(-1); 
            startTimePicker1.Format = DateTimePickerFormat.Custom;
            
            startTime.Format = DateTimePickerFormat.Custom;
            startTime.CustomFormat = "HH:mm";
            startTime.Value = time;
            startTime.ShowUpDown = true;

            endTimePicker1.CustomFormat = "dd.MM.yyyy";
            endTimePicker1.Value = DateTime.Now;
            endTimePicker1.Format = DateTimePickerFormat.Custom;

            endTime.Format = DateTimePickerFormat.Custom;
            endTime.CustomFormat = "HH:mm";
            endTime.Value = time;
            endTime.ShowUpDown = true;

            //EntityGridView.CellFormatting 

        }

       

        public Presenter.SelectObjectParameterPresenter Presenter { private get; set; }

        public IList<SiteDTO> SiteList
        {
            get { return (IList<SiteDTO>) SiteListCB.DataSource; }
            set
            {
                value.Insert(0, new SiteDTO() {Name = "Все ЛПУ"});
                SiteListCB.DataSource = value; 
            }
        }

        public IList<EntityTypeDTO> EntityTypeList
        {
            get { return (IList<EntityTypeDTO>) EntityTypeCB.DataSource; }
            set
            {
                value.Insert(0, new EntityTypeDTO() {Name = "Все типы объектов"});
                EntityTypeCB.DataSource = value;
            }
        }


        public IList<PropertyTypeDTO> PropertyTypeList
        {
          get { return (IList<PropertyTypeDTO>) PropertyTypeCB.DataSource; }
          set { PropertyTypeCB.DataSource = value; }
        }

        public IList<PeriodTypeDTO> PeriodTypeList
        {
            get { return (IList<PeriodTypeDTO>)PeriodTypeCB.DataSource; }
            set { PeriodTypeCB.DataSource = value; }
        }

        public DateTime StartDate
        {
            get
            {
                var dt = new DateTime(startTimePicker1.Value.Year, startTimePicker1.Value.Month,
                    startTimePicker1.Value.Day,startTime.Value.Hour,0,0);
                return dt; 
            }
            set { startTimePicker1.Value = value; }
        }

        public DateTime EndDate
        {
            get
            {
                var dt = new DateTime(endTimePicker1.Value.Year, endTimePicker1.Value.Month,
                    endTimePicker1.Value.Day, endTime.Value.Hour, 0, 0);
                return  dt;
            }
            set { endTimePicker1.Value = value; }
        }
        public CommonEntityDTO SelectedEntity
        {
            get
            {
                return (CommonEntityDTO)EntityGridView.SelectedRows[0].DataBoundItem;
            }
        }
        public IList<CommonEntityDTO> EntityList
        {
            get { return (IList<CommonEntityDTO>)this.EntityGridView.DataSource; }
            set { this.EntityGridView.DataSource = value; }
        }


        private void EntityTypeCbSelecteIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdateEntityTypeList)
            {
                if (searchBox1.Text.Length > 0 && EntityTypeCB.SelectedIndex != 0)
                {
                    Presenter.UpdateEntityList(searchBox1.Text, ((EntityTypeDTO)EntityTypeCB.SelectedItem).EntityType);
                }
                else if (EntityTypeCB.SelectedIndex == 0)
                {
                    Presenter.UpdateEntityList();
                    searchBox1.Text = "";
                }
                else
                {
                    Presenter.UpdateEntityList(((EntityTypeDTO) EntityTypeCB.SelectedItem).EntityType);
                }
            }
            else
            {
                _isUpdateEntityTypeList = true;
            }

        }
        private void SiteListCbSelecteIndexChanged(object sender, EventArgs e)
        {
            //if(SiteListCB.SelectedIndex != 0)
            //    Presenter.UpdateEntityList( (SiteDTO)SiteListCB.SelectedItem );
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // Presenter.UpdateEntityList(searchBox1.Text, ((EntityTypeDTO) EntityTypeCB.SelectedItem).EntityType);
            if(EntityTypeCB.SelectedIndex != 0 )
            { 
                _isUpdateEntityTypeList = false;
                EntityTypeCB.SelectedIndex = 0;
            }
            Presenter.UpdateEntityList(searchBox1.Text, null);
           
        }

        private void EntityGridViewSelection(object sender, EventArgs e)
        {
            if ( EntityGridView.SelectedRows.Count > 0)
            {
                var el = (CommonEntityDTO)EntityGridView.SelectedRows[0].DataBoundItem;
                Presenter.UpdatePropertyList(el.EntityType);
            }
            
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ParentForm?.Close();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            SetValuesOnSheet(startTimePicker1.ToString());
            ParentForm?.Close();
        }

        public  void SetValuesOnSheet(string s)
        {
            var selectedItem = (CommonEntityDTO)EntityGridView.SelectedRows[0].DataBoundItem;
            var propertyType = ((PropertyTypeDTO)PropertyTypeCB.SelectedItem).PropertyType;
            var period = ((PeriodTypeDTO)PeriodTypeCB.SelectedItem).PeriodType;
            
            FillExcel.FillData(StartDate, EndDate, selectedItem.Id, propertyType, period);


        }
    }
}
