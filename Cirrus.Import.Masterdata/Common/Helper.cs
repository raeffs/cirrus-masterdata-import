namespace Cirrus.Import.Masterdata.Common
{
    static class Helper
    {
        public static int GetStableHash(string value, int lowerThan)
        {
            uint hash = 0;
            foreach (byte b in System.Text.Encoding.Unicode.GetBytes(value))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return (int)(hash % lowerThan);
        }

        public static string Pad(long value, int length)
        {
            return value.ToString().PadLeft(length).Replace(' ', '0').Substring(0, length);
        }
    }
}
