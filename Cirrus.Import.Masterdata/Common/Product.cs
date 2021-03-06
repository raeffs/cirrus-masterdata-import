﻿using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Common
{
    class Product : BaseModel
    {
        public Length64String Name { get; set; }

        public string ExternalAssortmentId { get; set; }

        public Unit ExternalUnit { get; set; }

        public Tax ExternalTax { get; set; }

        public Group ExternalGroup { get; set; }

        public Barcode Barcode { get; set; }

        public Price Price { get; set; }

        public string Picture { get; set; }

        public List<string> ExternalCategoryIds { get; set; } = new List<string>();

        public List<string> MinAgesForYouthProtection { get; set; } = new List<string>();
    }
}
