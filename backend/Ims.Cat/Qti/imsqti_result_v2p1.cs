﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace Ims.Schemas.qti_result_v2p1 {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="AssessmentResult.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    [System.Xml.Serialization.XmlRootAttribute("assessmentResult", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1", IsNullable=false)]
    public partial class AssessmentResultType {
        
        private ContextType contextField;
        
        private TestResultType testResultField;
        
        private ItemResultType[] itemResultField;
        
        /// <remarks/>
        public ContextType context {
            get {
                return this.contextField;
            }
            set {
                this.contextField = value;
            }
        }
        
        /// <remarks/>
        public TestResultType testResult {
            get {
                return this.testResultField;
            }
            set {
                this.testResultField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("itemResult")]
        public ItemResultType[] itemResult {
            get {
                return this.itemResultField;
            }
            set {
                this.itemResultField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Context.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class ContextType {
        
        private SessionIdentifierType[] sessionIdentifierField;
        
        private string sourcedIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sessionIdentifier")]
        public SessionIdentifierType[] sessionIdentifier {
            get {
                return this.sessionIdentifierField;
            }
            set {
                this.sessionIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string sourcedId {
            get {
                return this.sourcedIdField;
            }
            set {
                this.sourcedIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SessionIdentifier.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class SessionIdentifierType : EmptyPrimitiveTypeType {
        
        private string sourceIDField;
        
        private string identifierField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string sourceID {
            get {
                return this.sourceIDField;
            }
            set {
                this.sourceIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="normalizedString")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SessionIdentifierType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="EmptyPrimitiveType.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class EmptyPrimitiveTypeType {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="TestResult.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class TestResultType {
        
        private object[] itemsField;
        
        private string identifierField;
        
        private System.DateTime datestampField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("outcomeVariable", typeof(OutcomeVariableType))]
        [System.Xml.Serialization.XmlElementAttribute("responseVariable", typeof(ResponseVariableType))]
        [System.Xml.Serialization.XmlElementAttribute("templateVariable", typeof(TemplateVariableType))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="normalizedString")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime datestamp {
            get {
                return this.datestampField;
            }
            set {
                this.datestampField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="OutcomeVariable.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class OutcomeVariableType {
        
        private ValueType[] valueField;
        
        private string identifierField;
        
        private OutcomeVariableTypeCardinality cardinalityField;
        
        private OutcomeVariableTypeBaseType baseTypeField;
        
        private bool baseTypeFieldSpecified;
        
        private ViewType[] viewField;
        
        private string interpretationField;
        
        private string longInterpretationField;
        
        private double normalMaximumField;
        
        private bool normalMaximumFieldSpecified;
        
        private double normalMinimumField;
        
        private bool normalMinimumFieldSpecified;
        
        private double masteryValueField;
        
        private bool masteryValueFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public ValueType[] value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public OutcomeVariableTypeCardinality cardinality {
            get {
                return this.cardinalityField;
            }
            set {
                this.cardinalityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public OutcomeVariableTypeBaseType baseType {
            get {
                return this.baseTypeField;
            }
            set {
                this.baseTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool baseTypeSpecified {
            get {
                return this.baseTypeFieldSpecified;
            }
            set {
                this.baseTypeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ViewType[] view {
            get {
                return this.viewField;
            }
            set {
                this.viewField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string interpretation {
            get {
                return this.interpretationField;
            }
            set {
                this.interpretationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string longInterpretation {
            get {
                return this.longInterpretationField;
            }
            set {
                this.longInterpretationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double normalMaximum {
            get {
                return this.normalMaximumField;
            }
            set {
                this.normalMaximumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool normalMaximumSpecified {
            get {
                return this.normalMaximumFieldSpecified;
            }
            set {
                this.normalMaximumFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double normalMinimum {
            get {
                return this.normalMinimumField;
            }
            set {
                this.normalMinimumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool normalMinimumSpecified {
            get {
                return this.normalMinimumFieldSpecified;
            }
            set {
                this.normalMinimumFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double masteryValue {
            get {
                return this.masteryValueField;
            }
            set {
                this.masteryValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool masteryValueSpecified {
            get {
                return this.masteryValueFieldSpecified;
            }
            set {
                this.masteryValueFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Value.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class ValueType {
        
        private string fieldIdentifierField;
        
        private ValueTypeBaseType baseTypeField;
        
        private bool baseTypeFieldSpecified;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string fieldIdentifier {
            get {
                return this.fieldIdentifierField;
            }
            set {
                this.fieldIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ValueTypeBaseType baseType {
            get {
                return this.baseTypeField;
            }
            set {
                this.baseTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool baseTypeSpecified {
            get {
                return this.baseTypeFieldSpecified;
            }
            set {
                this.baseTypeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType="normalizedString")]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum ValueTypeBaseType {
        
        /// <remarks/>
        boolean,
        
        /// <remarks/>
        directedPair,
        
        /// <remarks/>
        duration,
        
        /// <remarks/>
        file,
        
        /// <remarks/>
        @float,
        
        /// <remarks/>
        identifier,
        
        /// <remarks/>
        integer,
        
        /// <remarks/>
        pair,
        
        /// <remarks/>
        point,
        
        /// <remarks/>
        @string,
        
        /// <remarks/>
        uri,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum OutcomeVariableTypeCardinality {
        
        /// <remarks/>
        multiple,
        
        /// <remarks/>
        ordered,
        
        /// <remarks/>
        record,
        
        /// <remarks/>
        single,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum OutcomeVariableTypeBaseType {
        
        /// <remarks/>
        boolean,
        
        /// <remarks/>
        directedPair,
        
        /// <remarks/>
        duration,
        
        /// <remarks/>
        file,
        
        /// <remarks/>
        @float,
        
        /// <remarks/>
        identifier,
        
        /// <remarks/>
        integer,
        
        /// <remarks/>
        pair,
        
        /// <remarks/>
        point,
        
        /// <remarks/>
        @string,
        
        /// <remarks/>
        uri,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="View.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum ViewType {
        
        /// <remarks/>
        author,
        
        /// <remarks/>
        candidate,
        
        /// <remarks/>
        proctor,
        
        /// <remarks/>
        scorer,
        
        /// <remarks/>
        testConstructor,
        
        /// <remarks/>
        tutor,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="ResponseVariable.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class ResponseVariableType {
        
        private CorrectResponseType correctResponseField;
        
        private ValueType[] candidateResponseField;
        
        private string identifierField;
        
        private ResponseVariableTypeCardinality cardinalityField;
        
        private ResponseVariableTypeBaseType baseTypeField;
        
        private bool baseTypeFieldSpecified;
        
        private string[] choiceSequenceField;
        
        /// <remarks/>
        public CorrectResponseType correctResponse {
            get {
                return this.correctResponseField;
            }
            set {
                this.correctResponseField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("value", IsNullable=false)]
        public ValueType[] candidateResponse {
            get {
                return this.candidateResponseField;
            }
            set {
                this.candidateResponseField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ResponseVariableTypeCardinality cardinality {
            get {
                return this.cardinalityField;
            }
            set {
                this.cardinalityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ResponseVariableTypeBaseType baseType {
            get {
                return this.baseTypeField;
            }
            set {
                this.baseTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool baseTypeSpecified {
            get {
                return this.baseTypeFieldSpecified;
            }
            set {
                this.baseTypeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string[] choiceSequence {
            get {
                return this.choiceSequenceField;
            }
            set {
                this.choiceSequenceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="CorrectResponse.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class CorrectResponseType {
        
        private ValueType[] valueField;
        
        private string interpretationField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public ValueType[] value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string interpretation {
            get {
                return this.interpretationField;
            }
            set {
                this.interpretationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum ResponseVariableTypeCardinality {
        
        /// <remarks/>
        multiple,
        
        /// <remarks/>
        ordered,
        
        /// <remarks/>
        record,
        
        /// <remarks/>
        single,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum ResponseVariableTypeBaseType {
        
        /// <remarks/>
        boolean,
        
        /// <remarks/>
        directedPair,
        
        /// <remarks/>
        duration,
        
        /// <remarks/>
        file,
        
        /// <remarks/>
        @float,
        
        /// <remarks/>
        identifier,
        
        /// <remarks/>
        integer,
        
        /// <remarks/>
        pair,
        
        /// <remarks/>
        point,
        
        /// <remarks/>
        @string,
        
        /// <remarks/>
        uri,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="TemplateVariable.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class TemplateVariableType {
        
        private ValueType[] valueField;
        
        private string identifierField;
        
        private TemplateVariableTypeCardinality cardinalityField;
        
        private TemplateVariableTypeBaseType baseTypeField;
        
        private bool baseTypeFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public ValueType[] value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TemplateVariableTypeCardinality cardinality {
            get {
                return this.cardinalityField;
            }
            set {
                this.cardinalityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TemplateVariableTypeBaseType baseType {
            get {
                return this.baseTypeField;
            }
            set {
                this.baseTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool baseTypeSpecified {
            get {
                return this.baseTypeFieldSpecified;
            }
            set {
                this.baseTypeFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum TemplateVariableTypeCardinality {
        
        /// <remarks/>
        multiple,
        
        /// <remarks/>
        ordered,
        
        /// <remarks/>
        record,
        
        /// <remarks/>
        single,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum TemplateVariableTypeBaseType {
        
        /// <remarks/>
        boolean,
        
        /// <remarks/>
        directedPair,
        
        /// <remarks/>
        duration,
        
        /// <remarks/>
        file,
        
        /// <remarks/>
        @float,
        
        /// <remarks/>
        identifier,
        
        /// <remarks/>
        integer,
        
        /// <remarks/>
        pair,
        
        /// <remarks/>
        point,
        
        /// <remarks/>
        @string,
        
        /// <remarks/>
        uri,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="ItemResult.Type", Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public partial class ItemResultType {
        
        private object[] itemsField;
        
        private string candidateCommentField;
        
        private string identifierField;
        
        private string sequenceIndexField;
        
        private System.DateTime datestampField;
        
        private ItemResultTypeSessionStatus sessionStatusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("outcomeVariable", typeof(OutcomeVariableType))]
        [System.Xml.Serialization.XmlElementAttribute("responseVariable", typeof(ResponseVariableType))]
        [System.Xml.Serialization.XmlElementAttribute("templateVariable", typeof(TemplateVariableType))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        public string candidateComment {
            get {
                return this.candidateCommentField;
            }
            set {
                this.candidateCommentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="normalizedString")]
        public string identifier {
            get {
                return this.identifierField;
            }
            set {
                this.identifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        public string sequenceIndex {
            get {
                return this.sequenceIndexField;
            }
            set {
                this.sequenceIndexField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime datestamp {
            get {
                return this.datestampField;
            }
            set {
                this.datestampField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ItemResultTypeSessionStatus sessionStatus {
            get {
                return this.sessionStatusField;
            }
            set {
                this.sessionStatusField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.imsglobal.org/xsd/imsqti_result_v2p1")]
    public enum ItemResultTypeSessionStatus {
        
        /// <remarks/>
        final,
        
        /// <remarks/>
        initial,
        
        /// <remarks/>
        pendingResponseProcessing,
        
        /// <remarks/>
        pendingSubmission,
    }
}
