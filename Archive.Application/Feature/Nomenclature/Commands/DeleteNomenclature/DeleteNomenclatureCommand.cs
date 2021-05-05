using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Commands.DeleteNomenclature
{
    public class DeleteNomenclatureCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteNomenclatureCommandHandler : IRequestHandler<DeleteNomenclatureCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public DeleteNomenclatureCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(DeleteNomenclatureCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var nomenclaturesCollection =
                database.GetCollection<Core.Collections.Nomenclature>(_mongoDbOptions.Collections.Nomenclatures);

            var filter = Builders<Core.Collections.Nomenclature>.Filter.Eq("_id", request.Id);
            var result = await nomenclaturesCollection.DeleteOneAsync(filter, cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время удаления произошла ошибка");

            return Unit.Value;
        }
    }
}