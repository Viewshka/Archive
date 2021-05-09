using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Queries.GetRequisitions
{
    public class GetRequisitionsQuery : IRequest<IList<RequisitionDto>>
    {
    }

    public class GetRequisitionsQueryHandler : IRequestHandler<GetRequisitionsQuery, IList<RequisitionDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetRequisitionsQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<RequisitionDto>> Handle(GetRequisitionsQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionsCollection = database
                .GetCollection<RequisitionDto>(_mongoDbOptions.Collections.Requisitions);

            var filter = new BsonDocument();

            return await requisitionsCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}