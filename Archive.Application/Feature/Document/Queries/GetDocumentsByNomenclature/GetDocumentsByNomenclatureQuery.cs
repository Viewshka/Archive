using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetDocumentsByNomenclature
{
    public class GetDocumentsByNomenclatureQuery : IRequest<IList<DocumentsByNomenclatureDto>>
    {
        public string NomenclatureId { get; set; }
    }

    public class GetDocumentsByNomenclatureQueryHandler : IRequestHandler<GetDocumentsByNomenclatureQuery,
        IList<DocumentsByNomenclatureDto>>
    {
        private readonly IMediator _mediator;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetDocumentsByNomenclatureQueryHandler(IOptions<MongoDbOptions> mongoDbOptions,
            IMediator mediator)
        {
            _mediator = mediator;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<DocumentsByNomenclatureDto>> Handle(GetDocumentsByNomenclatureQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentsCollection = database
                .GetCollection<DocumentsByNomenclatureDto>(_mongoDbOptions.Collections.Documents);
            var currentUser = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);

            var builder = Builders<DocumentsByNomenclatureDto>.Filter;

            var filter = builder.Eq("NomenclatureId", request.NomenclatureId) &
                         builder.Gte("Priority", currentUser.Priority);

            var result = await documentsCollection
                .Find(filter)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}