// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.DataStores;
using AuthJanitor.Providers;
using System;

namespace AuthJanitor.UI.Shared.Models
{
    public class Resource : IAuthJanitorModel
    {
        public Guid ObjectId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsRekeyableObjectProvider { get; set; }
        public ProviderIdentifier ProviderId { get; set; }
        public string ProviderConfiguration { get; set; }
    }
}
