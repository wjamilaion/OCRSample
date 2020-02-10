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
    class DIFCTradeParser : TradeLicenseParser
    {
        public override string DateFormat => "dd/MM/yyyy";

        public override string DateSearchRegex => @"(.*)([0-9]{2,2}).([0-9]{2,2}).([0-9]{4,4})$";

        public override string DateFormatReplaceRegex => @"$2/$3/$4";

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
        protected override DateTime? IssueDate(List<LineData> lines)
        {
            DateTime? issueDate = null;
            string no = string.Empty;
            int i = 0, maxLinesExplore = 6;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "(1|I|l)ssue Date.*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, DateSearchRegex, RegexOptions.IgnoreCase))
                {
                    no = lines[i].FilterWithConfidenceScore();
                    no = no.Substring(Math.Max(0, no.Length - 10));
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            if (!string.IsNullOrEmpty(no))
            {

                try
                {
                    string format = DateFormat;
                    no = no.Trim();
                    no = Regex.Replace(no, DateSearchRegex, DateFormatReplaceRegex);
                    issueDate = DateTime.ParseExact(no, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
                catch { }
            }
            return issueDate;
        }
        protected override DateTime? ExpireDate(List<LineData> lines)
        {
            DateTime? expireDate = null;
            string no = string.Empty;
            int i = 0, maxLinesExplore = 6;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "(Expiry Date|valid till).*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, DateSearchRegex, RegexOptions.IgnoreCase))
                {
                    no = lines[i].FilterWithConfidenceScore();
                    no = no.Substring(Math.Max(0, no.Length - 10));
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            if (!string.IsNullOrEmpty(no))
            {
                try
                {
                    string format = DateFormat;
                    no = no.Trim();
                    no = Regex.Replace(no, DateSearchRegex, DateFormatReplaceRegex);
                    expireDate = DateTime.ParseExact(no, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
                catch { }
            }
            return expireDate;
        }
        protected override string EntityName(List<LineData> lines)
        {
            string no = string.Empty;
            string regexExpression = ".*(Trade.Name|Licensee|operating.name|company.name).*";
            int i = 0, lineFound = -1;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, regexExpression, RegexOptions.IgnoreCase))
                {
                    lineFound = i;
                    break;
                }
            }
            if (lineFound != -1 && lineFound < lines.Count)
            {
                no = lines[lineFound].FilterWithConfidenceScore();
                int valueIndex = no.IndexOf(':') + 1;
                if (valueIndex >= 0 && valueIndex < no.Length)
                    no = no.Substring(valueIndex);
                no = no.Trim();
            }
            return no;
        }
        protected override string LicenseNo(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 4;
            string regexExpression = "(License Number)(.*)(..[0-9]{4,7})$";
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
                    no = Regex.Replace(no, regexExpression, "$3");
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            return no;
        }
    }
}
