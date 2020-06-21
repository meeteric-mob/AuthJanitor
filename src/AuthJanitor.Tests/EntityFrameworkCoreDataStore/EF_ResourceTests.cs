﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.Providers;
using AuthJanitor.UI.Shared.Models;
using System;

namespace AuthJanitor.Tests.EntityFrameworkCoreDataStore
{
    public class EF_ResourceTests : EF_TestsBase<Resource>
    {
        protected override Resource CreateModel()
        {
            return new Resource()
            {
                ObjectId = Guid.NewGuid(),
                Description = "Description",
                IsRekeyableObjectProvider = true,
                Name = "Name",
                ProviderConfiguration = "ProviderConfiguration",
                ProviderId = ProviderIdentifier.FromString("ProviderType")
            };
        }

        protected override Resource UpdatedModel()
        {
            return new Resource()
            {
                ObjectId = model.ObjectId,
                Description = "Another Description",
                IsRekeyableObjectProvider = false,
                Name = "Another Name",
                ProviderConfiguration = "Another ProviderConfiguration",
                ProviderId = ProviderIdentifier.FromString("Another ProviderType")
            };
        }

        protected override bool CompareModel(Resource model1, Resource model2)
        {
            if (model1.ObjectId != model2.ObjectId ||
                model1.Description != model2.Description ||
                model1.IsRekeyableObjectProvider != model2.IsRekeyableObjectProvider ||
                model1.Name != model2.Name ||
                model1.ProviderConfiguration != model2.ProviderConfiguration ||
                model1.ProviderId != model2.ProviderId)
                return false;
            return true;
        }
    }
}
