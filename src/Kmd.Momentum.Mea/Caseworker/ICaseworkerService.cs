﻿using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker1
{
   public interface ICaseworkerService
    {
       

        Task<IReadOnlyList<CaseworkerDataResponseModel>> GetCaseworkerIdAsync();


    }
}