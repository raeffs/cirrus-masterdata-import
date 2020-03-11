using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.External.Brickset
{
    class ThemeDto
    {
        public string Theme { get; set; }
    }

    class ArrayOfThemesDto
    {
        public List<ThemeDto> Themes { get; set; }
    }

    class ThemeCollectionDto
    {
        public ArrayOfThemesDto ArrayOfThemes { get; set; }
    }
}