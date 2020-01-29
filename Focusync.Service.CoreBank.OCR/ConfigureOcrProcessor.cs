using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aquaforest.ExtendedOCR.Api;
using Aquaforest.ExtendedOCR.Shared;
using TradeLicense;
using System.IO;
using Focusync.Infrastructure;

namespace Focusync.Service.CoreBank.OCR
{
    public class ConfigureOcrProcessor
    {
        private static string tempPath;

        //private PreProcessor preProcessor = new PreProcessor();
        private int pageCompleted;
        private Rectangle region = new Rectangle();
        private bool textAvailable;
        private bool imageAvailable;
        private string license = Helper.LICENSE_KEY;
        private string logsPath;
        static ConfigureOcrProcessor()
        {
            tempPath = Path.Combine(Global.OcrTempFolderPath, "MultiThread");
        }

        public ConfigureOcrProcessor()
        {
            logsPath = Global.AquaforestSDKLogsPath;
        }
        private string GetConfigResource()
        {
            string resourceFolder = Global.AquaforestSDKPath;
            return resourceFolder;
        }
        private List<LineData> ProcessDocument(COB_ParseType stepId, string sourceFile, string extension)
        {
            List<LineData> lines = new List<LineData>();
            using (Ocr ocr = new Ocr(GetConfigResource()))
            {
                try
                {
                    var preProcessor = Configure(ocr);
                    switch (extension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            ocr.ReadJPEGSource(sourceFile);

                            break;

                        case ".tif":
                        case ".tiff":
                            ocr.ReadTIFFSource(sourceFile);
                            break;

                        case ".pdf":
                            //ocr.EnablePdfOutput = true;
                            //ocr.EnableTextOutput = false;
                            ocr.EndPage = 2;//process only 2 pages as currently nothing is being read from 3rd page
                            ocr.ReadPDFSource(sourceFile);
                            break;
                    }
                    if (stepId == COB_ParseType.TradeLicense)
                    {
                        preProcessor.ForceTableZones = true;
                    }
                    if (!ocr.Recognize(preProcessor))
                    {
                        if (ocr.LastException == null)
                            throw new Exception("Unable to parse document");
                        else
                            throw ocr.LastException;
                    }

                    for (int i = 1; i <= ocr.NumberPages && i <= ocr.EndPage; i++)
                    {
                        lines.AddRange(ocr.ReadPageLines(i));
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    try
                    {
                        File.Copy(ocr.LogFilePath, Path.Combine(logsPath, Path.GetFileNameWithoutExtension(sourceFile) + ".log"));
                    }
                    catch (Exception exp)
                    {

                    }
                    ocr.DeleteTemporaryFiles();
                }
            }
            return lines;
        }
        public string getValuesByTradeLicense(string sourceFile, string extension, string issuingAuth, string issuingAuthEntity)
        {
            var lines = ProcessDocument(COB_ParseType.TradeLicense, sourceFile, extension);
            string result = TradeLicenseParserFactory.Parse(lines, issuingAuth ,issuingAuthEntity);
            return result;
        }
        public string getValuesByEid(Ocr ocr)
        {
            StringBuilder sigbuild = new StringBuilder();
            Regex sigRegex = new Regex(@"(1|I)(L|D)ARE.*");
            for (int pageNo = 0; pageNo < ocr.NumberPages; pageNo++)
            {
                var lines = ocr.ReadPageLines(pageNo + 1);
                int i = 0;
                for (i = 0; i < lines.Count; i++)
                {
                    if (sigRegex.IsMatch(lines[i].LineWords))
                    {
                        break;
                    }
                }
                int idcount = 0;
                while (i < lines.Count && idcount < 3)
                {
                    string temp = lines[i].LineWords;
                    if (temp.Length > 26)
                    {
                        if (i == 0)
                        {
                            temp = (temp[0] == '1' ? 'I' : temp[0]).ToString().Replace("1", "I") + temp.Substring(1);
                        }
                        temp = temp.PadRight(30, '<');
                        sigbuild.Append(temp);
                    }
                    i++;
                }
            }
            string result = sigbuild.ToString();
            EIDMRZParser eidParser = new EIDMRZParser();
            return eidParser.Parse(result);
        }
        public string getValuesbyStepId(COB_ParseType stepId, string sourceFile, string extension)
        {
            string result = string.Empty;
            using (Ocr ocr = new Ocr(GetConfigResource()))
            {
                try
                {
                    var preProcessor = Configure(ocr);

                    if (stepId == COB_ParseType.EmiratesId)
                    {
                        using (Bitmap map = new Bitmap(sourceFile))
                        {
                            //using (var img = Helper.ConvertBlackWhiteImage(map))
                            {
                                var img = map;
                                //Helper.SetContrast(img, 80);
                                ocr.ReadImageSource(img);
                                if (ocr.Recognize(preProcessor))
                                {
                                    result = this.getValuesByEid(ocr);
                                }
                                else
                                {
                                    if (ocr.LastException == null)
                                        throw new Exception("Unable to parse document");
                                    else
                                        throw ocr.LastException;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (extension.ToLower())
                        {
                            case ".jpg":
                            case ".jpeg":
                            case ".png":
                            case ".bmp":
                                ocr.ReadJPEGSource(sourceFile);

                                break;

                            case ".tif":
                            case ".tiff":
                                ocr.ReadTIFFSource(sourceFile);
                                break;

                            case ".pdf":
                                //ocr.EnablePdfOutput = true;
                                //ocr.EnableTextOutput = false;
                                ocr.EndPage = 2;//process only 2 pages as currently nothing is being read from 3rd page
                                ocr.ReadPDFSource(sourceFile);
                                break;
                        }
                        if (stepId == COB_ParseType.TradeLicense)
                        {
                            preProcessor.ForceTableZones = true;
                        }

                        if (!ocr.Recognize(preProcessor))
                        {
                            if (ocr.LastException == null)
                                throw new Exception("Unable to parse document");
                            else
                                throw ocr.LastException;
                        }
                        switch (stepId)
                        {
                            case COB_ParseType.TradeLicense:
                                var licenseLines = new List<LineData>();
                                for (int i = 1; i <= ocr.NumberPages && i <= ocr.EndPage; i++)
                                {
                                    licenseLines.AddRange(ocr.ReadPageLines(i));
                                }
                                // currently 'dubai' is hardcoded, required changes to fetch it from Application
                                result = TradeLicenseParserFactory.Parse(licenseLines, "DUBAIECONOMY", "LLC");
                                break;
                            case COB_ParseType.Passport:
                                StringBuilder build = new StringBuilder();
                                for (int pageNo = 0; pageNo < ocr.NumberPages; pageNo++)
                                {
                                    var lines = ocr.ReadPageLines(pageNo + 1);
                                    int i = 0;
                                    for (i = 0; i < lines.Count; i++)
                                    {
                                        Regex re = new Regex(@"(P|V|C)(<|«)(.*)");
                                        if (re.IsMatch(lines[i].LineWords))
                                        {
                                            break;
                                        }
                                    }
                                    int counti = 0;
                                    while (i < lines.Count && counti < 2) //only two lines to be read as passport has only two line MRZ as per standard
                                    {
                                        string temp = lines[i].LineWords;
                                        temp = temp.TrimEnd().Replace("«", "<<").Replace("&", "<").Replace("\n", "").Replace(" ", "");
                                        temp = System.Text.RegularExpressions.Regex.Replace(temp, "[']+", "");
                                        if (counti == 0)
                                        {
                                            temp = Regex.Replace(temp, ".*P<", "P<");
                                        }
                                        temp = temp.Substring(0, Math.Min(temp.Length, 44));
                                        temp = temp.PadRight(44, '<');

                                        build.Append(temp);
                                        i++;
                                    }
                                }
                                result = build.ToString();
                                MRZParser pareser = new MRZParser();
                                result = pareser.Parse(result);
                                break;
                            
                            case COB_ParseType.EmiratesId:

                                result = getValuesByEid(ocr);
                                break;
                            default:
                                return string.Empty;
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw;
                }
                finally{
                    try
                    {
                        File.Copy(ocr.LogFilePath, Path.Combine(logsPath, Path.GetFileNameWithoutExtension(sourceFile)+ ".log"));
                    }
                    catch(Exception exp) {

                    }
                    ocr.DeleteTemporaryFiles();
                }
            }
            
            return result;
        }
        
        private PreProcessor Configure(Ocr ocr)
        {
            ocr.License = license;
            ocr.TempFolder = Path.Combine(tempPath, Guid.NewGuid().ToString());
            ocr.Language = SupportedLanguages.English;
            ocr.EnableTextOutput = true;
            ocr.GetAdvancedOCRData = true;
            ocr.LogToFile = true;
            //ocr.HandleExceptionsInternally = false;

            PreProcessor preProcessor = new PreProcessor();
            preProcessor.Deskew = true;
            preProcessor.Autorotate = false;
            preProcessor.KeepOriginalImage = true;

            //Create new binarization object
            Binarization binarize = new Binarization();

            //Enable binarization
            binarize.Binarize = true;

            //Assign it to PreProcessor
            preProcessor.Binarization = binarize;

            return preProcessor;
            //ocr.Recognize(preProcessor);
        }

    }
}
