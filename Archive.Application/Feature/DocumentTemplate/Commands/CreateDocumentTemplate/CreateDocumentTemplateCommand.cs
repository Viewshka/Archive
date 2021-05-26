using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.DocumentTemplate.Commands.CreateDocumentTemplate
{
    public class CreateDocumentTemplateCommand : IRequest
    {
        public string TemplateFileName { get; set; }
        public DocumentTemplateEnum Type { get; set; }
    }

    public class CreateDocumentTemplateCommandHandler : IRequestHandler<CreateDocumentTemplateCommand>
    {
        private readonly MongoDbOptions _options;

        public CreateDocumentTemplateCommandHandler(IOptions<MongoDbOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Unit> Handle(CreateDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var templateCollection = database
                .GetCollection<Core.Collections.DocumentTemplate>(_options.Collections.DocumentTemplates);

            var obj = new Core.Collections.DocumentTemplate
            {
                Path = $"template/{request.TemplateFileName}",
                Type = request.Type,
                FileName = request.TemplateFileName
            };
            
            await templateCollection.InsertOneAsync(obj, cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
    }
}