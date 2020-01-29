using Focusync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Focusync.Middleware.Models
{
    public class CustomerOnBoardDB:IDisposable
    {
        public void Dispose()
        {

        }

        public string documentReader(COB_ParseType stepId, string fileSource, string extension, int appId, string modelData)
        {
            string returnExpression = string.Empty;
            IDocProcess docProcess = null;
            try
            {
                switch (stepId)
                {
                    case COB_ParseType.TradeLicense:
                        docProcess = new CPTradeLicenceProcessing();
                        break;
                    case COB_ParseType.Passport:
                        docProcess = new PassportProcessing();
                        break;
                    case COB_ParseType.EmiratesId:
                        docProcess = new EmiratesIDProcessing();
                        break;
                    //case COB_ParseType.MOA:
                    //    docProcess = new MOAProcessing();
                        break;
                    case COB_ParseType.SHTradeLicense:
                        docProcess = new SHTradeLicenceProcessing();
                        break;
                    default:
                        throw new NotImplementedException();
                }
                returnExpression = docProcess.ReadProcessing(fileSource, extension, appId, modelData);
            }
            catch (Exception ex)
            {
                /*throw ex*/;
                //Global.Logger.Error(ex);
                return ex.ToString();
            }
            return returnExpression;
        }
    }
}