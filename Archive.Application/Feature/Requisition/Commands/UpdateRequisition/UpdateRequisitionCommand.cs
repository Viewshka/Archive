using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.UpdateRequisition
{
    public class UpdateRequisitionCommand : IRequest
    {
        public string Id { get; set; }
        public IList<string> Documents { get; set; }
    }

    public class UpdateRequisitionCommandHandler : IRequestHandler<UpdateRequisitionCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public UpdateRequisitionCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(UpdateRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection =
                database.GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Nomenclatures);

            var filter = Builders<Core.Collections.Requisition>.Filter
                .Eq("_id", request.Id);

            var update = Builders<Core.Collections.Requisition>.Update
                .Set("Documents", request.Documents);

            var result = await requisitionCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время изменения произошла ошибка");

            return Unit.Value;
        }
    }
}