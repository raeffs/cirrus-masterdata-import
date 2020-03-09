using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class DetailViewModel<TProperties, TLists>
        where TProperties : new()
        where TLists : new()
    {
        public TProperties Properties { get; set; } = new TProperties();

        public TLists Lists { get; set; } = new TLists();

        public List<object> ValidationErrors { get; set; }

        [JsonIgnore]
        public bool IsValid => this.ValidationErrors == null || this.ValidationErrors.Count == 0;
    }
}
