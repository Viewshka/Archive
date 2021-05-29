using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Queries.GetRequisitionPreview
{
    public class GetRequisitionPreviewQuery : IRequest<MemoryStream>
    {
        public string Id { get; set; }
        public string WebRootPath { get; set; }
    }

    public class GetRequisitionPreviewQueryHandler : IRequestHandler<GetRequisitionPreviewQuery, MemoryStream>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetRequisitionPreviewQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<MemoryStream> Handle(GetRequisitionPreviewQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionsCollection = database
                .GetCollection<RequisitionDto>(_mongoDbOptions.Collections.Requisitions);
            var filter = Builders<RequisitionDto>.Filter.Eq("_id", request.Id);

            var requisition = await requisitionsCollection.Find(filter)
                .SingleOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(requisition.Path))
                return new MemoryStream();

            var path = $"{request.WebRootPath}/{requisition.Path}";
            var file = System.IO.File.Open(path, FileMode.Open);

            var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            file.Close();
            return ms;
        }
    }
}