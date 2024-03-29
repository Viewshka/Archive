﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.DeniedRequisition
{
    public class DeniedRequisitionCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeniedRequisitionCommandHandler : IRequestHandler<DeniedRequisitionCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public DeniedRequisitionCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(DeniedRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var filter = Builders<Core.Collections.Requisition>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Requisition>.Update.Set("IsDenied", true);

            await requisitionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}