﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Draw.Commands.UpdateDraw
{
    public class UpdateDrawCommand : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
    }

    public class UpdateDrawCommandHandler : IRequestHandler<UpdateDrawCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public UpdateDrawCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(UpdateDrawCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection =
                database.GetCollection<Core.Collections.Document.Draw>(_mongoDbOptions.Collections.Documents);

            var filter = Builders<Core.Collections.Document.Draw>.Filter.Eq("_id", request.Id);
            var update = Builders<Core.Collections.Document.Draw>.Update
                .Set("Designation", request.Designation)
                .Set("Note", request.Note)
                .Set("Name", request.Name);

            var result = await documentsCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Ошибка обновления документа");

            return Unit.Value;
        }
    }
}