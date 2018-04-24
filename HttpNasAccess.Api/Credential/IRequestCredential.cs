using System.Net.Http;
using HttpNasAccess.Api.Model;

namespace HttpNasAccess.Api.Credential
{
    public interface IRequestCredential
    {
        NasIdentity GetCurrentCredential(HttpRequestMessage request);
    }
}
