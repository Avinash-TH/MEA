﻿using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public HttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient;
        }        

        public async Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            var response = await _meaClient.GetAsync(url).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<string[]>(response);
        }

        public async Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {            
            return await _meaClient.GetAsync(url).ConfigureAwait(false);
        }
    }
}

