using Focusync.Infrastructure;
using Focusync.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Focusync.Middleware.Controllers
{
    [AllowAnonymous]
    public class COBController : ApiController
    {
        public IHttpActionResult fileUpload([FromUri]COBFileUploadDto model)
        {
            string stepId = model.stepId, appId = model.appId, Id = model.Id, extra = model.extra;
            string ErrorMessage = null;
            string dmsDocumentId = string.Empty;
            CustomerOnBoardDB DB = new CustomerOnBoardDB();
            var response = new ResponseModel { StatusCode = ResponseStatus.Success, Message = "" };
            var request = System.Web.HttpContext.Current.Request;
            var uploads = request.Files;
            if (uploads != null)
            {

                
                string serverPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings.Get("COBPathURL"));
                DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(serverPath));

                if (!dir.Exists)
                {
                    response.StatusCode = ResponseStatus.Error;
                    response.Message = serverPath + " path does not exists.";
                    response.Title = "Configuration error";
                    return Json(response);
                }
                if (uploads.AllKeys.Count() > 0)
                {

                    for (int i = 0; i < uploads.Count; i++)
                    {
                        string keyName = uploads.AllKeys[i];
                        HttpPostedFile file = uploads[keyName];
                        string fileName = Path.GetFileName(file.FileName);

                        string extnession = Path.GetExtension(file.FileName);
                        if (!string.IsNullOrEmpty(model.fileName))
                        {
                            fileName = model.fileName + "." + extnession;
                        }
                        if (!String.IsNullOrEmpty(extra))
                        {
                            fileName = extra + "_" + fileName;
                        }
                        string name = Path.GetFileNameWithoutExtension(file.FileName);
                        fileName = Guid.NewGuid().ToString() + "_" + fileName;
                        var physicalPath = Path.Combine(serverPath, fileName);

                        file.SaveAs(physicalPath); //Will continue to save the file on physical or else will need to change the full flow of documentValidator as well.

                        serverPath = Url.Content(ConfigurationManager.AppSettings.Get("COBPathURL") + "/");

                        COB_ParseType? parseType = null;
                        var text = string.Empty;
                        if (stepId == Convert.ToString(NewCOB_Steps.GETTRADELICENSE))
                        {
                            parseType = COB_ParseType.TradeLicense;
                            
                        }
                        if (stepId == Convert.ToString(NewCOB_Steps.GETSHPASSPORT) || stepId == Convert.ToString(NewCOB_Steps.GETSIGPASSPORT) || stepId == Convert.ToString(NewCOB_Steps.GETBODPASSPORT))
                            parseType = COB_ParseType.Passport;
                        //if (stepId == Convert.ToString(NewCOB_Steps.GETMOA))
                        //    parseType = COB_ParseType.MOA;
                        if (stepId == Convert.ToString(NewCOB_Steps.GETSIGEID)
                            || stepId == Convert.ToString(NewCOB_Steps.GETSHEID) || stepId == Convert.ToString(NewCOB_Steps.GETBMEID))
                            parseType = COB_ParseType.EmiratesId;
                        if (stepId == Convert.ToString(NewCOB_Steps.GETCOMAPNYSHAREHOLDERTRADELICENSE))
                            parseType = COB_ParseType.SHTradeLicense;

                        if (parseType != null)
                        {
                            text = DB.documentReader(parseType.Value, physicalPath, extnession, Convert.ToInt16(appId), model.modelData);
                        }
                        response.Result = text;
                        response.Status = true;
                        response.StatusCode = ResponseStatus.Success;
                        return Json(response);
                    }
                }
                //var results = DB.LoadCustomerApplicationByAppID(Convert.ToInt16(appId));
                //results.message = "doc not valid";
                response.Message = "Not OK";
                response.Result = "doc not valid";
                response.Status = false;
                response.StatusCode = ResponseStatus.Failure;
                return Json(response);
            }
            else
            {
                response.StatusCode = ResponseStatus.Failure;
                response.Message = "No file is uploaded.";
                response.Title = "Invalid Action";
                return Json(response);
            }
        }
    }
}