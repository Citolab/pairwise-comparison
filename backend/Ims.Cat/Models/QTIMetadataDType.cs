/*
 * Computer Adaptive Testing (CAT) Service OpenAPI (YAML) Definition
 *
 * The Computer Adaptive Testing (CAT) Service enables a standard way of implementing adaptive testing using Question and Test Interoperability (QTI). This service has been described using the IMS Model Driven Specification development approach, this being the Platform Specific Model (PSM) of the service.
 *
 * OpenAPI spec version: 1.0
 * Contact: lmattson@imsglobal.org
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Ims.Cat.Models
{ 
    /// <summary>
    /// This contains the new category of metadata for the recording of QTI specific information. 
    /// </summary>
    [DataContract]
    public partial class QTIMetadataDType : IEquatable<QTIMetadataDType>
    { 
        /// <summary>
        /// True if the item is actually an item template, in other words, the item changes its appearance based on some random or external factor. An assessmentItem that contains a templateProcessing section. Model Primitive Datatype &#x3D; Boolean.
        /// </summary>
        /// <value>True if the item is actually an item template, in other words, the item changes its appearance based on some random or external factor. An assessmentItem that contains a templateProcessing section. Model Primitive Datatype &#x3D; Boolean.</value>
        [DataMember(Name="itemTemplate")]
        public bool? ItemTemplate { get; set; }

        /// <summary>
        /// Whether or not the item is time dependent. A time dependent item takes the length of time taken for an attempt into consideration when scoring. Model Primitive Datatype &#x3D; Boolean.
        /// </summary>
        /// <value>Whether or not the item is time dependent. A time dependent item takes the length of time taken for an attempt into consideration when scoring. Model Primitive Datatype &#x3D; Boolean.</value>
        [DataMember(Name="timeDependent")]
        public bool? TimeDependent { get; set; }

        /// <summary>
        /// True if the item comprises more than one interaction, for example, an assessmentItem that contains more than one interaction. Model Primitive Datatype &#x3D; Boolean.
        /// </summary>
        /// <value>True if the item comprises more than one interaction, for example, an assessmentItem that contains more than one interaction. Model Primitive Datatype &#x3D; Boolean.</value>
        [DataMember(Name="composite")]
        public bool? Composite { get; set; }

        /// <summary>
        /// Gets or Sets InteractionType
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum InteractionTypeEnum
        {
            /// <summary>
            /// Enum AssociateInteractionEnum for associateInteraction
            /// </summary>
            [EnumMember(Value = "associateInteraction")]
            AssociateInteractionEnum = 0,
            /// <summary>
            /// Enum ChoiceInteractionEnum for choiceInteraction
            /// </summary>
            [EnumMember(Value = "choiceInteraction")]
            ChoiceInteractionEnum = 1,
            /// <summary>
            /// Enum CustomInteractionEnum for customInteraction
            /// </summary>
            [EnumMember(Value = "customInteraction")]
            CustomInteractionEnum = 2,
            /// <summary>
            /// Enum DrawingInteractionEnum for drawingInteraction
            /// </summary>
            [EnumMember(Value = "drawingInteraction")]
            DrawingInteractionEnum = 3,
            /// <summary>
            /// Enum EndAttemptInteractionEnum for endAttemptInteraction
            /// </summary>
            [EnumMember(Value = "endAttemptInteraction")]
            EndAttemptInteractionEnum = 4,
            /// <summary>
            /// Enum ExtendedTextInteractionEnum for extendedTextInteraction
            /// </summary>
            [EnumMember(Value = "extendedTextInteraction")]
            ExtendedTextInteractionEnum = 5,
            /// <summary>
            /// Enum GapMatchInteractionEnum for gapMatchInteraction
            /// </summary>
            [EnumMember(Value = "gapMatchInteraction")]
            GapMatchInteractionEnum = 6,
            /// <summary>
            /// Enum GraphicAssociateInteractionEnum for graphicAssociateInteraction
            /// </summary>
            [EnumMember(Value = "graphicAssociateInteraction")]
            GraphicAssociateInteractionEnum = 7,
            /// <summary>
            /// Enum GraphicGapMatchInteractionEnum for graphicGapMatchInteraction
            /// </summary>
            [EnumMember(Value = "graphicGapMatchInteraction")]
            GraphicGapMatchInteractionEnum = 8,
            /// <summary>
            /// Enum GraphicOrderInteractionEnum for graphicOrderInteraction
            /// </summary>
            [EnumMember(Value = "graphicOrderInteraction")]
            GraphicOrderInteractionEnum = 9,
            /// <summary>
            /// Enum HotspotInteractionEnum for hotspotInteraction
            /// </summary>
            [EnumMember(Value = "hotspotInteraction")]
            HotspotInteractionEnum = 10,
            /// <summary>
            /// Enum HottextInteractionEnum for hottextInteraction
            /// </summary>
            [EnumMember(Value = "hottextInteraction")]
            HottextInteractionEnum = 11,
            /// <summary>
            /// Enum InlineChoiceInteractionEnum for inlineChoiceInteraction
            /// </summary>
            [EnumMember(Value = "inlineChoiceInteraction")]
            InlineChoiceInteractionEnum = 12,
            /// <summary>
            /// Enum MatchInteractionEnum for matchInteraction
            /// </summary>
            [EnumMember(Value = "matchInteraction")]
            MatchInteractionEnum = 13,
            /// <summary>
            /// Enum MediaInteractionEnum for mediaInteraction
            /// </summary>
            [EnumMember(Value = "mediaInteraction")]
            MediaInteractionEnum = 14,
            /// <summary>
            /// Enum OrderInteractionEnum for orderInteraction
            /// </summary>
            [EnumMember(Value = "orderInteraction")]
            OrderInteractionEnum = 15,
            /// <summary>
            /// Enum PortableCustomInteractionEnum for portableCustomInteraction
            /// </summary>
            [EnumMember(Value = "portableCustomInteraction")]
            PortableCustomInteractionEnum = 16,
            /// <summary>
            /// Enum PositionObjectInteractionEnum for positionObjectInteraction
            /// </summary>
            [EnumMember(Value = "positionObjectInteraction")]
            PositionObjectInteractionEnum = 17,
            /// <summary>
            /// Enum SelectPointInteractionEnum for selectPointInteraction
            /// </summary>
            [EnumMember(Value = "selectPointInteraction")]
            SelectPointInteractionEnum = 18,
            /// <summary>
            /// Enum SliderInteractionEnum for sliderInteraction
            /// </summary>
            [EnumMember(Value = "sliderInteraction")]
            SliderInteractionEnum = 19,
            /// <summary>
            /// Enum TextEntryInteractionEnum for textEntryInteraction
            /// </summary>
            [EnumMember(Value = "textEntryInteraction")]
            TextEntryInteractionEnum = 20,
            /// <summary>
            /// Enum UploadInteractionEnum for uploadInteraction
            /// </summary>
            [EnumMember(Value = "uploadInteraction")]
            UploadInteractionEnum = 21        }

        /// <summary>
        /// The interaction type(s) of the item. The vocabulary is comprised of the names, as defined in the information model, of the leaf classes derived from interaction. 
        /// </summary>
        /// <value>The interaction type(s) of the item. The vocabulary is comprised of the names, as defined in the information model, of the leaf classes derived from interaction. </value>
        [DataMember(Name="interactionType")]
        public List<InteractionTypeEnum> InteractionType { get; set; }

        /// <summary>
        /// Gets or Sets PortableCustomInteractionContext
        /// </summary>
        [DataMember(Name="portableCustomInteractionContext")]
        public PCIContextDType PortableCustomInteractionContext { get; set; }

        /// <summary>
        /// Describes the type of feedback, if any, available in the item. If feedback is available then it is described as being non-adaptive or adaptive depending on whether the item is itself adaptive. A non-adaptive item generates feedback based on the responses submitted as part of (the last) attempt only. An adaptive item generates feedback that takes into consideration the path taken through the item, in other words, feedback based on the accumulation of all attempts and not just the last. 
        /// </summary>
        /// <value>Describes the type of feedback, if any, available in the item. If feedback is available then it is described as being non-adaptive or adaptive depending on whether the item is itself adaptive. A non-adaptive item generates feedback based on the responses submitted as part of (the last) attempt only. An adaptive item generates feedback that takes into consideration the path taken through the item, in other words, feedback based on the accumulation of all attempts and not just the last. </value>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum FeedbackTypeEnum
        {
            /// <summary>
            /// Enum AdaptiveEnum for adaptive
            /// </summary>
            [EnumMember(Value = "adaptive")]
            AdaptiveEnum = 0,
            /// <summary>
            /// Enum NonadaptiveEnum for nonadaptive
            /// </summary>
            [EnumMember(Value = "nonadaptive")]
            NonadaptiveEnum = 1,
            /// <summary>
            /// Enum NoneEnum for none
            /// </summary>
            [EnumMember(Value = "none")]
            NoneEnum = 2        }

        /// <summary>
        /// Describes the type of feedback, if any, available in the item. If feedback is available then it is described as being non-adaptive or adaptive depending on whether the item is itself adaptive. A non-adaptive item generates feedback based on the responses submitted as part of (the last) attempt only. An adaptive item generates feedback that takes into consideration the path taken through the item, in other words, feedback based on the accumulation of all attempts and not just the last. 
        /// </summary>
        /// <value>Describes the type of feedback, if any, available in the item. If feedback is available then it is described as being non-adaptive or adaptive depending on whether the item is itself adaptive. A non-adaptive item generates feedback based on the responses submitted as part of (the last) attempt only. An adaptive item generates feedback that takes into consideration the path taken through the item, in other words, feedback based on the accumulation of all attempts and not just the last. </value>
        [DataMember(Name="feedbackType")]
        public FeedbackTypeEnum? FeedbackType { get; set; }

        /// <summary>
        /// Set to true if a model solution is available for the item. For example, an assessmentItem that provides correct responses for all declared response variables. Model Primitive Datatype &#x3D; Boolean.
        /// </summary>
        /// <value>Set to true if a model solution is available for the item. For example, an assessmentItem that provides correct responses for all declared response variables. Model Primitive Datatype &#x3D; Boolean.</value>
        [DataMember(Name="solutionAvailable")]
        public bool? SolutionAvailable { get; set; }

        /// <summary>
        /// Gets or Sets ScoringMode
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum ScoringModeEnum
        {
            /// <summary>
            /// Enum HumanEnum for human
            /// </summary>
            [EnumMember(Value = "human")]
            HumanEnum = 0,
            /// <summary>
            /// Enum ExternalmachineEnum for externalmachine
            /// </summary>
            [EnumMember(Value = "externalmachine")]
            ExternalmachineEnum = 1,
            /// <summary>
            /// Enum ResponseprocessingEnum for responseprocessing
            /// </summary>
            [EnumMember(Value = "responseprocessing")]
            ResponseprocessingEnum = 2        }

        /// <summary>
        /// The scoringMode is used to denote that the way in which the scoring is achieved. If the Item includes the response processing then the mode is &#x27;responseprocessing&#x27;. If human scoring is required the value is &#x27;human&#x27; and if some form of machine processing is required then the value is &#x27;externalmachine&#x27;. 
        /// </summary>
        /// <value>The scoringMode is used to denote that the way in which the scoring is achieved. If the Item includes the response processing then the mode is &#x27;responseprocessing&#x27;. If human scoring is required the value is &#x27;human&#x27; and if some form of machine processing is required then the value is &#x27;externalmachine&#x27;. </value>
        [DataMember(Name="scoringMode")]
        public List<ScoringModeEnum> ScoringMode { get; set; }

        /// <summary>
        /// The name of the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.
        /// </summary>
        /// <value>The name of the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.</value>
        [DataMember(Name="toolName")]
        public string ToolName { get; set; }

        /// <summary>
        /// The version of the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.
        /// </summary>
        /// <value>The version of the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.</value>
        [DataMember(Name="toolVersion")]
        public string ToolVersion { get; set; }

        /// <summary>
        /// The company which produced the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.
        /// </summary>
        /// <value>The company which produced the tool used to author the evaluation object. Model Primitive Datatype &#x3D; String.</value>
        [DataMember(Name="toolVendor")]
        public string ToolVendor { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class QTIMetadataDType {\n");
            sb.Append("  ItemTemplate: ").Append(ItemTemplate).Append("\n");
            sb.Append("  TimeDependent: ").Append(TimeDependent).Append("\n");
            sb.Append("  Composite: ").Append(Composite).Append("\n");
            sb.Append("  InteractionType: ").Append(InteractionType).Append("\n");
            sb.Append("  PortableCustomInteractionContext: ").Append(PortableCustomInteractionContext).Append("\n");
            sb.Append("  FeedbackType: ").Append(FeedbackType).Append("\n");
            sb.Append("  SolutionAvailable: ").Append(SolutionAvailable).Append("\n");
            sb.Append("  ScoringMode: ").Append(ScoringMode).Append("\n");
            sb.Append("  ToolName: ").Append(ToolName).Append("\n");
            sb.Append("  ToolVersion: ").Append(ToolVersion).Append("\n");
            sb.Append("  ToolVendor: ").Append(ToolVendor).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((QTIMetadataDType)obj);
        }

        /// <summary>
        /// Returns true if QTIMetadataDType instances are equal
        /// </summary>
        /// <param name="other">Instance of QTIMetadataDType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(QTIMetadataDType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ItemTemplate == other.ItemTemplate ||
                    ItemTemplate != null &&
                    ItemTemplate.Equals(other.ItemTemplate)
                ) && 
                (
                    TimeDependent == other.TimeDependent ||
                    TimeDependent != null &&
                    TimeDependent.Equals(other.TimeDependent)
                ) && 
                (
                    Composite == other.Composite ||
                    Composite != null &&
                    Composite.Equals(other.Composite)
                ) && 
                (
                    InteractionType == other.InteractionType ||
                    InteractionType != null &&
                    InteractionType.SequenceEqual(other.InteractionType)
                ) && 
                (
                    PortableCustomInteractionContext == other.PortableCustomInteractionContext ||
                    PortableCustomInteractionContext != null &&
                    PortableCustomInteractionContext.Equals(other.PortableCustomInteractionContext)
                ) && 
                (
                    FeedbackType == other.FeedbackType ||
                    FeedbackType != null &&
                    FeedbackType.Equals(other.FeedbackType)
                ) && 
                (
                    SolutionAvailable == other.SolutionAvailable ||
                    SolutionAvailable != null &&
                    SolutionAvailable.Equals(other.SolutionAvailable)
                ) && 
                (
                    ScoringMode == other.ScoringMode ||
                    ScoringMode != null &&
                    ScoringMode.SequenceEqual(other.ScoringMode)
                ) && 
                (
                    ToolName == other.ToolName ||
                    ToolName != null &&
                    ToolName.Equals(other.ToolName)
                ) && 
                (
                    ToolVersion == other.ToolVersion ||
                    ToolVersion != null &&
                    ToolVersion.Equals(other.ToolVersion)
                ) && 
                (
                    ToolVendor == other.ToolVendor ||
                    ToolVendor != null &&
                    ToolVendor.Equals(other.ToolVendor)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (ItemTemplate != null)
                    hashCode = hashCode * 59 + ItemTemplate.GetHashCode();
                    if (TimeDependent != null)
                    hashCode = hashCode * 59 + TimeDependent.GetHashCode();
                    if (Composite != null)
                    hashCode = hashCode * 59 + Composite.GetHashCode();
                    if (InteractionType != null)
                    hashCode = hashCode * 59 + InteractionType.GetHashCode();
                    if (PortableCustomInteractionContext != null)
                    hashCode = hashCode * 59 + PortableCustomInteractionContext.GetHashCode();
                    if (FeedbackType != null)
                    hashCode = hashCode * 59 + FeedbackType.GetHashCode();
                    if (SolutionAvailable != null)
                    hashCode = hashCode * 59 + SolutionAvailable.GetHashCode();
                    if (ScoringMode != null)
                    hashCode = hashCode * 59 + ScoringMode.GetHashCode();
                    if (ToolName != null)
                    hashCode = hashCode * 59 + ToolName.GetHashCode();
                    if (ToolVersion != null)
                    hashCode = hashCode * 59 + ToolVersion.GetHashCode();
                    if (ToolVendor != null)
                    hashCode = hashCode * 59 + ToolVendor.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(QTIMetadataDType left, QTIMetadataDType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QTIMetadataDType left, QTIMetadataDType right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
