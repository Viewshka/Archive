using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.CanceledRequisition
{
    public class CanceledRequisitionCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class CanceledRequisitionCommandHandler : IRequestHandler<CanceledRequisitionCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public CanceledRequisitionCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(CanceledRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var filter = Builders<Core.Collections.Requisition>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Requisition>.Update.Set("Canceled", true);

            await requisitionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
    }
}