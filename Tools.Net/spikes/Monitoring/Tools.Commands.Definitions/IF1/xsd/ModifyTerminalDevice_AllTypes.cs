﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Tools.Commands.Implementation.IF1.ModifyTerminalDevice
{
    using System.Xml.Serialization;

    // 
    // This source code was auto-generated by xsd, Version=2.0.50727.3038.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd", IsNullable = false)]
    public partial class ModifyTerminalDevice
    {

        private req reqField;

        private res resField;

        /// <remarks/>
        public req req
        {
            get
            {
                return this.reqField;
            }
            set
            {
                this.reqField = value;
            }
        }

        /// <remarks/>
        public res res
        {
            get
            {
                return this.resField;
            }
            set
            {
                this.resField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd", IsNullable = false)]
    public partial class req
    {

        private string tISTDidField;

        private string tISwalletIdField;

        private double monthlyLimitField;

        private bool monthlyLimitFieldSpecified;

        private string phoneNumberField;

        private string contractEndField;

        private string reqIdField;

        private System.DateTime reqTimeField;

        /// <remarks/>
        public string TISTDid
        {
            get
            {
                return this.tISTDidField;
            }
            set
            {
                this.tISTDidField = value;
            }
        }

        /// <remarks/>
        public string TISwalletId
        {
            get
            {
                return this.tISwalletIdField;
            }
            set
            {
                this.tISwalletIdField = value;
            }
        }

        public double monthlyLimit
        {
            get
            {
                return this.monthlyLimitField;
            }
            set
            {
                this.monthlyLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool monthlyLimitSpecified
        {
            get
            {
                return this.monthlyLimitFieldSpecified;
            }
            set
            {
                this.monthlyLimitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string phoneNumber
        {
            get
            {
                return this.phoneNumberField;
            }
            set
            {
                this.phoneNumberField = value;
            }
        }

        /// <remarks/>
        public string contractEnd
        {
            get
            {
                return this.contractEndField;
            }
            set
            {
                this.contractEndField = value;
            }
        }

        /// <remarks/>
        public string reqId
        {
            get
            {
                return this.reqIdField;
            }
            set
            {
                this.reqIdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime reqTime
        {
            get
            {
                return this.reqTimeField;
            }
            set
            {
                this.reqTimeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd", IsNullable = false)]
    public partial class res
    {

        private result resultField;

        /// <remarks/>
        public result result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/AllT" +
        "ypes.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/Modi" +
        "fyTerminalDevice.xsd", IsNullable = false)]
    public partial class result
    {

        private string codeField;

        private string descField;

        /// <remarks/>
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }
}