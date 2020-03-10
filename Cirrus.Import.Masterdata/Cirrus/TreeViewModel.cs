using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class TreeViewModel<T>
    {
        public List<T> Data { get; set; } = new List<T>();
    }
}
