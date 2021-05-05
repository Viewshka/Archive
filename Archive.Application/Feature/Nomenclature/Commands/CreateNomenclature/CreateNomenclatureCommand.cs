using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Commands.CreateNomenclature
{
    public class CreateNomenclatureCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Index { get; set; }
        public string DepartmentId { get; set; }
        public int Year { get; set; }
    }

    public class CreateNomenclatureCommandHandler : IRequestHandler<CreateNomenclatureCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateNomenclatureCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<string> Handle(CreateNomenclatureCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var nomenclaturesCollection =
                database.GetCollection<Core.Collections.Nomenclature>(_mongoDbOptions.Collections.Nomenclatures);

            var entity = new Core.Collections.Nomenclature
            {
                Id = Guid.NewGuid().ToString(),
                Index = request.Index,
                Name = request.Name,
                Year = request.Year,
                DepartmentId = request.DepartmentId
            };

            await nomenclaturesCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity.Id;
        }
    }
}