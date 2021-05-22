using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.Nomenclature.Queries.GetAllNomenclatures;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Queries.GetDocumentsByNomenclature
{
    public class GetDocumentsByNomenclatureQuery : IRequest<IList<DocumentsByNomenclatureDto>>
    {
        public string NomenclatureId { get; set; }
    }

    public class GetDocumentsByNomenclatureQueryHandler : IRequestHandler<GetDocumentsByNomenclatureQuery,
        IList<DocumentsByNomenclatureDto>>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetDocumentsByNomenclatureQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<DocumentsByNomenclatureDto>> Handle(GetDocumentsByNomenclatureQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database
                .GetCollection<DocumentsByNomenclatureDto>(_mongoDbOptions.Collections.Documents);

            var filter = Builders<DocumentsByNomenclatureDto>.Filter.Eq("NomenclatureId", request.NomenclatureId);
            var result = await documentsCollection
                .Find(filter)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}