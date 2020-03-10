using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus.Categories
{
    class CategoryDetailViewModelProperties
    {
        public string Id { get; set; } = "0";

        public string Name { get; set; }

        public string ExternalId { get; set; }

        public bool MaxOneNodePerProductAssignable { get; set; } = true;

        public List<Reference> Parent { get; set; }
    }
}
