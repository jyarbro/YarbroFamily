using HtmlAgilityPack;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Utilities {
    public class GzipWebClient : WebClient {
        public async Task<HtmlDocument> DownloadDocument(string remoteUrl) {
            var data = await DownloadStringSafe(remoteUrl);

            HtmlDocument returnObject = null;

            if (!string.IsNullOrEmpty(data)) {
                returnObject = new HtmlDocument();
                returnObject.LoadHtml(data);
            }

            return returnObject;
        }

        public async Task<T> DownloadJSObject<T>(string remoteUrl, JsonSerializerOptions options = null) {
            var data = await DownloadStringSafe(remoteUrl);

            var returnObject = default(T);

            if (data?.Length > 0) {
                try {
                    if (options is null) {
                        returnObject = JsonSerializer.Deserialize<T>(data);
                    }
                    else {
                        returnObject = JsonSerializer.Deserialize<T>(data, options);
                    }
                }
                catch (JsonException) { }
                catch (NotSupportedException) { }
            }

            return returnObject;
        }

        public async Task<string> DownloadStringSafe(string remoteUrl) {
            remoteUrl = CleanUrl(remoteUrl);

            var data = string.Empty;

            try {
                data = await DownloadStringTaskAsync(remoteUrl);
            }
            catch (UriFormatException) { }
            catch (AggregateException) { }
            catch (ArgumentException) { }
            catch (WebException) { }

            return data;
        }

        protected override WebRequest GetWebRequest(Uri remoteUri) {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = base.GetWebRequest(remoteUri) as HttpWebRequest;

            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246";
            request.AllowAutoRedirect = true;
            request.MaximumAutomaticRedirections = 3;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Timeout = 5000;
            request.CookieContainer = new CookieContainer();

            return request;
        }

        string CleanUrl(string remoteUrl) {
            if (remoteUrl?.Length > 0) {
                throw new ArgumentException($"Argument {nameof(remoteUrl)} cannot be empty or null.");
            }

            return remoteUrl.Split('#')[0];
        }
    }
}
