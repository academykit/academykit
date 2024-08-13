using Newtonsoft.Json;

namespace AcademyKit.Infrastructure.Helpers
{
    public static class HttpClientUtils
    {
        /// <summary>
        /// Handle to download file
        /// </summary>
        /// <param name="client"> the instance of <see cref="HttpClient" /> .</param>
        /// <param name="uri"> the instance of <see cref="Uri" /> .</param>
        /// <param name="FileName"> the file name </param>
        /// <returns> task complete </returns>
        public static async Task DownloadFileTaskAsync(
            this HttpClient client,
            Uri uri,
            string FileName
        )
        {
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }

        /// <summary>
        /// Fetch remote image tags
        /// </summary>
        /// <param name="registryUrl">docker image registry .</param>
        /// <param name="imageName">docker repo name .</param>
        /// <returns> task complete </returns>
        public static async Task<string[]> GetImageTagsAsync(string registryUrl, string imageName)
        {
            var httpClient = new HttpClient();

            var authenticationRequest = new HttpRequestMessage(HttpMethod.Get, $"https://{registryUrl}/token?scope=repository:{imageName}:pull");
            var authenticationResponse = await httpClient.SendAsync(authenticationRequest);
            authenticationResponse.EnsureSuccessStatusCode();

            var tokenJson = await authenticationResponse.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeAnonymousType(tokenJson, new { token = "" });

            var listRequest = new HttpRequestMessage(HttpMethod.Get, $"https://{registryUrl}/v2/{imageName}/tags/list");
            listRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.token);

            var listResponse = await httpClient.SendAsync(listRequest);
            listResponse.EnsureSuccessStatusCode();

            var manifestJson = await listResponse.Content.ReadAsStringAsync();
            var manifest = JsonConvert.DeserializeAnonymousType(manifestJson, new { tags = Array.Empty<string>() });

            return manifest.tags;
        }
    }
}