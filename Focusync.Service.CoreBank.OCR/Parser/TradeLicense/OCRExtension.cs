using Aquaforest.ExtendedOCR.Shared;
using Focusync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLicense
{
    static class OCRExtension
    {
        public static Dictionary<string, string> LegalTypes = new Dictionary<string, string>()
        {
            { "LLC", @"(.*)(LLC|Limited Liability|Liability Company|Limited Liability Company|LC|Limited.*Company)(.*)" },
            { "PRT", @".*(PRT|Partnership).*" },
            { "SPT", @".*(SPT|Sole Propriertorship|Sole|Propriertorship).*"},
            { "FZC", @".*(FZC|Free Zone Company|Free|Zone).*"},
            { "OFC", @".*(OFC|Offshore Company|Offshore).*"},
            { "BLF", @".*(BLF|Branch of Local / Foreign Company|Branch of Local|Foreign Company|Branch.*Local).*"},
            { "FOC", @".*(FOC|Foreign Company).*"},
            { "GOV", @".*(GOV|Government).*"},
            { "OTH", @".*(OTH|Others).*"},
            { "IND", @".*Individual.*"},
            { "Establishment", @".*Establishment.*"},
        };
        public static string FilterWithConfidenceScore(this LineData line)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var word in line.Words)
            {
                if(word.ConfidenceScore <= Global.AquaforestConfidenceScore)
                {
                    builder.Append(word.Word).Append(' ');
                }
            }
            return builder.ToString().Trim();
        }
    }
}
