﻿using System;
using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class Reference
    {
        public string Id { get; }

        // name needs to be something != empty
        public string Name { get; } = "random";

        public Reference(string id)
        {
            this.Id = id;
        }

        public static Reference From(string id)
        {
            if (id == null || id == "0")
            {
                return null;
            }

            return new Reference(id);
        }

        public static List<Reference> ListFrom(string id)
        {
            if (id == null || id == "0")
            {
                return new List<Reference>();
            }

            return new List<Reference>
            {
                new Reference(id)
            };
        }

        public override bool Equals(object value)
        {
            Reference reference = value as Reference;

            return !Object.ReferenceEquals(null, reference)
                && String.Equals(Id, reference.Id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Id) ? Id.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(Reference refA, Reference refB)
        {
            if (Object.ReferenceEquals(refA, refB))
            {
                return true;
            }

            if (Object.ReferenceEquals(null, refA))
            {
                return false;
            }

            return (refA.Equals(refB));
        }

        public static bool operator !=(Reference refA, Reference refB)
        {
            return !(refA == refB);
        }
    }
}