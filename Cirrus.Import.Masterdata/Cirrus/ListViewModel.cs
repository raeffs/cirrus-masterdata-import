using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class ListViewModel<T>
    {
        public List<T> Data { get; set; } = new List<T>();

        public ListViewModelInfo Info { get; set; } = new ListViewModelInfo();
    }
}
