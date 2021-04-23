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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Ims.Cat.Models
{ 
    /// <summary>
    /// A set of Item identfiers. 
    /// </summary>
    [DataContract]
    public partial class ItemSetDType : IEquatable<ItemSetDType>
    { 
        /// <summary>
        /// The Identifiers for the set of Items. These should be the GUIDs that have been assigned to AssessmentItems when defined using IMS QTI. Model Primitive Datatype &#x3D; NCName. 
        /// </summary>
        /// <value>The Identifiers for the set of Items. These should be the GUIDs that have been assigned to AssessmentItems when defined using IMS QTI. Model Primitive Datatype &#x3D; NCName. </value>
        [Required]
        [DataMember(Name="itemIdentifiers")]
        public List<string> ItemIdentifiers { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ItemSetDType {\n");
            sb.Append("  ItemIdentifiers: ").Append(ItemIdentifiers).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ItemSetDType)obj);
        }

        /// <summary>
        /// Returns true if ItemSetDType instances are equal
        /// </summary>
        /// <param name="other">Instance of ItemSetDType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ItemSetDType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ItemIdentifiers == other.ItemIdentifiers ||
                    ItemIdentifiers != null &&
                    ItemIdentifiers.SequenceEqual(other.ItemIdentifiers)
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
                    if (ItemIdentifiers != null)
                    hashCode = hashCode * 59 + ItemIdentifiers.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(ItemSetDType left, ItemSetDType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ItemSetDType left, ItemSetDType right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}