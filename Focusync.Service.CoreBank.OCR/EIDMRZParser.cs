//using Focusync.Models.OnlineBanking.CustomerOnBoard;
using Focusync.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Focusync.Service.CoreBank.OCR
{
    class EIDMRZParser
    {
        private readonly Nationalities _nationalities = new Nationalities();


        private readonly Dictionary<char, int> _checkDigitArray = new Dictionary<char, int>();
        public string Parse(string mrz)
        {
            var validationMessage = MRZValidationMessage(mrz);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                var errorObj = new MRZResult { IsValid = false, ValidationMessage = validationMessage };
                return JsonConvert.SerializeObject(errorObj);
            }

            var output = new MRZResult
            {
                DocumentType = DocumentType(mrz),
                Gender = Gender(mrz),
                ExpireDate = ExpireDate(mrz),
                IssuingCountryIso = IssuingCountryIso(mrz),
                FirstName = FirstName(mrz),
                LastName = LastName(mrz),
                DocumentNumber = DocumentNumber(mrz),
                NationalityIso = NationalityIso(mrz),
                DateOfBirth = DateOfBirth(mrz)
            };

            output.DocumentTypeDescription = DocumentTypeDescription(output.DocumentType);

            output.IssuingCountryName = IssuingCountryName(output.IssuingCountryIso);

            output.FullName = (output.FirstName + " " + output.LastName).Replace("  ", " ").Trim();

            output.NationalityName = NationalityName(output.NationalityIso);

            //output.Age = (int)(DateTime.Now.Subtract(output.DateOfBirth).TotalDays / 365);

            return JsonConvert.SerializeObject(output);
        }
        private string MRZValidationMessage(string mrz)
        {
            if (string.IsNullOrEmpty(mrz)) throw new Exception("Empty MRZ");
            if (mrz.Length < 90) throw new Exception($"MRZ length is not valid should be 90 but it is {mrz.Length}");
            if (mrz.Substring(0, 1) != "I") throw new Exception($"Document Type should either be I");


            string issueDate = mrz.Substring(30 + 0, 6);
            string expireDate = mrz.Substring(30 + 8, 6);
            string optional = mrz.Substring(30 + 18, 11);

            string documentNo = mrz.Substring(0 + 5, 9);
            char documentCheckDigit = CheckDigit(documentNo).FirstOrDefault();
            if (CheckDigit(documentNo).FirstOrDefault() != documentCheckDigit) throw new Exception("unable to read passport properly, please upload well-scanned document");

            if (CheckDigit(issueDate).FirstOrDefault() != mrz[30+6]) throw new Exception("unable to read passport properly, please upload well-scanned document");

            if (CheckDigit(expireDate).FirstOrDefault() != mrz[30+8+6]) throw new Exception("unable to read passport properly, please upload well-scanned document");

            //char checkDigit = CheckDigit(CheckDigit(mrz.Substring(5, 25)) + mrz.Substring(30 + 0, 7) +//+ issueDate + mrz[30 + 6] +
            //                                mrz.Substring(30 + 8, 7) +//expireDate + mrz[30 + 8 + 6] +
            //                                optional

            //                                ).FirstOrDefault();
            //if (checkDigit != mrz[30 + 29]) throw new Exception("unable to read passport properly, please upload well-scanned document");
            return string.Empty;
        }
        private string DocumentType(string mrz)
        {
            return mrz.Substring(0, 1);

        }
        private string DocumentTypeDescription(string docType)
        {
            switch (docType)
            {
                case "I":
                    return "Emirates ID";
                default:
                    throw new Exception("Invalid Document Type");
            }
        }
        private string IssuingCountryIso(string mrz)
        {
            string iso = mrz.Substring(2, 3);
            iso = iso.Replace('1', 'I').Replace('6', 'G');
            if (this._nationalities.NationalitybyCode(iso) == null) return string.Empty;
            return iso;
        }
        private string IssuingCountryName(string issIso)
        {
            var natItem = _nationalities.NationalitybyCode(issIso);
            return natItem != null ? natItem.Name : string.Empty;

        }
        private string FirstName(string mrz)
        {

            var nameArraySplit = mrz.Substring(60).Split(new[] { "<<" }, StringSplitOptions.RemoveEmptyEntries);
            return Regex.Replace(nameArraySplit.Length >= 2 ? nameArraySplit[1].Replace("<", " ") : nameArraySplit[0].Replace("<", " "), @"[^a-zA-Z]\s", string.Empty);
        }

        private string LastName(string mrz)
        {
            var nameArraySplit = mrz.Substring(60).Split(new[] { "<<" }, StringSplitOptions.RemoveEmptyEntries);
            return Regex.Replace(nameArraySplit.Length >= 2 ? nameArraySplit[0].Replace("<", " ") : string.Empty, @"[^a-zA-Z]\s",string.Empty);
        }


        private string DocumentNumber(string mrz)
        {
            string no = mrz.Substring(0 + 15, 15);
            if (string.IsNullOrEmpty(no)) throw new Exception("Unable to read Emirates Id");
            return Regex.Replace(no,"[^0-9]",string.Empty);

        }

        private string NationalityIso(string mrz)
        {
            string iso = mrz.Substring(30 + 15, 3);
            iso = iso.Replace('1', 'I').Replace('6', 'G');
            iso = iso.Replace("IRG", "IRQ");
            if (this._nationalities.NationalitybyCode(iso) == null) return string.Empty;

            return iso;
        }
        private string NationalityName(string natIso)
        {
            var natItem = _nationalities.NationalitybyCode(natIso);
            return natItem != null ? natItem.Name : string.Empty;

        }
        private DateTime? DateOfBirth(string mrz)
        {
            try
            {
                var dob = new DateTime(int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + mrz.Substring(0 + 30, 2)), int.Parse(mrz.Substring(2 + 30, 2)),
                        int.Parse(mrz.Substring(4 + 30, 2)));

                if (dob < DateTime.Now)
                    return dob;

                return dob.AddYears(-100); //Subtract a century
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string Gender(string mrz)
        {
            string gender = mrz.Substring(7 + 30, 1);
            if (Regex.IsMatch(gender, "(M|F)", RegexOptions.IgnoreCase)) return gender;
            return string.Empty;

        }

        private DateTime? ExpireDate(string mrz)
        {
            //I am assuming all passports will certainly expire this century

            try
            {
                return new DateTime(int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + mrz.Substring(8 + 30, 2)), int.Parse(mrz.Substring(10 + 30, 2)),
                                int.Parse(mrz.Substring(12 + 30, 2)));
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        internal string CheckDigit(string icaoPassportNumber)
        {
            //http://www.highprogrammer.com/alan/numbers/mrp.html#checkdigit
            if (!_checkDigitArray.Any())
                FillCheckDigitDictionary();
            icaoPassportNumber = icaoPassportNumber.ToUpper();
            var inputArray = icaoPassportNumber.Trim().ToCharArray();
            var multiplier = 7;
            var total = 0;
            foreach (var dig in inputArray)
            {
                total = total + _checkDigitArray[dig] * multiplier;
                if (multiplier == 7) multiplier = 3;
                else if (multiplier == 3) multiplier = 1;
                else if (multiplier == 1) multiplier = 7;
            }

            long result;
            Math.DivRem(total, 10, out result);
            return result.ToString();
        }

        private void FillCheckDigitDictionary()
        {
            _checkDigitArray.Add('<', 0);
            _checkDigitArray.Add('0', 0);
            _checkDigitArray.Add('1', 1);
            _checkDigitArray.Add('2', 2);
            _checkDigitArray.Add('3', 3);
            _checkDigitArray.Add('4', 4);
            _checkDigitArray.Add('5', 5);
            _checkDigitArray.Add('6', 6);
            _checkDigitArray.Add('7', 7);
            _checkDigitArray.Add('8', 8);
            _checkDigitArray.Add('9', 9);
            _checkDigitArray.Add('A', 10);
            _checkDigitArray.Add('B', 11);
            _checkDigitArray.Add('C', 12);
            _checkDigitArray.Add('D', 13);
            _checkDigitArray.Add('E', 14);
            _checkDigitArray.Add('F', 15);
            _checkDigitArray.Add('G', 16);
            _checkDigitArray.Add('H', 17);
            _checkDigitArray.Add('I', 18);
            _checkDigitArray.Add('J', 19);
            _checkDigitArray.Add('K', 20);
            _checkDigitArray.Add('L', 21);
            _checkDigitArray.Add('M', 22);
            _checkDigitArray.Add('N', 23);
            _checkDigitArray.Add('O', 24);
            _checkDigitArray.Add('P', 25);
            _checkDigitArray.Add('Q', 26);
            _checkDigitArray.Add('R', 27);
            _checkDigitArray.Add('S', 28);
            _checkDigitArray.Add('T', 29);
            _checkDigitArray.Add('U', 30);
            _checkDigitArray.Add('V', 31);
            _checkDigitArray.Add('W', 32);
            _checkDigitArray.Add('X', 33);
            _checkDigitArray.Add('Y', 34);
            _checkDigitArray.Add('Z', 35);
        }
    }
}
