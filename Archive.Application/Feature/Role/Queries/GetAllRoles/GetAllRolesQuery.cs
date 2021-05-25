using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Identity;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Role.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<IList<RoleDto>>
    {
    }

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IList<RoleDto>>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllRolesQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var rolesCollection = database.GetCollection<ApplicationRole>(_mongoDbOptions.Collections.Roles);

            var filter = new BsonDocument();
            return await rolesCollection.Find(filter).As<RoleDto>().ToListAsync(cancellationToken);
        }
    }
}