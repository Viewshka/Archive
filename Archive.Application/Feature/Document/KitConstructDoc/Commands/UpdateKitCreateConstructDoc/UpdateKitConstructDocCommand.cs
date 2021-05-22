using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.KitConstructDoc.Commands.UpdateKitCreateConstructDoc
{
    public class UpdateKitConstructDocCommand : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime? IncomingDate { get; set; }
        public string NomenclatureId { get; set; }
        public string ParentId { get; set; }
        public DateTime? StorageDate { get; set; }
        public Priority Priority { get; set; }
    }

    public class UpdateKitConstructDocCommandHandler : IRequestHandler<UpdateKitConstructDocCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public UpdateKitConstructDocCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(UpdateKitConstructDocCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection =
                database.GetCollection<Core.Collections.Document.KitConstructDoc>(_mongoDbOptions.Collections.Documents);

            var filter = Builders<Core.Collections.Document.KitConstructDoc>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Document.KitConstructDoc>.Update
                .Set("Designation", request.Designation)
                .Set("DocumentDate", request.DocumentDate)
                .Set("IncomingDate", request.IncomingDate)
                .Set("NomenclatureId", request.NomenclatureId)
                .Set("ParentId", request.ParentId)
                .Set("Name", request.Name)
                .Set("StorageDate", request.StorageDate)
                .Set("Note", request.Note)
                .Set("Priority",request.Priority);

            var result = await documentsCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Ошибка обновления документа");

            return Unit.Value;
        }
    }
}