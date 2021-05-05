using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Commands.UpdateNoomenclature
{
    public class UpdateNomenclatureCommand : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public string DepartmentId { get; set; }
        public int Year { get; set; }
    }

    public class UpdateNomenclatureCommandHandler : IRequestHandler<UpdateNomenclatureCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public UpdateNomenclatureCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(UpdateNomenclatureCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var nomenclaturesCollection =
                database.GetCollection<Core.Collections.Nomenclature>(_mongoDbOptions.Collections.Nomenclatures);

            var filter = Builders<Core.Collections.Nomenclature>.Filter
                .Eq("_id", request.Id);

            var update = Builders<Core.Collections.Nomenclature>.Update
                .Set("Name", request.Name)
                .Set("Index", request.Index)
                .Set("DepartmentId", request.DepartmentId)
                .Set("Year", request.Year);

            var result = await nomenclaturesCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время изменения произошла ошибка");

            return Unit.Value;
        }
    }
}