using System.Globalization;

namespace Cirrus.Import.Masterdata.Common
{
    [System.Diagnostics.DebuggerDisplay("{value}")]
    class Price
    {
        private readonly decimal value;

        public Price(decimal value)
        {
            this.value = value;
        }

        public static implicit operator decimal(Price value) => value.value;

        public static Price From(string value, int divisor = 1)
        {
            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return new Price(result / divisor);
            }

            return null;
        }

        public static Price FromId(long id, int maxPrice)
        {
            return FromId(id.ToString(), maxPrice);
        }

        public static Price FromId(string id, int maxPrice)
        {
            var number = Helper.GetStableHash(id, maxPrice * 100);
            var value = number / (decimal)100;
            return new Price(value);
        }
    }
}
