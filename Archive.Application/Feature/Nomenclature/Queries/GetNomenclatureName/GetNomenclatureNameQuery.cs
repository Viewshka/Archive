using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Common;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Archive.Application.Feature.Nomenclature.Queries.GetNomenclatureName
{
    public class GetNomenclatureNameQuery : IRequest<string>
    {
        public string Id { get; set; }
    }

    public class GetNomenclatureNameQueryHandler : IRequestHandler<GetNomenclatureNameQuery,string>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetNomenclatureNameQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<string> Handle(GetNomenclatureNameQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var nomenclaturesCollection = database
                .GetCollection<Core.Collections.Nomenclature>(_mongoDbOptions.Collections.Nomenclatures);

            var filter = Builders<Core.Collections.Nomenclature>.Filter.Eq("_id", request.Id);
            var nomenclature = await nomenclaturesCollection.Find(filter).SingleOrDefaultAsync(cancellationToken);
            
            return $"{nomenclature.Index} - {nomenclature.Name}";
        }
    }
}