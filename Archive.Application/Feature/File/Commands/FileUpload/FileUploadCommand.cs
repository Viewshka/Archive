using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.Document.Queries.GetAllDocuments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.File.Commands.FileUpload
{
    public class FileUploadCommand : IRequest
    {
        public string DocumentId { get; set; }
        public string WebRootPath { get; set; }
        public IFormFile File { get; set; }
    }

    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public FileUploadCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            var extension = request.File.FileName.Split('.').Last();
            var newFileName = $"{Guid.NewGuid().ToString()}.{extension}";
            var path = Path.Combine(request.WebRootPath, $"files/{newFileName}");
            await using var fileStream = new FileStream(path, FileMode.Create);
            await request.File.CopyToAsync(fileStream, cancellationToken);

            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database
                .GetCollection<Core.Collections.Document.Document>(_mongoDbOptions.Collections.Documents);

            var filter = Builders<Core.Collections.Document.Document>.Filter.Eq("_id", request.DocumentId);
            var update = Builders<Core.Collections.Document.Document>.Update
                .Set("Path", $"files/{newFileName}");

            await documentsCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
    }
}