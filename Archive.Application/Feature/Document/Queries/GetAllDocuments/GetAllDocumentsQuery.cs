using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetAllDocuments
{
    public class GetAllDocumentsQuery : IRequest<IList<DocumentDto>>
    {
    }

    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IList<DocumentDto>>
    {
        private readonly IMediator _mediator;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllDocumentsQueryHandler(IOptions<MongoDbOptions> mongoDbOptions,
            IMediator mediator)
        {
            _mediator = mediator;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database.GetCollection<DocumentDto>(_mongoDbOptions.Collections.Documents);

            var currentUser = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            var builder = Builders<DocumentDto>.Filter;
            var filter = builder.Gte("Priority", currentUser.Priority) &
                         builder.Ne("Type", DocumentTypeEnum.Заявка) &
                         builder.Ne("Type", DocumentTypeEnum.ОписьДела);

            return await documentsCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}