using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.GiveOutDocument
{
    public class GiveOutDocumentCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class GiveOutDocumentCommandHandler : IRequestHandler<GiveOutDocumentCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GiveOutDocumentCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }
        public async Task<Unit> Handle(GiveOutDocumentCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection =
                database.GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var filter = Builders<Core.Collections.Requisition>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Requisition>.Update
                .Set("GiverId", _currentUserService.UserId)
                .Set("DateOfGiveOut", DateTime.Now);

            await requisitionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
    }
}