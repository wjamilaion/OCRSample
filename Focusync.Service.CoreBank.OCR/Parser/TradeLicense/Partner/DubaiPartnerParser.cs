using Aquaforest.ExtendedOCR.Shared;
using Focusync.Infrastructure;
//using Focusync.Common.Enumerators;
using Focusync.Service.CoreBank.OCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradeLicense.Model;

namespace TradeLicense
{
    class DubaiPartnerParser
    {
        private static readonly Nationalities _nationalities = new Nationalities();

        public static string Parse(List<LineData> lines)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ParseObject(lines));
        }
        public static PartnerListModel ParseObject(List<LineData> lines)
        {
            List<string> partners = new List<string>();
            int no = 0;
            Regex exp = new Regex(@".*([0-9]|[0-9][0-9])\.[0-9][0-9][0-9][0-9][0-9].*");
            for (no = lines.Count - 1; no >= 0; no--)
            {
                if (exp.IsMatch(lines[no].LineWords))
                {
                    StringBuilder builder = new StringBuilder();

                    string prec = lines[no].FilterWithConfidenceScore();

                    builder.Append(prec.Substring(0, Math.Min(6, prec.Length))).Append('|');
                    int i = 1;
                    for (i = 1; i < 4 && no + i < lines.Count; i++)
                    {
                        builder.Append(lines[no + i].FilterWithConfidenceScore()).Append('|');
                    }
                    while (i < 4) builder.Append('|');
                    partners.Add(builder.ToString());
                }
            }
            List<PartnerModel> data = new List<PartnerModel>();
            foreach (var item in partners)
            {
                var words = item.Split('|');
                PartnerModel partner = null;
                partner = new PartnerModel
                {
                    Percentage = Percentage(words),
                    Nationality = Nationality(words),
                    SrNo = SrNo(words),
                    Name = Name(words),
                    CompanyType = CompanyType(words),
                };
                partner.Status = Status(partner.CompanyType);
                data.Add(partner);
            }
            return new PartnerListModel { Partners = data };
        }
        public static string Percentage(string[] lines)
        {
            if (lines.Length <= 0) return string.Empty;
            return lines[0];
        }
        public static string SrNo(string[] lines)
        {
            if (lines.Length <= 1) return string.Empty;

            return lines[1];
        }
        public static string Nationality(string[] lines)
        {
            if (lines.Length <= 2) return string.Empty;

            var nation = lines[2].Split('/');
            var name = _nationalities.NationalitybyName(Regex.Replace(nation[Math.Max(0, nation.Length - 1)],@"^a-zA-Z\s",string.Empty));

            if (name == null) return string.Empty;

            return name.Code;
        }
        public static string Name(string[] lines)
        {
            if (lines.Length <= 3) return string.Empty;

            var name = lines[3].Split('/');
            return Regex.Replace(name[Math.Max(0, name.Length - 1)], @"[^a-zA-Z\s]", string.Empty);
        }
        public static string CompanyType(string[] lines)
        {
            string name = Name(lines);
            string type = string.Empty;
            foreach (var item in OCRExtension.LegalTypes)
            {
                var exp = new Regex(item.Value, RegexOptions.IgnoreCase);
                if (exp.IsMatch(name))
                {
                    type = item.Key;
                    break;
                }
            }
            return type;
        }
        public static string Status(string companyType)
        {
            if (string.IsNullOrEmpty(companyType)) return COB_ShareHolderType.Ind.ToString();

            return COB_ShareHolderType.Com.ToString();
        }
    }
}
