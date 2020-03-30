﻿using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class MeaCustomClaimHandler : AuthorizationHandler<MeaCustomClaimRequirement>
    {
        public const string Aud = "69d9693e-c4b7-4294-a29f-cddaebfa518b";
        public readonly List<string> Tenant = new List<string> { "159","189" };
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MeaCustomClaimRequirement requirement)
        {
            // If user does not have the audience claim, get out of here
            if (!context.User.HasClaim(c => c.Type == MeaCustomClaimAttributes.AudienceClaimTypeName))
                return Task.CompletedTask;

            // If user does not have the tenant claim, get out of here
            if (!context.User.HasClaim(c => c.Type == MeaCustomClaimAttributes.TenantClaimTypeName))
                return Task.CompletedTask;

            // Split the audience and tenants string into an array
            var audience = context.User.FindFirst(c => c.Type == MeaCustomClaimAttributes.AudienceClaimTypeName).Value.Split(' ');
            var tenant = context.User.FindFirst(c => c.Type == MeaCustomClaimAttributes.TenantClaimTypeName).Value.Split(' ');

            // Succeed if the audience and tenant array contains the required audience and tenants
            if (audience.Any(s => s == Aud) && Tenant.Any(x=>tenant.Contains(x)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}