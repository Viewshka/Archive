using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetInventory
{
    public class GetInventoryQuery : IRequest<IList<InventoryDto>>
    {
    }

    public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, IList<InventoryDto>>
    {
        private readonly MongoDbOptions _options;

        public GetInventoryQueryHandler(IOptions<MongoDbOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IList<InventoryDto>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var documentCollection = database.GetCollection<Core.Collections.Document.Document>(_options.Collections.Documents);

            var filter = Builders<Core.Collections.Document.Document>.Filter
                .Eq("Type", DocumentTypeEnum.ОписьДела);

            var inventories = await documentCollection
                .Find(filter).As<InventoryDto>()
                .ToListAsync(cancellationToken);

            return inventories;
        }
    }
}