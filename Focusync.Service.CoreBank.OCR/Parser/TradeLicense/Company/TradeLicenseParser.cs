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
    public abstract class TradeLicenseParser : ITradeLicense
    {
        public abstract string DateFormat { get; }
        public abstract string DateSearchRegex { get; }
        public abstract string DateFormatReplaceRegex { get; }
        public abstract CompanyModel Parse(List<LineData> lines);
        public abstract PartnerListModel ParsePartner(List<LineData> lines);
        public virtual CompanyModel ParseCompany(List<LineData> lines)
        {
            CompanyModel company = new CompanyModel
            {
                TradeLicenceNumber = LicenseNo(lines),
                EntityName = EntityName(lines),
                EntityType = EntityType(lines),
                TradeLicenceExpiryDate = ExpireDate(lines),
                TradeLicenceIssueDate = IssueDate(lines),
                PhoneNumber = PhoneNumber(lines)
            };
            return company;
        }
        protected virtual string PhoneNumber(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 6;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "Mobile No.*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim().Replace(".", "");
                if (Regex.IsMatch(data, ".*[0-9]{3,3}-[0-9]{2,2}-[0-9]{7,7}.*", RegexOptions.IgnoreCase))
                {
                    no = lines[i].FilterWithConfidenceScore();
                    break;
                }
                maxLinesExplore--;
                i++;
            }
            return no;
        }
        protected virtual DateTime? IssueDate(List<LineData> lines)
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
        protected virtual DateTime? ExpireDate(List<LineData> lines)
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
        protected virtual string EntityType(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 3;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "^Legal (Type|status|Form).*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            Dictionary<string, string> legalTypes = new Dictionary<string, string>();
            legalTypes["LLC"] = @"(.*)(LLC|Limited Liability|Liability Company|Limited Liability Company|LC|Limited.*Company)(.*)";// "Limited Liability Company(LLC)";
            legalTypes["PRT"] = @".*(PRT|Partnership).*";
            legalTypes["SPT"] = @".*(SPT|Sole Propriertorship|Sole|Propriertorship).*";
            legalTypes["FZC"] = @".*(FZC|Free Zone Company|Free|Zone).*";
            legalTypes["OFC"] = @".*(OFC|Offshore Company|Offshore).*";
            legalTypes["BLF"] = @".*(BLF|Branch of Local / Foreign Company|Branch of Local|Foreign Company|Branch.*Local).*";
            legalTypes["FOC"] = @".*(FOC|Foreign Company).*";
            legalTypes["GOV"] = @".*(GOV|Government).*";
            legalTypes["OTH"] = @".*(OTH|Others).*";

            legalTypes["IND"] = @".*Individual.*";
            legalTypes["Establishment"] = @".*Establishment.*";

            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim().Replace(".", "");
                foreach (var item in legalTypes)
                {
                    var exp = new Regex(item.Value);
                    if (exp.IsMatch(data))
                    {
                        return item.Key;
                    }
                }
                maxLinesExplore--;
                i++;
            }
            return no;
        }
        protected virtual string EntityName(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, lineFound = -1;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, ".*(Trade.Name|Licensee|operating.name|company.name).*", RegexOptions.IgnoreCase))
                {
                    lineFound = i + 1;
                    break;
                }
            }
            if (lineFound != -1 && lineFound < lines.Count)
            {
                no = lines[lineFound].FilterWithConfidenceScore();
                int valueIndex = no.IndexOf(':') + 1;
                if(valueIndex >= 0 && valueIndex < no.Length)
                    no = no.Substring(valueIndex);
                no = no.Trim();
            }
            return no;
        }
        protected virtual string LicenseNo(List<LineData> lines)
        {
            string no = string.Empty;
            int i = 0, maxLinesExplore = 4;
            for (i = 0; i < lines.Count; i++)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "License No.*", RegexOptions.IgnoreCase))
                {
                    break;
                }
            }
            while (i < lines.Count && maxLinesExplore > 0)
            {
                string data = lines[i].LineWords.Trim();
                if (Regex.IsMatch(data, "^[0-9]{4,6}$", RegexOptions.IgnoreCase))
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
