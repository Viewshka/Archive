using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Queries.GetAllNomenclatures
{
    public class GetAllNomenclaturesQuery : IRequest<IList<NomenclatureDto>>
    {
    }

    public class GetAllNomenclaturesQueryHandler : IRequestHandler<GetAllNomenclaturesQuery,IList<NomenclatureDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllNomenclaturesQueryHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<NomenclatureDto>> Handle(GetAllNomenclaturesQuery request, 
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var nomenclaturesCollection = database
                .GetCollection<NomenclatureDto>(_mongoDbOptions.Collections.Nomenclatures);
            var filter = new BsonDocument();

            return await nomenclaturesCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}