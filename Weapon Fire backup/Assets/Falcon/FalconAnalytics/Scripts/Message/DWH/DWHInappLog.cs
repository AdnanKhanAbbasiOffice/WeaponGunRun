using System;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhInAppLog : DwhMessage
    {
        public string productId;
        public string currencyCode;
        public string price;
        public string transactionId;
        public string purchaseToken;
        public string where;
        
        [Preserve]
        [JsonConstructor]
        public DwhInAppLog(string productId, string currencyCode, string price, string transactionId, string purchaseToken, string where, long clientCreateDate) : base(clientCreateDate)
        {

            if (string.IsNullOrEmpty(currencyCode))
            {
                AnalyticLogger.Instance.Error("Dwh Log invalid field: Null or empty currency code of InAppLog, considering it as USD");
                currencyCode = "USD";
            }

            if (string.IsNullOrEmpty(price))
            {
                AnalyticLogger.Instance.Error("Dwh Log invalid field: Null or empty price of InAppLog, considering it as 0.00");
                price = "0.00";
            }
            
            this.productId = CheckStringLength(productId, nameof(productId), 100);
            this.currencyCode = CheckStringLength(currencyCode, nameof(currencyCode),20);
            this.price = FormatCurrencyStr(price, currencyCode);
            this.transactionId = TrimIfTooLong(transactionId, 200);
            this.purchaseToken = TrimIfTooLong(purchaseToken, 500);
            this.where = CheckStringLength(where, nameof(where),250);
        }
        
        public DwhInAppLog(string productId, string currencyCode, decimal price, string transactionId, string purchaseToken, string where, long clientCreateDate) : base(clientCreateDate)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                AnalyticLogger.Instance.Error("Dwh Log invalid field: Null or empty currency code of InAppLog, considering it as USD");
                currencyCode = "USD";
            }
            
            this.productId = CheckStringLength(productId, nameof(productId), 100);
            this.currencyCode = CheckStringLength(currencyCode, nameof(currencyCode),20);
            this.price = Math.Round(CheckNumberNonNegative(price, nameof(price))).ToString("0.00",CultureInfo.InvariantCulture);
            this.transactionId = TrimIfTooLong(transactionId, 200);
            this.purchaseToken = TrimIfTooLong(purchaseToken, 500);
            this.where = CheckStringLength(where, nameof(where),250);
        }


        protected override string GetAPI()
        {
            return DwhConstants.InAppApi;
        }

        private static string FormatCurrencyStr(string currencyStr, string currencyCode)
        {
            try
            {
                var numberPart = PreprocessCurrency(currencyStr);
                int dividerCount = GetDividerCount(numberPart);
                if (IsFloatingNumber(numberPart) && dividerCount == 1)
                {
                    return numberPart.Replace(',', '.');
                }
                if (currencyCode.ToUpper().Equals("EUR") && dividerCount == 1)
                {
                    numberPart = ProcessEuro(numberPart);
                }
                if(currencyCode.ToUpper().Equals("USD") && dividerCount == 1)
                {
                    numberPart = ProcessUsd(numberPart);
                }
                
                CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    string isoCurrencySymbol = new RegionInfo(info.LCID).ISOCurrencySymbol;
                    if (isoCurrencySymbol.Equals(currencyCode,StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        cultureInfo = info;
                        break;
                    }
                }
                
                decimal currency = FormatCurrency(numberPart, cultureInfo);

                return Math.Round(currency).ToString("0.00",CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                AnalyticLogger.Instance.Warning(e.Message);
                return currencyStr;
            }
        }
        
        private static decimal FormatCurrency(string input, CultureInfo culture)
        {
            try
            {
                return decimal.Parse(input, culture);
            }
            catch (Exception)
            {
                return decimal.Parse(input, CultureInfo.InvariantCulture);
            }
        }
        private static string PreprocessCurrency(string currencyStr)
        {
            var result = new StringBuilder();
            foreach (var ch in currencyStr)
                if (ch == '.' || ch == ',')
                    result.Append(ch);
                else if (char.IsNumber(ch))
                    result.Append(char.GetNumericValue(ch));
                //for arabic number
                else if (ch == '٫') result.Append('.');

            // Remove the dot at the start and end of the string, if any
            while (result[result.Length -1]== '.' || result[result.Length -1]== ',')
            {
                result.Length--;
            }

            while (result[0]== '.' || result[0]== ',')
            {
                result.Remove(0,1);
            }
            
            return result.ToString();
        }

        private static int GetDividerCount(string currencyStr)
        {
            return currencyStr.Split(',').Length + currencyStr.Split('.').Length - 2;
        }

        private static bool IsFloatingNumber(string currencyStr)
        {
            if (currencyStr.Length < 4) return false;
            char ch = currencyStr[currencyStr.Length - 3];
            return ch.Equals('.') || ch.Equals(',');
        }
        
        private static string ProcessEuro(string currencyStr)
        {
            return currencyStr.Replace(".", ",");
        }

        private static string ProcessUsd(string currencyStr)
        {
            return currencyStr.Replace(",", ".");
        }
    }
}