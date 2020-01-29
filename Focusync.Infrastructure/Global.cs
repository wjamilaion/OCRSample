using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Focusync.Infrastructure
{
    public static class Global
    {

        public static string AquaforestSDKPath { get; set; }
        public static string AquaforestSDKLogsPath { get; set; }
        public static byte AquaforestConfidenceScore { get; set; }
        public static string OcrTempFolderPath { get; set; }
        public static string COBTempPath { get; set; }
        private static string _ApplicationPath = "";

        private static string _ConfigurationPath = ApplicationPath + "app_data\\config";

        private static string _AppDataPath = ApplicationPath + "app_data";


        public static string AppDataPath
        {
            get { return _AppDataPath; }
            set { _AppDataPath = value; }
        }

        private static string _ApplicationRoot = "";

        private static string _TempFolder = ApplicationPath + "\\temp";

        private static string _CurrentTheme = "";

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationPath
        {
            get { return _ApplicationPath; }
            set { _ApplicationPath = value; }
        }

        public static string ConfigurationPath
        {
            get { return _ConfigurationPath; }
            set { _ConfigurationPath = value; }
        }

        /// <summary>
        /// Website host Root path 
        /// </summary>
        public static string ApplicationRoot
        {
            get { return _ApplicationRoot; }
            set { _ApplicationRoot = value; }
        }

        /// <summary>
        /// Website host Root path 
        /// </summary>
        public static string UIRoot
        {
            get;
            set;
        }

        public static string apiRoot { get; set; }

        public static string CurrentTheme
        {
            get { return _CurrentTheme; }
            set { _CurrentTheme = value; }
        }

        public static string TempFolder
        {
            get { return _TempFolder; }
            set { _TempFolder = value; }
        }

        ///// <summary>
        ///// Global Logger used in application must be set when application is started
        ///// </summary>
        //public static log4net.ILog Logger { get; set; }

        ///// <summary>
        ///// Global OrmLogger used in application must be set when application is started
        ///// Used to log NHibernate operations in different log file i.e. dblog.txt
        ///// </summary>
        //public static log4net.ILog OrmLogger { get; set; }

        public static Boolean IsDebugging { get; set; }


        public static Boolean isFullErrorMessage { get; set; }

        public static Boolean IsDocumentValidationRequred
        {
            get; set;
        }
        //public static string SiteMap
        //{
        //    get { return Path.Combine(Focusync.Infrastructure.Classes.Global.ConfigurationPath, "sitemap.xml"); }
        //}

        //public static string SiteMap_XSLT
        //{
        //    get { return Path.Combine(Focusync.Infrastructure.Classes.Global.ConfigurationPath, "sitemap.xslt"); }
        //}

        public static string LogonPage
        {
            get;
            set;
        }

        /// <summary>
        /// current user id, must be set from the login form
        /// </summary>
        public static Int32? UserID
        {
            get;
            set;
        }

        public static string UserName
        {
            get;
            set;
        }

        public static DateTime SystemDate
        {
            get;
            set;
        }

        public static dynamic User { get; set; }


        public static bool isCrossAuthorise
        {
            get;
            set;
        }


        public static Int64 SessionSerialNo
        {
            get;
            set;
        }

        //public static XDocument EncryptedFeilds
        //{
        //    get;
        //    set;
        //}


        public static string AuthoriseBerearToken
        {
            get { return "Token"; }
        }

        public static string AuthenticateCookieName
        {
            get { return "fs1pref"; }
        }

        public static string UserCookieName
        {
            get { return "FS001"; }
        }
        public static string SysTokenName
        {
            get { return "SysT0k01"; }
        }

        public static Int16 LoginTokenExpiryTime
        {
            get;
            set;
        }

        public static Int16 ComonTokenExpiryTime
        {
            get;
            set;
        }

        public static Int16 OTPExpiryTime
        {
            get;
            set;
        }

        public static Int16 PasswordResetTokenExpiryTime
        {
            get;
            set;
        }

        public static string ClaimsCookie
        {
            get { return "FS002"; }
        }

        public static bool IsInternalUserLogin
        {
            get;
            set;
        }

        public static bool ShowAzureADLogin { get; set; }
        public static string AADClientID { get; set; }
        public static string AADTenant { get; set; }
        public static string AADInstance { get; set; }
        public static string AADPostLogoutRedirectUri { get; set; }

    }
}
