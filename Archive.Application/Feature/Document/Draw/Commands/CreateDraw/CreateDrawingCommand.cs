using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Common;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Draw.Commands.CreateDraw
{
    public class CreateDrawingCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
        public DateTime? IncomingDate { get; set; }
        public string NomenclatureId { get; set; }
        public string ParentId { get; set; }
        public DateTime? StorageDate { get; set; }
    }

    public class CreateDrawingCommandHandler : IRequestHandler<CreateDrawingCommand,string>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateDrawingCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<string> Handle(CreateDrawingCommand request, CancellationToken cancellationToken)
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
                DocumentDate = request.DocumentDate,
                IncomingDate = request.IncomingDate ?? DateTime.Now,
                NomenclatureId = request.NomenclatureId,
                ParentId = request.ParentId,
                StorageDate = request.StorageDate
            };

            await documentsCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity.Id;
        }
    }
}