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
    class FujairahFZTradeParser : TradeLicenseParser
    {
        public override string DateFormat => "dd/MM/yyyy";

        public override string DateSearchRegex => @"^(.*)([0-9]{2,2}).([0-9]{2,2}).([0-9]{4,4})$";

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
        protected override string PhoneNumber(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 6;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "Telephone.*", RegexOptions.IgnoreCase))
                {
                    while (i >= 0 && maxLinesExplore > 0)
                    {
                        data = lines[i].LineWords.Trim().Replace(".", "");
                        if (Regex.IsMatch(data, "^\\(?([0-9]{3})?\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3,4})$", RegexOptions.IgnoreCase))
                        {
                            no = lines[i].FilterWithConfidenceScore();
                            break;
                        }
                        maxLinesExplore--;
                        i--;
                    }
                    break;
                }
            }
            
            return no;
        }
        protected override DateTime? IssueDate(List<LineData> lines)
        {
            DateTime? issueDate = null;
            string no = string.Empty;
            int i = 0, maxLinesExplore = 6;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "(Date).*((1|I|l)ssue)", RegexOptions.IgnoreCase))
                {
                    while (i >= 0 && maxLinesExplore > 0)
                    {
                        data = lines[i].LineWords.Trim();
                        if (Regex.IsMatch(data, DateSearchRegex, RegexOptions.IgnoreCase))
                        {
                            no = lines[i].FilterWithConfidenceScore();
                            break;
                        }
                        maxLinesExplore--;
                        i--;
                    }
                    break;
                }
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
                if (Regex.IsMatch(data, "(Date.*Expiry|Expiry Date|Expiny Date|valid till).*", RegexOptions.IgnoreCase))
                {
                    i-=2;
                    while (i >= 0 && maxLinesExplore > 0)
                    {
                        data = lines[i].LineWords.Trim();
                        if (Regex.IsMatch(data, DateSearchRegex, RegexOptions.IgnoreCase))
                        {
                            no = lines[i].FilterWithConfidenceScore();
                            break;
                        }
                        maxLinesExplore--;
                        i--;
                    }
                    break;
                }
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
        protected override string LicenseNo(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 4;
            string regexExpression = "(.*)([0-9]{4,7})$";
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "License.No.*", RegexOptions.IgnoreCase))
                {
                    while (i >= 0 && maxLinesExplore > 0)
                    {
                        data = lines[i].LineWords.Trim();
                        if (Regex.IsMatch(data, regexExpression, RegexOptions.IgnoreCase))
                        {
                            no = lines[i].FilterWithConfidenceScore();
                            no = Regex.Replace(no, regexExpression, "$2");
                            break;
                        }
                        maxLinesExplore--;
                        i--;
                    }
                    break;
                }
            }
            
            return no;
        }
    }
}
