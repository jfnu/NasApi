using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpNasAccess.Api.Model;

namespace HttpNasAccess.Api.Extractor
{
    public interface INasRequestExtractor
    {
        string StagingFolder { get; set; }
        ResponseMessage Response { get; set; }
        Collection<MultipartFileData> FileData { get; }
        NameValueCollection NonFileData { get;  }
        Task<bool> Extract(HttpRequestMessage request);
    }
}
