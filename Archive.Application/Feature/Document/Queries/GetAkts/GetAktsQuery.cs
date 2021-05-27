using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Document;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetAkts
{
    public class GetAktsQuery : IRequest<IList<AktDto>>
    {
    }

    public class GetAktsQueryHandler : IRequestHandler<GetAktsQuery,IList<AktDto>>
    {
        private readonly MongoDbOptions _options;

        public GetAktsQueryHandler(IOptions<MongoDbOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IList<AktDto>> Handle(GetAktsQuery request, CancellationToken cancellationToken)
        {

            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var aktCollection = database.GetCollection<Core.Collections.Document.Document>(_options.Collections.Documents);

            var filter = Builders<Core.Collections.Document.Document>.Filter
                .Eq("Type", DocumentTypeEnum.Акт);

            var akts = await aktCollection.Find(filter).As<AktDto>().ToListAsync(cancellationToken);

            return akts;
        }
    }
}