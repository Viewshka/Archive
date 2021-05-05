using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections;
using Archive.Core.Collections.Document;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Commands.CreateDraw
{
    public class CreateDrawCommand : IRequest<string>
    {
        /// <summary>
        /// Наименовнаие документа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Обозначение
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Дата документа
        /// </summary>
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Тип документа
        /// </summary>
        public DocumentTypeEnum Type { get; set; }
    }

    public class CreateDrawCommandHandler : IRequestHandler<CreateDrawCommand,string>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateDrawCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<string> Handle(CreateDrawCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database.GetCollection<Draw>(_mongoDbOptions.Collections.Documents);
            
            await documentsCollection.InsertOneAsync(new Draw(), cancellationToken: cancellationToken);
            
            return "";
        }
    }
}