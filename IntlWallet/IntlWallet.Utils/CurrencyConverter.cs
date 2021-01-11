using IntlWallet.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntlWallet.Utils
{
    public static class CurrencyConverter
    {
        private static async Task<Rates> GetCurrencyRates()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://data.fixer.io/api/latest?access_key=154446bde9c5d815fea1b585494b1a64");
            var content = await response.Content.ReadAsStringAsync();

            Page page = JsonSerializer.Deserialize<Page>(content);
            
            return page.rates;
        }

        public static async Task<decimal> ConvertCurrency(string transactionCurrency, string mainCurrency, decimal amount)
        {
            var mainCurrencyRate = await SelectCurrencyRate(mainCurrency);
            var transactionCurrencyRate = await SelectCurrencyRate(transactionCurrency);

            var convertedAmount = mainCurrencyRate / transactionCurrencyRate * amount;

            return convertedAmount;
        }

        private static async Task<decimal> SelectCurrencyRate(string currency)
        {
            decimal rate = 0;
            Rates rates = await GetCurrencyRates();
            if (currency == "AED")
                rate = rates.AED;
            else if (currency == "AFN")
                rate = rates.AFN;
            else if (currency == "ALL")
                rate = rates.ALL;
            else if (currency == "AMD")
                rate = rates.AMD;
            else if (currency == "ANG")
                rate = rates.ANG;
            else if (currency == "AOA")
                rate = rates.AOA;
            else if (currency == "ARS")
                rate = rates.ARS;
            else if (currency == "AUD")
                rate = rates.AUD;
            else if (currency == "AWG")
                rate = rates.AWG;
            else if (currency == "AZN")
                rate = rates.AZN;
            else if (currency == "BAM")
                rate = rates.BAM;
            else if (currency == "BBD")
                rate = rates.BBD;
            else if (currency == "BDT")
                rate = rates.BDT;
            else if (currency == "BGN")
                rate = rates.BGN;
            else if (currency == "BHD")
                rate = rates.BHD;
            else if (currency == "BIF")
                rate = rates.BIF;
            else if (currency == "BMD")
                rate = rates.BMD;
            else if (currency == "BND")
                rate = rates.BND;
            else if (currency == "BOB")
                rate = rates.BOB;
            else if (currency == "BRL")
                rate = rates.BRL;
            else if (currency == "BSD")
                rate = rates.BSD;
            else if (currency == "BTC")
                rate = rates.BTC;
            else if (currency == "BTN")
                rate = rates.BTN;
            else if (currency == "BWP")
                rate = rates.BWP;
            else if (currency == "BYN")
                rate = rates.BYN;
            else if (currency == "BYR")
                rate = rates.BYR;
            else if (currency == "BZD")
                rate = rates.BZD;
            else if (currency == "CAD")
                rate = rates.CAD;
            else if (currency == "CDF")
                rate = rates.CDF;
            else if (currency == "CHF")
                rate = rates.CHF;
            else if (currency == "CLF")
                rate = rates.CLF;
            else if (currency == "CLP")
                rate = rates.CLP;
            else if (currency == "CNY")
                rate = rates.CNY;
            else if (currency == "COP")
                rate = rates.COP;
            else if (currency == "CRC")
                rate = rates.CRC;
            else if (currency == "CUC")
                rate = rates.CUC;
            else if (currency == "CUP")
                rate = rates.CUP;
            else if (currency == "CVE")
                rate = rates.CVE;
            else if (currency == "CZK")
                rate = rates.CZK;
            else if (currency == "DJF")
                rate = rates.DJF;
            else if (currency == "DKK")
                rate = rates.DKK;
            else if (currency == "DOP")
                rate = rates.DOP;
            else if (currency == "DZD")
                rate = rates.DZD;
            else if (currency == "EGP")
                rate = rates.EGP;
            else if (currency == "ERN")
                rate = rates.ERN;
            else if (currency == "ETB")
                rate = rates.ETB;
            else if (currency == "EUR")
                rate = rates.EUR;
            else if (currency == "FJD")
                rate = rates.FJD;
            else if (currency == "FKP")
                rate = rates.FKP;
            else if (currency == "GBP")
                rate = rates.GBP;
            else if (currency == "GEL")
                rate = rates.GEL;
            else if (currency == "GGP")
                rate = rates.GGP;
            else if (currency == "GHS")
                rate = rates.GHS;
            else if (currency == "GIP")
                rate = rates.GIP;
            else if (currency == "GMD")
                rate = rates.GMD;
            else if (currency == "GNF")
                rate = rates.GNF;
            else if (currency == "GTQ")
                rate = rates.GTQ;
            else if (currency == "GYD")
                rate = rates.GYD;
            else if (currency == "HKD")
                rate = rates.HKD;
            else if (currency == "HNL")
                rate = rates.HNL;
            else if (currency == "HRK")
                rate = rates.HRK;
            else if (currency == "HTG")
                rate = rates.HTG;
            else if (currency == "HUF")
                rate = rates.HUF;
            else if (currency == "IDR")
                rate = rates.IDR;
            else if (currency == "ILS")
                rate = rates.ILS;
            else if (currency == "IMP")
                rate = rates.IMP;
            else if (currency == "INR")
                rate = rates.INR;
            else if (currency == "IQD")
                rate = rates.IQD;
            else if (currency == "IRR")
                rate = rates.IRR;
            else if (currency == "ISK")
                rate = rates.ISK;
            else if (currency == "JEP")
                rate = rates.JEP;
            else if (currency == "JMD")
                rate = rates.JMD;
            else if (currency == "JOD")
                rate = rates.JOD;
            else if (currency == "JPY")
                rate = rates.JPY;
            else if (currency == "KES")
                rate = rates.KES;
            else if (currency == "KGS")
                rate = rates.KGS;
            else if (currency == "KHR")
                rate = rates.KHR;
            else if (currency == "KMF")
                rate = rates.KMF;
            else if (currency == "KPW")
                rate = rates.KPW;
            else if (currency == "KRW")
                rate = rates.KRW;
            else if (currency == "KWD")
                rate = rates.KWD;
            else if (currency == "KYD")
                rate = rates.KYD;
            else if (currency == "KZT")
                rate = rates.KZT;
            else if (currency == "LAK")
                rate = rates.LAK;
            else if (currency == "LBP")
                rate = rates.LBP;
            else if (currency == "LKR")
                rate = rates.LKR;
            else if (currency == "LRD")
                rate = rates.LRD;
            else if (currency == "LSL")
                rate = rates.LSL;
            else if (currency == "LTL")
                rate = rates.LTL;
            else if (currency == "LVL")
                rate = rates.LVL;
            else if (currency == "LYD")
                rate = rates.LYD;
            else if (currency == "MAD")
                rate = rates.MAD;
            else if (currency == "MDL")
                rate = rates.MDL;
            else if (currency == "MGA")
                rate = rates.MGA;
            else if (currency == "MKD")
                rate = rates.MKD;
            else if (currency == "MMK")
                rate = rates.MMK;
            else if (currency == "MNT")
                rate = rates.MNT;
            else if (currency == "MOP")
                rate = rates.MOP;
            else if (currency == "MRO")
                rate = rates.MRO;
            else if (currency == "MUR")
                rate = rates.MUR;
            else if (currency == "MVR")
                rate = rates.MVR;
            else if (currency == "MWK")
                rate = rates.MWK;
            else if (currency == "MXN")
                rate = rates.MXN;
            else if (currency == "MYR")
                rate = rates.MYR;
            else if (currency == "MZN")
                rate = rates.MZN;
            else if (currency == "NAD")
                rate = rates.NAD;
            else if (currency == "NGN")
                rate = rates.NGN;
            else if (currency == "NIO")
                rate = rates.NIO;
            else if (currency == "NOK")
                rate = rates.NOK;
            else if (currency == "NPR")
                rate = rates.NPR;
            else if (currency == "NZD")
                rate = rates.NZD;
            else if (currency == "OMR")
                rate = rates.OMR;
            else if (currency == "PAB")
                rate = rates.PAB;
            else if (currency == "PEN")
                rate = rates.PEN;
            else if (currency == "PGK")
                rate = rates.PGK;
            else if (currency == "PHP")
                rate = rates.PHP;
            else if (currency == "PKR")
                rate = rates.PKR;
            else if (currency == "PLN")
                rate = rates.PLN;
            else if (currency == "PYG")
                rate = rates.PYG;
            else if (currency == "QAR")
                rate = rates.QAR;
            else if (currency == "RON")
                rate = rates.RON;
            else if (currency == "RSD")
                rate = rates.RSD;
            else if (currency == "RUB")
                rate = rates.RUB;
            else if (currency == "RWF")
                rate = rates.RWF;
            else if (currency == "SAR")
                rate = rates.SAR;
            else if (currency == "SBD")
                rate = rates.SBD;
            else if (currency == "SCR")
                rate = rates.SCR;
            else if (currency == "SDG")
                rate = rates.SDG;
            else if (currency == "SEK")
                rate = rates.SEK;
            else if (currency == "SGD")
                rate = rates.SGD;
            else if (currency == "SHP")
                rate = rates.SHP;
            else if (currency == "SLL")
                rate = rates.SLL;
            else if (currency == "SOS")
                rate = rates.SOS;
            else if (currency == "SRD")
                rate = rates.SRD;
            else if (currency == "STD")
                rate = rates.STD;
            else if (currency == "SVC")
                rate = rates.SVC;
            else if (currency == "SYP")
                rate = rates.SYP;
            else if (currency == "SZL")
                rate = rates.SZL;
            else if (currency == "THB")
                rate = rates.THB;
            else if (currency == "TJS")
                rate = rates.TJS;
            else if (currency == "TMT")
                rate = rates.TMT;
            else if (currency == "TND")
                rate = rates.TND;
            else if (currency == "TOP")
                rate = rates.TOP;
            else if (currency == "TRY")
                rate = rates.TRY;
            else if (currency == "TTD")
                rate = rates.TTD;
            else if (currency == "TWD")
                rate = rates.TWD;
            else if (currency == "TZS")
                rate = rates.TZS;
            else if (currency == "UAH")
                rate = rates.UAH;
            else if (currency == "UGX")
                rate = rates.UGX;
            else if (currency == "USD")
                rate = rates.USD;
            else if (currency == "UYU")
                rate = rates.UYU;
            else if (currency == "UZS")
                rate = rates.UZS;
            else if (currency == "VEF")
                rate = rates.VEF;
            else if (currency == "VND")
                rate = rates.VND;
            else if (currency == "VUV")
                rate = rates.VUV;
            else if (currency == "WST")
                rate = rates.WST;
            else if (currency == "XAF")
                rate = rates.XAF;
            else if (currency == "XAG")
                rate = rates.XAG;
            else if (currency == "XAU")
                rate = rates.XAU;
            else if (currency == "XCD")
                rate = rates.XCD;
            else if (currency == "XDR")
                rate = rates.XDR;
            else if (currency == "XOF")
                rate = rates.XOF;
            else if (currency == "XPF")
                rate = rates.XPF;
            else if (currency == "YER")
                rate = rates.YER;
            else if (currency == "ZAR")
                rate = rates.ZAR;
            else if (currency == "ZMK")
                rate = rates.ZMK;
            else if (currency == "ZMW")
                rate = rates.ZMW;
            else if (currency == "ZWL")
                rate = rates.ZWL;

            return rate;
        }
    }
}
