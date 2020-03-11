namespace Cirrus.Import.Masterdata.Common
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    class Assortment : BaseModel
    {
        public string Name => $"{this.ExternalId} ({this.ExternalKey})";
    }
}
