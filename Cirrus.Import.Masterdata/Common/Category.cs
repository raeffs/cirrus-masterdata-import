namespace Cirrus.Import.Masterdata.Common
{
    class Category : BaseModel
    {
        public string Name => $"{this.ExternalId} ({this.ExternalKey})";

        public string ExternalParentId { get; set; }

        public bool IsChild => !string.IsNullOrWhiteSpace(this.ExternalParentId);
    }
}
