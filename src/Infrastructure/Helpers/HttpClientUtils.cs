namespace Infrastructure.Helpers
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
    }
}
