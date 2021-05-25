using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.User.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<IList<UserDto>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery,IList<UserDto>>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllUsersQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var usersCollection = database.GetCollection<UserDto>(_mongoDbOptions.Collections.Users);

            var filter = new BsonDocument();

            return await usersCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}