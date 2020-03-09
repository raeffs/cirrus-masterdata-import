namespace Cirrus.Import.Masterdata.Common
{
    class Assortment : BaseModel
    {
        public string Name => $"{this.ExternalId} ({this.ExternalKey})";
    }
}
