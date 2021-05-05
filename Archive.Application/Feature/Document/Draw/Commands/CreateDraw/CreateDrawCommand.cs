using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Draw.Commands.CreateDraw
{
    public class CreateDrawCommand : IRequest
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
    }

    public class CreateDrawCommandHandler : IRequestHandler<CreateDrawCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateDrawCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(CreateDrawCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database.GetCollection<Core.Collections.Document.Draw>(_mongoDbOptions.Collections.Documents);

            var entity = new Core.Collections.Document.Draw
            {
                Name = request.Name, 
                Designation = request.Designation,
                Note = request.Note,
                Type = request.Type,
                DocumentDate = request.DocumentDate
            };

            await documentsCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}