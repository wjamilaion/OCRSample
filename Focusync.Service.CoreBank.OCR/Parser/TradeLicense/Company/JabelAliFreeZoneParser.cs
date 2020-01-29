using Aquaforest.ExtendedOCR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradeLicense.Model;

namespace TradeLicense
{
    public class JabelAliFreeZoneParser : TradeLicenseParser
    {
        public override string DateFormat => "dd/MM/yyyy";

        public override string DateSearchRegex => @"^([0-9]{2,2}).([0-9]{2,2}).([0-9]{4,4})$";

        public override string DateFormatReplaceRegex => @"$1/$2/$3";

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
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "License.No.*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "[0-9]{4,7}$", RegexOptions.IgnoreCase))
                {
                    no = lines[i].FilterWithConfidenceScore();
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            return no;
        }
    }
}
