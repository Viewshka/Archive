using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.GenerateAktAboutDocumentGiveOut
{
    public class GenerateAktAboutDocumentGiveOutCommand : IRequest
    {
        public string RequisitionId { get; set; }
    }

    public class GenerateAktAboutDocumentGiveOutCommandHandler :
        IRequestHandler<GenerateAktAboutDocumentGiveOutCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GenerateAktAboutDocumentGiveOutCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(GenerateAktAboutDocumentGiveOutCommand request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            //Todo: нужно что-то с этим сделать

            return Unit.Value;
        }
    }
}