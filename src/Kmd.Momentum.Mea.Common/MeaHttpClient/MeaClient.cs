﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public class MeaClient : IMeaClient
    {
        private static HttpClient _httpClient;
        private readonly IConfiguration _config;
        static readonly object _object = new object();

        public MeaClient(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> GetAsync(Uri url, string token)
        {
            Monitor.Enter(_object);
            if(CheckIfNewTokenNeeded(token))
            {
                var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

                var accessToken = (JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"]).ToString();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer "+ accessToken);
            }
            else
            {
                var accessToken = JObject.Parse(token)["access_token"];
                _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer " + accessToken);
            }
            Monitor.Exit(_object);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public Task<string> PostAsync(Uri uri, StringContent stringContent, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> ReturnAuthorizationTokenAsync()
        {
            var content = new FormUrlEncodedContent(new[]
           {
                        new KeyValuePair<string, string>("grant_type","client_credentials"),
                        new KeyValuePair<string, string>("client_id", _config["KMD_MOMENTUM_MEA_ClientId"]),
                        new KeyValuePair<string, string>("client_secret", _config["KMD_MOMENTUM_MEA_ClientSecret"]),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
                    });

            var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"), content).ConfigureAwait(false);
            return response;
        }

        private bool CheckIfNewTokenNeeded(JToken token)
        {
            try
            {
                var expiresIn = DateTimeOffset.UtcNow.AddSeconds((int)(JObject.Parse(token.ToString())["expires_in"]));

                if (expiresIn < DateTimeOffset.UtcNow)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return true;
            }
        }
    }
}
