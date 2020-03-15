using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class SetDto
    {
        public string Name { get; set; }
    }

    class SetCollectionDto
    {
        public List<SetDto> Sets { get; set; }
    }
}
