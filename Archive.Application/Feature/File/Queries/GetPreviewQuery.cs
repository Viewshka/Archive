using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.File.Queries
{
    public class GetPreviewQuery : IRequest<MemoryStream>
    {
        public string Id { get; set; }
        public string WebRootPath { get; set; }
    }

    public class GetPreviewQueryHandler : IRequestHandler<GetPreviewQuery, MemoryStream>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetPreviewQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<MemoryStream> Handle(GetPreviewQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database
                .GetCollection<DocumentDto>(_mongoDbOptions.Collections.Documents);
            var filter = Builders<DocumentDto>.Filter.Eq("_id", request.Id);

            var document = await documentsCollection.Find(filter)
                .SingleOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(document.Path))
                return new MemoryStream();

            var path = $"{request.WebRootPath}/{document.Path}";
            var file = System.IO.File.Open(path, FileMode.Open);

            var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            file.Close();
            return ms;
        }
    }
}