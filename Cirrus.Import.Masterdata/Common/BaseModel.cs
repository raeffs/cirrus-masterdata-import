using System.Security.Cryptography;
using System.Text;

namespace Cirrus.Import.Masterdata.Common
{
    class BaseModel
    {
        public string ExternalKey { get; set; }

        public string ExternalId { get; set; }

        public string UniqueId => this.CalculateUniqueId();

        protected string CalculateUniqueId()
        {
            using MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes($"{this.ExternalKey}-{this.ExternalId}"));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
