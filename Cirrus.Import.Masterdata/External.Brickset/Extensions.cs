using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Xml;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Brickset
{
    static class Extensions
    {
        public static async Task<T> GetJsonFromXmlAsync<T>(this IFlurlRequest request)
        {
            var xDoc = await request.GetXDocumentAsync();
            var str = JsonConvert.SerializeXNode(xDoc);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
