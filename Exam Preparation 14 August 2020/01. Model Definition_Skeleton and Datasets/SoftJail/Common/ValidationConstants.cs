using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.Common
{
    public static class ValidationConstants
    {
        //Prisoner
        public const int PrisonerFullNameMinLength = 3;
        public const int PrisonerFullNameMaxLength = 20;
        public const string PrisonerNicknameRegex = @"^(The\s)([A-Z][a-z]*)$";
        public const int PrisonerAgeMinValue = 18;
        public const int PrisonerAgeMaxValue = 65;
        public const string PrisonerBailMinValue = "0";
        public const string PrisonerBailMaxValue = "79228162514264337593543950335";

        //Officer
        public const int OfficerFullNameMinLength = 3;
        public const int OfficerFullNameMaxLength = 30;


        //Mail
        public const string MailAddressRegex = @"^([A-za-z\s0-9]+?)(\sstr\.)$";

        //Department
        public const int DepartmenrNameMinLength = 3;
        public const int DepartmenrNameMaxLength = 25;

        //Cell
        public const int CellNumberMinValue = 1;
        public const int CellNumberMaxValue = 1000;
    }
}
