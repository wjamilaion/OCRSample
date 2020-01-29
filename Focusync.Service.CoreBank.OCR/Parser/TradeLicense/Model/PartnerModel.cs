using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLicense.Model
{
    public class PartnerModel
    {
        public string Percentage { get; set; }
        public string SrNo { get; set; }
        public string Nationality { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string IssuingAuth { get; set; }
        public string CompanyType { get; set; }
    }
    public class PartnerListModel
    {
        public PartnerListModel()
        {
            Partners = new List<PartnerModel>(); 
        }
        public List<PartnerModel> Partners { get; set; }
    }
}
