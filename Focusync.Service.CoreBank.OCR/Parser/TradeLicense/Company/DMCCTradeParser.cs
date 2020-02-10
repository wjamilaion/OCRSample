using Aquaforest.ExtendedOCR.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradeLicense.Model;

namespace TradeLicense
{
    class DMCCTradeParser : TradeLicenseParser
    {
        public override string DateFormat => "dd-MMM-yyyy";

        public override string DateSearchRegex => @"^([0-9]{2,2})-(jan|feb|mar|apr|may|june|july|aug|sep|oct|nov|dec)(.*)-([0-9]{4,4})$";

        public override string DateFormatReplaceRegex => @"$1 $2$3 $4";

        public override CompanyModel Parse(List<LineData> lines)
        {
            CompanyModel company = ParseCompany(lines);

            //company.Partners = ParsePartner(lines);
            return company;
        }

        public override PartnerListModel ParsePartner(List<LineData> lines)
        {
            throw new NotImplementedException();
        }
        protected override string LicenseNo(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 4;
            string regexExpression = "(.*)(....-[0-9]{4,7})$";
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "License.Number.*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, regexExpression, RegexOptions.IgnoreCase))
                {
                    no = lines[i].FilterWithConfidenceScore();
                    no = Regex.Replace(no, regexExpression, "$2");
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            return no;
        }
    }
}
