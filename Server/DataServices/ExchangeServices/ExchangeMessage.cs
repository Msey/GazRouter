﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace DataServices.ExchangeServices {
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class ExchangeMessage {
        
        private ExchangeMessageHeaderSection headerSectionField;
        
        private ExchangeMessageDataSection dataSectionField;
        
        /// <remarks/>
        public ExchangeMessageHeaderSection HeaderSection {
            get {
                return this.headerSectionField;
            }
            set {
                this.headerSectionField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSection DataSection {
            get {
                return this.dataSectionField;
            }
            set {
                this.dataSectionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageHeaderSection {
        
        private System.DateTime timestampField;
        
        private string periodTypeField;
        
        private string senderField;
        
        private string receiverField;
        
        private System.DateTime generatedTimeField;
        
        private string commentField;
        
        /// <remarks/>
        public System.DateTime timestamp {
            get {
                return this.timestampField;
            }
            set {
                this.timestampField = value;
            }
        }
        
        /// <remarks/>
        public string periodType {
            get {
                return this.periodTypeField;
            }
            set {
                this.periodTypeField = value;
            }
        }
        
        /// <remarks/>
        public string sender {
            get {
                return this.senderField;
            }
            set {
                this.senderField = value;
            }
        }
        
        /// <remarks/>
        public string receiver {
            get {
                return this.receiverField;
            }
            set {
                this.receiverField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime generatedTime {
            get {
                return this.generatedTimeField;
            }
            set {
                this.generatedTimeField = value;
            }
        }
        
        /// <remarks/>
        public string comment {
            get {
                return this.commentField;
            }
            set {
                this.commentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSection {
        
        private ExchangeMessageDataSectionSite siteField;
        
        private ExchangeMessageDataSectionCompressorStation compressorStationField;
        
        private ExchangeMessageDataSectionCompressorShop compressorShopField;
        
        /// <remarks/>
        public ExchangeMessageDataSectionSite site {
            get {
                return this.siteField;
            }
            set {
                this.siteField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorStation compressorStation {
            get {
                return this.compressorStationField;
            }
            set {
                this.compressorStationField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorShop compressorShop {
            get {
                return this.compressorShopField;
            }
            set {
                this.compressorShopField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionSite {
        
        private string idField;
        
        private string nameField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorStation {
        
        private string idField;
        
        private string nameField;
        
        private string siteIdField;
        
        private ExchangeMessageDataSectionCompressorStationValueList valueListField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string SiteId {
            get {
                return this.siteIdField;
            }
            set {
                this.siteIdField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorStationValueList valueList {
            get {
                return this.valueListField;
            }
            set {
                this.valueListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorStationValueList {
        
        private string temperatureAirField;
        
        private string temperatureGroundField;
        
        /// <remarks/>
        public string temperatureAir {
            get {
                return this.temperatureAirField;
            }
            set {
                this.temperatureAirField = value;
            }
        }
        
        /// <remarks/>
        public string temperatureGround {
            get {
                return this.temperatureGroundField;
            }
            set {
                this.temperatureGroundField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorShop {
        
        private string idField;
        
        private string nameField;
        
        private string compressorShopTypeIdField;
        
        private string pipelineIdField;
        
        private string kilometerConnField;
        
        private ExchangeMessageDataSectionCompressorShopValueList valueListField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string compressorShopTypeId {
            get {
                return this.compressorShopTypeIdField;
            }
            set {
                this.compressorShopTypeIdField = value;
            }
        }
        
        /// <remarks/>
        public string pipelineId {
            get {
                return this.pipelineIdField;
            }
            set {
                this.pipelineIdField = value;
            }
        }
        
        /// <remarks/>
        public string kilometerConn {
            get {
                return this.kilometerConnField;
            }
            set {
                this.kilometerConnField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorShopValueList valueList {
            get {
                return this.valueListField;
            }
            set {
                this.valueListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorShopValueList {
        
        private string pressureInletField;
        
        private string pressureOutletField;
        
        private string temperatureInletField;
        
        private string temperatureOutletField;
        
        private string temperatureCoolingField;
        
        private ExchangeMessageDataSectionCompressorShopValueListScheme schemeField;
        
        /// <remarks/>
        public string pressureInlet {
            get {
                return this.pressureInletField;
            }
            set {
                this.pressureInletField = value;
            }
        }
        
        /// <remarks/>
        public string pressureOutlet {
            get {
                return this.pressureOutletField;
            }
            set {
                this.pressureOutletField = value;
            }
        }
        
        /// <remarks/>
        public string temperatureInlet {
            get {
                return this.temperatureInletField;
            }
            set {
                this.temperatureInletField = value;
            }
        }
        
        /// <remarks/>
        public string temperatureOutlet {
            get {
                return this.temperatureOutletField;
            }
            set {
                this.temperatureOutletField = value;
            }
        }
        
        /// <remarks/>
        public string temperatureCooling {
            get {
                return this.temperatureCoolingField;
            }
            set {
                this.temperatureCoolingField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorShopValueListScheme scheme {
            get {
                return this.schemeField;
            }
            set {
                this.schemeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorShopValueListScheme {
        
        private ExchangeMessageDataSectionCompressorShopValueListSchemeGroupsCount groupsCountField;
        
        private ExchangeMessageDataSectionCompressorShopValueListSchemeCompressionStagesCount compressionStagesCountField;
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorShopValueListSchemeGroupsCount groupsCount {
            get {
                return this.groupsCountField;
            }
            set {
                this.groupsCountField = value;
            }
        }
        
        /// <remarks/>
        public ExchangeMessageDataSectionCompressorShopValueListSchemeCompressionStagesCount compressionStagesCount {
            get {
                return this.compressionStagesCountField;
            }
            set {
                this.compressionStagesCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorShopValueListSchemeGroupsCount {
        
        private string numberField;
        
        private string valueField;
        
        /// <remarks/>
        public string number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
        
        /// <remarks/>
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ExchangeMessageDataSectionCompressorShopValueListSchemeCompressionStagesCount {
        
        private string numberField;
        
        private string valueField;
        
        /// <remarks/>
        public string number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
        
        /// <remarks/>
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
}