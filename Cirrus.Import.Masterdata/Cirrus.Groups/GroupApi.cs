using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.Cirrus.Groups
{
    class GroupApi
    {
        private readonly GroupOptions groupOptions;

        public GroupApi(GroupOptions groupOptions)
        {
            this.groupOptions = groupOptions;
        }

        public Task<List<CustomMapping<Group>>> GetMappingsAsync()
        {
            return Task.FromResult(new List<CustomMapping<Group>>
            {
                new CustomMapping<Group> { Value = Group.Default, Id = this.groupOptions.DefaultGroupId.ToString() },
            });
        }
    }
}
