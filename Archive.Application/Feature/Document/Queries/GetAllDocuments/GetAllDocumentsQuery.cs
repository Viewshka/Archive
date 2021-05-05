using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetAllDocuments
{
    public class GetAllDocumentsQuery : IRequest<IList<DocumentDto>>
    {
    }

    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery,IList<DocumentDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllDocumentsQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }
        public async Task<IList<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database.GetCollection<DocumentDto>(_mongoDbOptions.Collections.Documents);

            var filter = new BsonDocument();

            return await documentsCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}