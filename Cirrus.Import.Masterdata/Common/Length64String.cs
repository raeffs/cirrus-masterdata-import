namespace Cirrus.Import.Masterdata.Common
{
    class Length64String
    {
        private readonly string value;

        public Length64String(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            if (this.value == null || this.value.Length <= 64)
            {
                return this.value;
            }

            return $"{this.value.Substring(0, 61)}...";
        }

        public static implicit operator string(Length64String value) => value.ToString();
        public static implicit operator Length64String(string value) => new Length64String(value);
    }
}
