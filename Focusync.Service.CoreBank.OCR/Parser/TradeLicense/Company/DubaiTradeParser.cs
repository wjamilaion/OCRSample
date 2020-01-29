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
    class DubaiTradeParser : TradeLicenseParser
    {
        public override string DateFormat { get { return "dd/MM/yyyy"; } }

        public override string DateSearchRegex => @"^([0-9]{2,2}).([0-9]{2,2}).([0-9]{4,4})$";

        public override string DateFormatReplaceRegex => @"$1/$2/$3";

        public override CompanyModel Parse(List<LineData> lines)
        {
            CompanyModel company = ParseCompany(lines);
            company.Partners = ParsePartner(lines);
            return company;
        }

        public override PartnerListModel ParsePartner(List<LineData> lines)
        {
            return DubaiPartnerParser.ParseObject(lines);
        }
    }
}
