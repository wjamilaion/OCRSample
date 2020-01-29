using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Focusync.Infrastructure
{
    public enum COB_ParseType
    {
        TradeLicense,
        Passport,
        EmiratesId,
        MOA,
        SHTradeLicense
    }
    public enum NewCOB_Steps
    {
        GETTRADELICENSE = 1,
        GETMOA = 2,
        GETSHPASSPORT = 3,
        GETSIGPASSPORT = 5,
        GETSIGEID = 6,
        GETSIGEVISA = 7,
        GETSIGPOA = 8,
        GETBODPASSPORT = 9,
        UPLOADCHAMBEROFCOMMERCE = 10,
        UPLOADTAXCERTIFICATION = 11,
        //SHPassport = 3,
        //MOA = 3,
        Signatory = 12,
        POASignatory = 13,
        //SigPassport = 4,
        //SigEid = 5,
        //SigEVisa = 6,
        //POASignatory = 60,
        //SigPOA = 7,
        //BODPassport = 8,
        //UploadChamberOfCommerce = 81,
        //UploadTaxCertification = 82,
        SubmitScreen = 14,
        GETSHEID = 15,
        GETOTHERDOC = 20,
        GETSHEIDFRONT = 151,
        GETBMEID = 152,
        GETBMEIDFRONT = 153,
        GETSIGEIDFRONT = 154,
        BoardOfResolution = 155,
        //CIFCreated = 100,
        GETCOMAPNYSHAREHOLDERTRADELICENSE = 16,
        RMSubmitScreen = 101,

    }
    public enum COB_ShareHolderType
    {
        Ind,
        Com
    }
}
