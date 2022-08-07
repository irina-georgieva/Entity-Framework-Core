using System;
using System.Collections.Generic;
using System.Text;

namespace Footballers.Common
{
    public class ValidationConstants
    {
        //Footballer
        public const int FootballerNameMinLength = 2;
        public const int FootballerNameMaxLength = 40;

        //Team
        public const int TeamNameMinLength = 2;
        public const int TeamNameMaxLength = 40;
        public const string TeamNameRegex = @"^([A-Za-z0-9\s\.\-]*)$";

        public const int TeamNationalityMinLength = 2;
        public const int TeamNationalityMaxLength = 40;

        //Coach
        public const int CoachNameMinLength = 2;
        public const int CoachNameMaxLength = 40;
    }
}
