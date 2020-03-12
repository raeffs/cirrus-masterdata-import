namespace Cirrus.Import.Masterdata.Common
{
    class Category : BaseModel
    {
        private bool allowsMultipleAssignments;

        public string Name => $"{this.ExternalId} ({this.ExternalKey})";

        public string ExternalParentId { get; set; }

        public bool IsChild => !string.IsNullOrWhiteSpace(this.ExternalParentId);

        public bool AllowsMultipleAssignments
        {
            set { this.allowsMultipleAssignments = value; }
            get { return !this.IsChild && this.allowsMultipleAssignments; }
        }
    }
}
