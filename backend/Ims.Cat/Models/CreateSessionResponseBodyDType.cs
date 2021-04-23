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
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Ims.Cat.Models
{ 
    /// <summary>
    /// The response code for when the new Session has been created in the CAT Engine and the associated identifier has been returned. The response message consists of the identifier allocated to the session, the initial set of Items to be presented and the session state to be used for later requests. 
    /// </summary>
    [DataContract]
    public partial class CreateSessionResponseBodyDType : IEquatable<CreateSessionResponseBodyDType>
    { 
        /// <summary>
        /// Model Primitive Datatype &#x3D; NCName.
        /// </summary>
        /// <value>Model Primitive Datatype &#x3D; NCName.</value>
        [DataMember(Name="sessionIdentifier")]
        public string SessionIdentifier { get; set; }

        /// <summary>
        /// Gets or Sets NextItems
        /// </summary>
        [DataMember(Name="nextItems")]
        public NextItemSetDType NextItems { get; set; }

        /// <summary>
        /// Model Primitive Datatype &#x3D; NormalizedString.
        /// </summary>
        /// <value>Model Primitive Datatype &#x3D; NormalizedString.</value>
        [DataMember(Name="sessionState")]
        public string SessionState { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CreateSessionResponseBodyDType {\n");
            sb.Append("  SessionIdentifier: ").Append(SessionIdentifier).Append("\n");
            sb.Append("  NextItems: ").Append(NextItems).Append("\n");
            sb.Append("  SessionState: ").Append(SessionState).Append("\n");
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
            return obj.GetType() == GetType() && Equals((CreateSessionResponseBodyDType)obj);
        }

        /// <summary>
        /// Returns true if CreateSessionResponseBodyDType instances are equal
        /// </summary>
        /// <param name="other">Instance of CreateSessionResponseBodyDType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CreateSessionResponseBodyDType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    SessionIdentifier == other.SessionIdentifier ||
                    SessionIdentifier != null &&
                    SessionIdentifier.Equals(other.SessionIdentifier)
                ) && 
                (
                    NextItems == other.NextItems ||
                    NextItems != null &&
                    NextItems.Equals(other.NextItems)
                ) && 
                (
                    SessionState == other.SessionState ||
                    SessionState != null &&
                    SessionState.Equals(other.SessionState)
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
                    if (SessionIdentifier != null)
                    hashCode = hashCode * 59 + SessionIdentifier.GetHashCode();
                    if (NextItems != null)
                    hashCode = hashCode * 59 + NextItems.GetHashCode();
                    if (SessionState != null)
                    hashCode = hashCode * 59 + SessionState.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(CreateSessionResponseBodyDType left, CreateSessionResponseBodyDType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreateSessionResponseBodyDType left, CreateSessionResponseBodyDType right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
