using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Identity;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.User.Commands.SetUserPriority
{
    public class SetUserPriorityCommand : IRequest
    {
        public Priority Priority { get; set; }
        public string Id { get; set; }
    }

    public class SetUserPriorityCommandHandler : IRequestHandler<SetUserPriorityCommand>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public SetUserPriorityCommandHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(SetUserPriorityCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var usersCollection = database.GetCollection<ApplicationUser>(_mongoDbOptions.Collections.Users);

            var filter = Builders<ApplicationUser>.Filter.Eq("_id", request.Id);
            var update = Builders<ApplicationUser>.Update.Set("Priority", request.Priority);

            await usersCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}