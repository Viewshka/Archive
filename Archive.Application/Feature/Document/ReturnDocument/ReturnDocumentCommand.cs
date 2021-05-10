using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.ReturnDocument
{
    public class ReturnDocumentCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class ReturnDocumentCommandHandler : IRequestHandler<ReturnDocumentCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public ReturnDocumentCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(ReturnDocumentCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var usersCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var filter = Builders<Core.Collections.Requisition>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Requisition>.Update.Set("DateOfReturn", DateTime.Now);

            await usersCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}