using Aquaforest.ExtendedOCR.Shared;
using Focusync.Infrastructure;
//using Focusync.Common.Enumerators;
using Focusync.Service.CoreBank.OCR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradeLicense.Model;

namespace TradeLicense
{
    static class SharjahPartnerParser
    {
        private static readonly Nationalities _nationalities = new Nationalities();

        public static PartnerListModel ParseObject(List<LineData> lines)
        {
            List<string> partners = new List<string>();
            int no = 0;
            Regex exp = new Regex(@"^%[0-9]{1,3}$");
            for (no = lines.Count - 1; no >= 0; no--)
            {
                if (exp.IsMatch(lines[no].LineWords))
                {
                    StringBuilder builder = new StringBuilder();

                    string prec = lines[no].FilterWithConfidenceScore().TrimStart().Substring(1);

                    builder.Append(prec.Substring(0, Math.Min(6, prec.Length))).Append('|');
                    int i = 1;
                    for (i = 1; i < 6 && no + i < lines.Count; i++)
                    {
                        builder.Append(lines[no + i].FilterWithConfidenceScore()).Append('|');
                    }
                    while (i < 6) builder.Append('|');
                    partners.Add(builder.ToString());
                }
            }
            List<PartnerModel> data = new List<PartnerModel>();
            foreach (var item in partners)
            {
                var words = item.Split('|');
                data.Add(new PartnerModel
                {
                    Percentage = Percentage(words),
                    Nationality = Nationality(words),
                    SrNo = SrNo(words),
                    Name = Name(words),
                    CompanyType = COB_ShareHolderType.Ind.ToString(),
                    Status = COB_ShareHolderType.Ind.ToString()
                });
            }
            return new PartnerListModel { Partners = data };
        }
        public static string Parse(List<LineData> lines)
        {
            return JsonConvert.SerializeObject(ParseObject(lines));
        }
        public static string Percentage(string[] lines)
        {
            if (lines.Length <= 0) return string.Empty;
            return lines[0];
        }
        public static string SrNo(string[] lines)
        {
            if (lines.Length <= 5) return string.Empty;

            return lines[5];
        }
        public static string Nationality(string[] lines)
        {
            if (lines.Length <= 3) return string.Empty;

            var nation = lines[3];
            var name = _nationalities.NationalitybyName(Regex.Replace(nation, @"^a-zA-Z\s", string.Empty));

            if (name == null) return string.Empty;

            return name.Code;
        }
        public static string Name(string[] lines)
        {
            if (lines.Length <= 4) return string.Empty;

            var name = lines[4];//.Split('/');

            return Regex.Replace(name,@"^a-zA-Z\s",string.Empty);
        }
    }
}
