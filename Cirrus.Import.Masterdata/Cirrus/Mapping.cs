using System;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class Mapping
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public override bool Equals(object value)
        {
            Mapping mapping = value as Mapping;

            return !Object.ReferenceEquals(null, mapping)
                && String.Equals(Id, mapping.Id)
                && String.Equals(Key, mapping.Key)
                && String.Equals(Value, mapping.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Id) ? Id.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Key) ? Key.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Value) ? Value.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(Mapping mapA, Mapping mapB)
        {
            if (Object.ReferenceEquals(mapA, mapB))
            {
                return true;
            }

            if (Object.ReferenceEquals(null, mapA))
            {
                return false;
            }

            return (mapA.Equals(mapB));
        }

        public static bool operator !=(Mapping mapA, Mapping mapB)
        {
            return !(mapA == mapB);
        }
    }
}
