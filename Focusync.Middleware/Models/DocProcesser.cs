using Focusync.Infrastructure;
using Focusync.Service.CoreBank.OCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeLicense;

namespace Focusync.Middleware.Models
{
    interface IDocProcess
    {
        string ReadProcessing(string fileSource, string extension, int AppId, string modelData);
    }


    public abstract class TradeLicenceProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, string issuingAuthCode, string entityTypeCode)
        {
            ConfigureOcrProcessor ocr = new ConfigureOcrProcessor();
            return ocr.getValuesByTradeLicense(fileSource, extension, issuingAuthCode, entityTypeCode);
        }

        public abstract string ReadProcessing(string fileSource, string extension, int AppId, string modelData);
    }
    public class CPTradeLicenceProcessing : TradeLicenceProcessing
    {
        public override string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            string issuingAuthCode = IssuingAuth.DUBAIECONOMY, entityTypeCode= "LLC";
            
            return ReadProcessing(fileSource, extension, issuingAuthCode, entityTypeCode);
        }
    }
    public class SHTradeLicenceProcessing : TradeLicenceProcessing
    {
        public override string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            string issuingAuthCode = IssuingAuth.DUBAIECONOMY, entityTypeCode = "LLC";

            //var modelObject = Newtonsoft.Json.JsonConvert.DeserializeObject<COBShareHolderCompanyModel>(modelData);
            //using (CustomerOnBoardDB db = new CustomerOnBoardDB())
            //{
            //    int authorityId = Convert.ToInt32(modelObject.IssuanceAuthority);
            //    int companyTypeId = Convert.ToInt32(modelObject.CompanyType);
            //    var authorities = db.GetAuthorities();
            //    issuingAuthCode = authorities.FirstOrDefault(auth => auth.Id == authorityId)?.Code;
            //    entityTypeCode = authorities.FirstOrDefault(auth => auth.Id == companyTypeId)?.Code;
            //}
            return ReadProcessing(fileSource, extension, issuingAuthCode, entityTypeCode);
        }
    }

    //public class MOAProcessing : IDocProcess
    //{
    //    public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
    //    {
    //        CustomerOnBoardDB db = new CustomerOnBoardDB();
    //        var list = db.GetDocumentReaderOCRBoxingByDocumentTypeId(2, 2);
    //        ConfigureOcrProcessor ocr = new ConfigureOcrProcessor();
    //        return ocr.getValuesbyStepId(COB_ParseType.MOA, fileSource, extension, list);
    //    }
    //}

    public class PassportProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            ConfigureOcrProcessor ocr = new ConfigureOcrProcessor();
            return ocr.getValuesbyStepId(COB_ParseType.Passport, fileSource, extension);
        }
    }

    public class EmiratesIDProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            ConfigureOcrProcessor ocr = new ConfigureOcrProcessor();
            return ocr.getValuesbyStepId(COB_ParseType.EmiratesId, fileSource, extension);
        }
    }

    public class NationalIDProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            throw new NotImplementedException();
        }
    }

    public class BoardOfResolutionProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            throw new NotImplementedException();
        }
    }

    public class PowerOfAttorneyProcessing : IDocProcess
    {
        public string ReadProcessing(string fileSource, string extension, int AppId, string modelData)
        {
            throw new NotImplementedException();
        }
    }
}