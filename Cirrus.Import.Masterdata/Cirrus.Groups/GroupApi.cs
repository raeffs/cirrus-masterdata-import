using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.Cirrus.Groups
{
    class GroupApi : MappedApi<Group>
    {
        private readonly GroupOptions groupOptions;

        public GroupApi(GroupOptions groupOptions)
        {
            this.groupOptions = groupOptions;
        }

        protected override Task<IEnumerable<Mapping<Group>>> LoadMappingsAsync(string key, IEnumerable<Group> values)
        {
            return Task.FromResult(new List<Mapping<Group>>
            {
                new Mapping<Group> { Value = Group.Default, Id = this.groupOptions.DefaultGroupId.ToString() },
            }.AsEnumerable());
        }
    }
}
