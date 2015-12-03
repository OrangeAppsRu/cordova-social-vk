using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API;

namespace VK.WindowsPhone.SDK
{
    public class VKAppPlatform
    {
        public class InAppPurchaseData
        {
            public class ReceiptData
            {
                public string receipt_data { get; set; }
                public decimal price_value { get; set; }
                public string currency { get; set; }
                public int quantity { get; set; }
            }

            public string Receipt { get; private set; }
            public decimal Price { get; private set; }
            public string Currency { get; private set; }

            public InAppPurchaseData(
                string receiptStr,
                string price)
            {
                if (string.IsNullOrWhiteSpace(receiptStr))
                {
                    throw new ArgumentException("Invalid receipt string");
                }

                if (string.IsNullOrWhiteSpace(price))
                {
                    throw new ArgumentException("Invalid price string");
                }

                Receipt = receiptStr;



                var regionInfo = System.Globalization.RegionInfo.CurrentRegion;

                Currency = regionInfo.ISOCurrencySymbol;

                Price = ParsePriceStr(price);
            }

            private decimal ParsePriceStr(string price)
            {
                // since we don't know the exact culture here, we do our best
                var parsedValue = ToDecimal(price);
                return parsedValue;
            }


            /// <summary>
            /// Convert string value to decimal ignore the culture.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>Decimal value.</returns>
            public static decimal ToDecimal(string value)
            {
                decimal number;
                value = new String(value.Where(c => char.IsPunctuation(c) || char.IsNumber(c)).ToArray());
                string tempValue = value;

                var punctuation = value.Where(x => char.IsPunctuation(x)).Distinct();
                int count = punctuation.Count();

                NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
                switch (count)
                {
                    case 0:
                        break;
                    case 1:
                        tempValue = value.Replace(",", ".");
                        break;
                    case 2:
                        if (punctuation.ElementAt(0) == '.')
                            tempValue = SwapChar(value, '.', ',');
                        break;
                    default:
                        throw new InvalidCastException();
                }

                number = decimal.Parse(tempValue, format);
                return number;
            }
            /// <summary>
            /// Swaps the char.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            /// <returns></returns>
            public static string SwapChar(string value, char from, char to)
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                StringBuilder builder = new StringBuilder();

                foreach (var item in value)
                {
                    char c = item;
                    if (c == from)
                        c = to;
                    else if (c == to)
                        c = from;

                    builder.Append(c);
                }
                return builder.ToString();
            }

            internal string ToJsonString()
            {
                var rd = new ReceiptData
                {
                    receipt_data = Receipt,
                    price_value = Price,
                    currency = Currency,
                    quantity = 1
                };

                var jsonStr = JsonConvert.SerializeObject(rd);

                return jsonStr;
            }
        }


        private static VKAppPlatform _instance;

        public static VKAppPlatform Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VKAppPlatform();
                }

                return _instance;
            }
        }

        protected VKAppPlatform() { }

        public void ReportInAppPurchase(InAppPurchaseData inAppPurchaseData)
        {
            if (inAppPurchaseData == null)
            {
                throw new ArgumentNullException("inAppPurchaseData");
            }

            SendPurchaseData(inAppPurchaseData);
        }

        private async void SendPurchaseData(InAppPurchaseData inAppPurchaseData)
        {
            var vkSaveTransationRequest = new VKRequest("apps.saveTransaction",
                "platform", "winphone",
                "app_id", VKSDK.Instance.CurrentAppID,
                "device_id", VKSDK.DeviceId,
                "receipt", inAppPurchaseData.ToJsonString());

            bool success = false;

            int it = 0;

            while (!success && it < 3)
            {
                var result = await vkSaveTransationRequest.DispatchAsync<Object>((jsonStr) => new Object());

                success = result.ResultCode == VKResultCode.Succeeded;

                it++;

                if (!success)
                {
                    await Task.Delay(4000);
                }
            }
        }

    }
}
