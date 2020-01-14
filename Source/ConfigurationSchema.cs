// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.37595
//    <NameSpace>Harris.Automation.ADC.Services.ListService</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><EnableLazyLoading>True</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>True</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>True</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace Harris.Automation.ADC.Services.ListService {
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Xml;
    using System.Collections.Generic;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Harris/Automation/ADC/Services/ListService")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://Harris/Automation/ADC/Services/ListService", IsNullable=false)]
    public partial class ListServiceConfiguration : System.ComponentModel.INotifyPropertyChanged {
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private ADCConnectionParameters aDCConnectionOptionsField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<string> commercialContentIdentifiersField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private DataServiceConnectionParameters dataServiceConnectionOptionsField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ADCConnectionParameters ADCConnectionOptions {
            get {
                if ((this.aDCConnectionOptionsField == null)) {
                    this.aDCConnectionOptionsField = new ADCConnectionParameters();
                }
                return this.aDCConnectionOptionsField;
            }
            set {
                if ((this.aDCConnectionOptionsField != null)) {
                    if ((aDCConnectionOptionsField.Equals(value) != true)) {
                        this.aDCConnectionOptionsField = value;
                        this.OnPropertyChanged("ADCConnectionOptions");
                    }
                }
                else {
                    this.aDCConnectionOptionsField = value;
                    this.OnPropertyChanged("ADCConnectionOptions");
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("CommercialContentIdentifiers", DataType="normalizedString", Order=1)]
        public List<string> CommercialContentIdentifiers {
            get {
                if ((this.commercialContentIdentifiersField == null)) {
                    this.commercialContentIdentifiersField = new List<string>();
                }
                return this.commercialContentIdentifiersField;
            }
            set {
                if ((this.commercialContentIdentifiersField != null)) {
                    if ((commercialContentIdentifiersField.Equals(value) != true)) {
                        this.commercialContentIdentifiersField = value;
                        this.OnPropertyChanged("CommercialContentIdentifiers");
                    }
                }
                else {
                    this.commercialContentIdentifiersField = value;
                    this.OnPropertyChanged("CommercialContentIdentifiers");
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public DataServiceConnectionParameters DataServiceConnectionOptions {
            get {
                if ((this.dataServiceConnectionOptionsField == null)) {
                    this.dataServiceConnectionOptionsField = new DataServiceConnectionParameters();
                }
                return this.dataServiceConnectionOptionsField;
            }
            set {
                if ((this.dataServiceConnectionOptionsField != null)) {
                    if ((dataServiceConnectionOptionsField.Equals(value) != true)) {
                        this.dataServiceConnectionOptionsField = value;
                        this.OnPropertyChanged("DataServiceConnectionOptions");
                    }
                }
                else {
                    this.dataServiceConnectionOptionsField = value;
                    this.OnPropertyChanged("DataServiceConnectionOptions");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public virtual void OnPropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null)) {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://Harris/Automation/ADC/Services")]
    public partial class ADCConnectionParameters : System.ComponentModel.INotifyPropertyChanged {
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string applicationNameField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<string> deviceServersToInitializeField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<string> deviceServersToConnectField;
        
        public ADCConnectionParameters() {
            this.applicationNameField = "ADCService";
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string ApplicationName {
            get {
                return this.applicationNameField;
            }
            set {
                if ((this.applicationNameField != null)) {
                    if ((applicationNameField.Equals(value) != true)) {
                        this.applicationNameField = value;
                        this.OnPropertyChanged("ApplicationName");
                    }
                }
                else {
                    this.applicationNameField = value;
                    this.OnPropertyChanged("ApplicationName");
                }
            }
        }
        
        [System.Xml.Serialization.XmlArrayAttribute(Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("DeviceServerName", DataType="normalizedString", IsNullable=false)]
        public List<string> DeviceServersToInitialize {
            get {
                if ((this.deviceServersToInitializeField == null)) {
                    this.deviceServersToInitializeField = new List<string>();
                }
                return this.deviceServersToInitializeField;
            }
            set {
                if ((this.deviceServersToInitializeField != null)) {
                    if ((deviceServersToInitializeField.Equals(value) != true)) {
                        this.deviceServersToInitializeField = value;
                        this.OnPropertyChanged("DeviceServersToInitialize");
                    }
                }
                else {
                    this.deviceServersToInitializeField = value;
                    this.OnPropertyChanged("DeviceServersToInitialize");
                }
            }
        }
        
        [System.Xml.Serialization.XmlArrayAttribute(Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("DeviceServerName", DataType="normalizedString", IsNullable=false)]
        public List<string> DeviceServersToConnect {
            get {
                if ((this.deviceServersToConnectField == null)) {
                    this.deviceServersToConnectField = new List<string>();
                }
                return this.deviceServersToConnectField;
            }
            set {
                if ((this.deviceServersToConnectField != null)) {
                    if ((deviceServersToConnectField.Equals(value) != true)) {
                        this.deviceServersToConnectField = value;
                        this.OnPropertyChanged("DeviceServersToConnect");
                    }
                }
                else {
                    this.deviceServersToConnectField = value;
                    this.OnPropertyChanged("DeviceServersToConnect");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public virtual void OnPropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null)) {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://Harris/Automation/ADC/Services")]
    public partial class DataServiceConnectionParameters : System.ComponentModel.INotifyPropertyChanged {
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string dataServiceAddressField;
        
        public DataServiceConnectionParameters() {
            this.dataServiceAddressField = "http://localhost:10021/Harris.Automation.ADC.Services.DataService";
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="normalizedString", Order=0)]
        public string DataServiceAddress {
            get {
                return this.dataServiceAddressField;
            }
            set {
                if ((this.dataServiceAddressField != null)) {
                    if ((dataServiceAddressField.Equals(value) != true)) {
                        this.dataServiceAddressField = value;
                        this.OnPropertyChanged("DataServiceAddress");
                    }
                }
                else {
                    this.dataServiceAddressField = value;
                    this.OnPropertyChanged("DataServiceAddress");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public virtual void OnPropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null)) {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}