using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sandbox.Services
{
    internal class YoutubeService: IYoutubeService
    {
        private readonly HttpClient _httpClient;

        public YoutubeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IReadOnlyCollection<string>> GetVideosUrls()
        {
            throw new System.NotImplementedException();
        }
    }

    internal interface IYoutubeService
    {
        Task<IReadOnlyCollection<string>> GetVideosUrls();
    }
}