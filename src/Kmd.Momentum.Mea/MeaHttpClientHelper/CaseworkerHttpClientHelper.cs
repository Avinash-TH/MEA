﻿using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class CaseworkerHttpClientHelper : ICaseworkerHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public CaseworkerHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
        }

        public async Task<ResultOrHttpError<MeaBaseList, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url, int pageNumber)
        {
            var pageSize = 50;
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            List<MeaCaseworkerDataResponseModel> totalRecords = new List<MeaCaseworkerDataResponseModel>();

            var queryStringParams = $"pagingInfo.pageNumber={pageNumber}&pagingInfo.pageSize={pageSize}";

            var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<MeaBaseList, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            var caseworkerDataObj = JsonConvert.DeserializeObject<McaPUnitData>(content);
            var records = caseworkerDataObj.Data;

            foreach (var item in records)
            {
                var dataToReturn = new MeaCaseworkerDataResponseModel(item.Id, item.DisplayName, item.GivenName, item.MiddleName, item.Initials,
               item.Email?.Address, item.Phone?.Number, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                totalRecords.Add(dataToReturn);
            }

            var responseData = new MeaBaseList()
            {
                PageNo = pageNumber,
                TotalNoOfPages = caseworkerDataObj.TotalPages,
                TotalSearchCount = caseworkerDataObj.TotalSearchCount,
                Result = totalRecords
            };

            return new ResultOrHttpError<MeaBaseList, Error>(responseData);
        }

        public async Task<ResultOrHttpError<string, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(Uri url)
        {
            var response = await _meaClient.GetAsync(url).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }
    }
}