using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NGP_Bitmap_2_File.Core.Clases
{
    class Utils
    {
        public static bool IsPar(int number)
        {
            return ((number % 2) == 0);
        }

        public static bool IsMultiploDe(int number1, int number2)
        {
            return (number1 % number2 == 0);
        }

        public static bool IsPath(string value)
        {
            try
            {
                if (value.Equals(string.Empty)) { return false; }

                Regex rx1 = new Regex(@"([a-zA-Z]*:[\\[a-zA-Z0-9 .]*]*)", RegexOptions.Compiled);
                //Regex rx1 = new Regex(@"(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.(txt|gif|pdf|doc|docx|xls|xlsx)", RegexOptions.Compiled);
                //Regex rx2 = new Regex("([0-Z0-z]{6})|([0-Z0-z]{8})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                MatchCollection matches1 = rx1.Matches(value);

                return (matches1.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsRGBColor(string value)
        {
            try
            {
                if (value.Equals(string.Empty)) { return false; }

                Regex rx1 = new Regex(@"([0-9]{1,3}),([0-9]{1,3}),([0-9]{1,3})", RegexOptions.Compiled);

                MatchCollection matches1 = rx1.Matches(value);

                return (matches1.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsNGPPlane(string value)
        {
            try
            {
                if (value.Equals(string.Empty)) { return false; }

                Regex rx1 = new Regex(@"(PS|P1|P2)", RegexOptions.Compiled);

                MatchCollection matches1 = rx1.Matches(value);

                return (matches1.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsOption(string value)
        {
            try
            {
                if (value.Equals(string.Empty)) { return false; }

                Regex rx1 = new Regex(@"(^-)([ipd]{1,3}|[IPD]{1,3})", RegexOptions.Compiled);

                MatchCollection matches1 = rx1.Matches(value);

                return (matches1.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
