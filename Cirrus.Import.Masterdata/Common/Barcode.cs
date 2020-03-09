using System;

namespace Cirrus.Import.Masterdata.Common
{
    class Barcode
    {
        private readonly string value;

        public Barcode(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return this.value;
        }

        public static implicit operator string(Barcode value) => value.ToString();

        public static Barcode FromId(string key, long id)
        {
            var barcode = $"2{Helper.Pad(Helper.GetStableHash(key, 1000), 3)}{Helper.Pad(id, 8)}";
            var checkDigit = GetCheckDigit(barcode);
            return new Barcode($"{barcode}{checkDigit}");
        }

        public static Barcode FromId(string key, string id)
        {
            var barcode = $"2{Helper.Pad(Helper.GetStableHash(key, 1000), 3)}{Helper.Pad(Helper.GetStableHash(id, 100000000), 8)}";
            var checkDigit = GetCheckDigit(barcode);
            return new Barcode($"{barcode}{checkDigit}");
        }

        private static string GetCheckDigit(string data)
        {
            var barcodeString = data.ToCharArray();
            Array.Reverse(barcodeString);

            var roundingValue = 0;
            var checkDigit = 0;

            for (var i = 0; i < barcodeString.Length; i++)
            {
                if (i % 2 == 0)
                {
                    checkDigit += int.Parse(barcodeString[i].ToString(), System.Globalization.NumberStyles.Integer) * 3;
                }
                else
                {
                    checkDigit += int.Parse(barcodeString[i].ToString(), System.Globalization.NumberStyles.Integer) * 1;
                }
            }

            roundingValue = (int)(Math.Ceiling((double)checkDigit / 10) * 10);
            checkDigit = roundingValue - checkDigit;

            return checkDigit.ToString();
        }
    }
}
