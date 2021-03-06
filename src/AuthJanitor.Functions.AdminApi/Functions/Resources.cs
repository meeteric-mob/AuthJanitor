﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.Services;
using AuthJanitor.UI.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthJanitor.Functions
{
    /// <summary>
    /// API functions to control the creation and management of AuthJanitor Resources.
    /// A Resource is the description of how to connect to an object or resource, using a given Provider.
    /// </summary>
    public class Resources
    {
        private readonly ResourcesService _service;

        public Resources(ResourcesService service)
        {
            _service = service;
        }

        [FunctionName("Resources-Create")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "resources")] ResourceViewModel resource,
            HttpRequest req, CancellationToken cancellationToken)
        {
            return await _service.Create(resource, req, cancellationToken);
        }

        [FunctionName("Resources-List")]
        public async Task<IActionResult> List([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "resources")] HttpRequest req, CancellationToken cancellationToken)
        {
            return await _service.List(req, cancellationToken);
        }

        [FunctionName("Resources-Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "resources/{resourceId:guid}")] HttpRequest req,
            Guid resourceId, CancellationToken cancellationToken)
        {
            return await _service.Get(req, resourceId, cancellationToken);
        }

        [FunctionName("Resources-Delete")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "resources/{resourceId:guid}")] HttpRequest req,
            Guid resourceId, CancellationToken cancellationToken)
        {
            return await _service.Delete(req, resourceId, cancellationToken);
        }

        [FunctionName("Resources-Update")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "resources/{resourceId:guid}")] ResourceViewModel resource,
            HttpRequest req,
            Guid resourceId, CancellationToken cancellationToken)
        {
            return await _service.Update(resource, req, resourceId, cancellationToken);
        }
    }
}
