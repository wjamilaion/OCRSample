using Aquaforest.ExtendedOCR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLicense
{
    interface ITradeLicense
    {
        string DateFormat { get; }
        string DateSearchRegex { get; }
        string DateFormatReplaceRegex { get; }
        Model.CompanyModel Parse(List<LineData> lines);
        Model.PartnerListModel ParsePartner(List<LineData> lines);
        Model.CompanyModel ParseCompany(List<LineData> lines);
    }
}
