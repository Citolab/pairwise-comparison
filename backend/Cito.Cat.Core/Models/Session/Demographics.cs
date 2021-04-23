using System;

namespace Cito.Cat.Core.Models.Session
{
    public class Demographics
    {
        public DateTime? Birthdate { get; set; }

        public string Sex { get; set; }

        public bool? AmericanIndianOrAlaskaNative { get; set; }

        public bool? Asian { get; set; }

        public bool? BlackOrAfricanAmerican { get; set; }

        public bool? NativeHawaiianOrOtherPacificIslander { get; set; }

        public bool? White { get; set; }

        public bool? DemographicRaceTwoOrMoreRaces { get; set; }

        public bool? HispanicOrLatinoEthnicity { get; set; }

        public string CountryofBirthCode { get; set; }

        public string StateOfBirthAbbreviation { get; set; }

        public string CityOfBirth { get; set; }

        public string PublicSchoolResidenceStatus { get; set; }
    }
}
