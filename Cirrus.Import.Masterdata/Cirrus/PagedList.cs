using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class PagedList<T>
    {
        public List<T> Items { get; set; }

        public int Count { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        [JsonIgnore]
        public bool HasMore => this.CurrentPage * this.PageSize < this.Count;

        [JsonIgnore]
        public int NextPage => this.CurrentPage + 1;
    }
}
