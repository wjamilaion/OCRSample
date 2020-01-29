﻿using Aquaforest.ExtendedOCR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLicense
{
    public sealed class IssuingAuth
    {
        public const string DUBAIECONOMY = "DUBAIECONOMY";
        public const string DCCA = "DCCA";
        public const string SHARJAH = "SHARJAH";
        public const string ABUDHABI = "ABUDHABI";
        public const string Fujairah = "Fujairah";
        public const string JABELALI = "JABELALI";
    }
    class TradeLicenseParserFactory
    {
        public static string Parse(List<LineData> lines, string issuingAuth, string issuingAuthEntity)
        {
            TradeLicense.ITradeLicense tradeLicense = null;
            switch(issuingAuth.ToUpper())
            {
                case IssuingAuth.DUBAIECONOMY:
                    tradeLicense = new DubaiTradeParser();
                    break;
                case IssuingAuth.SHARJAH:
                    tradeLicense = new SharjahTradeParser();
                    break;
                case IssuingAuth.DCCA:
                    tradeLicense = new DubaiCCATradeParser();
                    break;
                case IssuingAuth.ABUDHABI:
                    tradeLicense = new AbuDhabiTradeParser();
                    break;
                case IssuingAuth.JABELALI:
                    tradeLicense = new JabelAliFreeZoneParser();
                    break;
                default:
                    throw new NotImplementedException(string.Format("'{0}' not implemented", issuingAuth));
            }

            var company = tradeLicense.Parse(lines);
            company.TradeLicenceIssuanceAuthority = issuingAuth;
            company.EntityType = issuingAuthEntity;

            return Newtonsoft.Json.JsonConvert.SerializeObject(company);
        }
    }
}