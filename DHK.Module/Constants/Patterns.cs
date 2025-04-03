using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKH.Module.Constants
{
    public static class Patterns
    {
        public const string EMAIL_REGEX = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
        public const string PHONE_EDIT_MASK = "(000) 000-0000";
        public const string PHONE_DISPLAY_FORMAT = "{0:(000) 000-0000}";
        public const string TWILIO_PHONE_EDIT_MASK = "+00000000000";
        public const string LONG_LAT_DISPLAY_FORMAT = "0.0000000";
        public const string LONG_LAT_EDIT_MASK = "N7";
        public const string TWENTY_FOUR_HOUR_FORMAT = "HH:mm";
        public const string TWELVE_HOUR_FORMAT = "hh:mm tt";
        public const string TWELVE_HOUR_FORMAT_WITH_SECONDS = @"hh\:mm\:ss";
        public const string TRACKING_INPUTS = "abcdefghijklmnopqrstuvwxyz0123456789";
        public const string SEMI_COLON = ";";
        public const string NUMBERS = "0123456789";
        public const string GENERAL_SHORT_DATE_TIME_MASK = "g";
        public const string GENERAL_SHORT_DATE_TIME_FORMAT = "{0:g}";
        public const string FORMATTED_PARENTHESIS = "{0} ({1})";
        public const string GENERAL_SHORT_DATE_TIME_FORMAT_UTC_POSTFIX = "{0:g} (UTC)";
        public const string AZURE_SECRET_PATTERN = @"[^a-zA-Z0-9-_]";
        public const string PASSWORD_PATTERN = "*";
        public const string REGEX = "RegEx";
        public const string ALPHANUMERIC = "[A-Z0-9]*";
        public const string LOWER_ALPHANUMERIC = "[a-zA-Z0-9 ]*";
        public const string DECIMAL = "{0:N}";
        public const string ZERO = "0";
        public const string PERCENTAGE = "{0}%";
        public const string NON_ZERO_TWELVE_HOUR_FORMAT = "h:mm tt";
        public const string DATE_FORMAT = "M/d/yyyy";
        public const string ZERO_HOUR_ZERO_MINUTE = "0h 0m";
        public const string ALPHA_NUMERIC_STARTING_WITH_LETTER = @"^[a-zA-Z][a-zA-Z0-9]*$";
        public const string NUMBER_PERCENTAGE = "{0:n0}%";
        public const string PERCENTAGE_RATE = "P0";
        public const string NUMBER_FORMAT = "{0:N}";
        public const string NUMBER = "N";
        public const string COMMA_SPACE = ", ";
        public const string COMMA = ",";
        public const string NUMERIC_FORMAT_WITH_LEADING_ZEROS = "##0";
        public const string TWELVE_HOUR_PARENTHESIS_FORMAT = "h:mm tt '({0})'";
        public const string TWENTY_FOUR_HOUR_PARENTHESIS_FORMAT = "HH:mm '({0})'";
        public const string DASH = "-";
        public const string PERIOD = ".";
        public const string DAY_OF_THE_WEEK = "dddd";
        public const string FULL_NAME_MONTH = "MMMM";
        public const string DATE = "dd";
        public const string CONVERSION = "N4";
        public const string CONVERSIONFORMAT = "{0:N4}";
        public const string NUMBER_NO_DECIMAL_PLACES = "{0:N0}";
        public const string ROUND_OFF = "d0";
    }
}
