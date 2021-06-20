using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.Document.Queries.GetAllDocuments;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetEmployeeDocuments
{
    public class GetEmployeeDocumentsQuery : IRequest<IList<DocumentDto>>
    {
    }

    public class GetEmployeeDocumentsQueryHandler : IRequestHandler<GetEmployeeDocumentsQuery, IList<DocumentDto>>
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetEmployeeDocumentsQueryHandler(IOptions<MongoDbOptions> mongoDbOptions,
            IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<DocumentDto>> Handle(GetEmployeeDocumentsQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database.GetCollection<DocumentDto>(_mongoDbOptions.Collections.Documents);
            var requisitionsCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);
            var documentIds = requisitionsCollection.AsQueryable()
                .Where(requisition => requisition.RecipientId == _currentUserService.UserId
                                      &&
                                      requisition.DateOfGiveOut.HasValue
                                      &&
                                      !requisition.DateOfReturn.HasValue
                                      &&
                                      !requisition.Canceled
                                      &&
                                      !requisition.IsDenied)
                .Select(requisition => requisition.Documents)
                .ToList();

            var documentIdsUnique = documentIds
                .SelectMany(l => l.Select(docId => docId))
                .ToList();

            return await Task.FromResult(documentsCollection.AsQueryable()
                .Where(doc => documentIdsUnique.Contains(doc.Id))
                .ToList());
        }
    }
}