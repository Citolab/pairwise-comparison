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
    /// This is the container for the session configuration data that must be supplied to the CAT Engine when the session is to be created. 
    /// </summary>
    [DataContract]
    public partial class SessionDType : IEquatable<SessionDType>
    { 
        /// <summary>
        /// This is the Base64 encoded XML data for the Personal Needs and Preferences (PNP) for the examinee.  This data conforms to the XML as defined in the IMS Access for All Personal Needs and Preferences (AfAPNP) 3.0 XML Binding specification [AFAPNP, 20]. Model Primitive Datatype &#x3D; NormalizedString.
        /// </summary>
        /// <value>This is the Base64 encoded XML data for the Personal Needs and Preferences (PNP) for the examinee.  This data conforms to the XML as defined in the IMS Access for All Personal Needs and Preferences (AfAPNP) 3.0 XML Binding specification [AFAPNP, 20]. Model Primitive Datatype &#x3D; NormalizedString.</value>
        [DataMember(Name="personalNeedsAndPreferences")]
        public string PersonalNeedsAndPreferences { get; set; }

        /// <summary>
        /// The Base64 encoding of the JSON data for the Demographics information for the examinee. This data conforms to the JSON as defined in the IMS OneRoster 1.1 specification [OR, 17]. Model Primitive Datatype &#x3D; NormalizedString.
        /// </summary>
        /// <value>The Base64 encoding of the JSON data for the Demographics information for the examinee. This data conforms to the JSON as defined in the IMS OneRoster 1.1 specification [OR, 17]. Model Primitive Datatype &#x3D; NormalizedString.</value>
        [DataMember(Name="demographics")]
        public string Demographics { get; set; }

        /// <summary>
        /// This is proprietary data that is supplied to the CAT Engine as a key/value pairs. 
        /// </summary>
        /// <value>This is proprietary data that is supplied to the CAT Engine as a key/value pairs. </value>
        [DataMember(Name="priorData")]
        public List<KeyValuePairDType> PriorData { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SessionDType {\n");
            sb.Append("  PersonalNeedsAndPreferences: ").Append(PersonalNeedsAndPreferences).Append("\n");
            sb.Append("  Demographics: ").Append(Demographics).Append("\n");
            sb.Append("  PriorData: ").Append(PriorData).Append("\n");
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
            return obj.GetType() == GetType() && Equals((SessionDType)obj);
        }

        /// <summary>
        /// Returns true if SessionDType instances are equal
        /// </summary>
        /// <param name="other">Instance of SessionDType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SessionDType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    PersonalNeedsAndPreferences == other.PersonalNeedsAndPreferences ||
                    PersonalNeedsAndPreferences != null &&
                    PersonalNeedsAndPreferences.Equals(other.PersonalNeedsAndPreferences)
                ) && 
                (
                    Demographics == other.Demographics ||
                    Demographics != null &&
                    Demographics.Equals(other.Demographics)
                ) && 
                (
                    PriorData == other.PriorData ||
                    PriorData != null &&
                    PriorData.SequenceEqual(other.PriorData)
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
                    if (PersonalNeedsAndPreferences != null)
                    hashCode = hashCode * 59 + PersonalNeedsAndPreferences.GetHashCode();
                    if (Demographics != null)
                    hashCode = hashCode * 59 + Demographics.GetHashCode();
                    if (PriorData != null)
                    hashCode = hashCode * 59 + PriorData.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(SessionDType left, SessionDType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SessionDType left, SessionDType right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
