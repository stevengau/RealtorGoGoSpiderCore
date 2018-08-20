using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RealtorGoGoSpider
{
    public class OwnHttpClient
    {
        
       // private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 100 });

        public static async Task<string> GetResponseContent(string url)
        {
            try
            {
                using (HttpClient _httpClient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 100 }))
                {
                    _httpClient.BaseAddress = new Uri("https://cors-anywhere.herokuapp.com/");
                    var result = _httpClient.GetAsync(new Uri(url)).Result;

                    return await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                System.Threading.Thread.Sleep(10000);
                return await GetResponseContent(url);
            }
            

        }
    }
}
