using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.DeleteRequisition
{
    public class DeleteRequisitionCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteRequisitionCommandHandler : IRequestHandler<DeleteRequisitionCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public DeleteRequisitionCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(DeleteRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection =
                database.GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var filter = Builders<Core.Collections.Requisition>.Filter.Eq("_id", request.Id);
            var result = await requisitionCollection.DeleteOneAsync(filter, cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время удаления произошла ошибка");

            return Unit.Value;
        }
    }
}