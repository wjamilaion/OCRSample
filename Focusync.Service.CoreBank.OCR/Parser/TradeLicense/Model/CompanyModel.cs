using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLicense.Model
{
    public class CompanyModel
    {
        public string TradeLicenceNumber { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public string TradeLicenceIssuanceAuthority { get; set; }
        public DateTime? TradeLicenceExpiryDate { get; set; }
        public DateTime? TradeLicenceIssueDate { get; set; }
        public string PhoneNumber { get; set; }
        public PartnerListModel Partners { get; set; }
        public CompanyModel()
        {
            Partners = new PartnerListModel();
        }
    }
}
