using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.User.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<CurrentUserDto>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var usersCollection = database.GetCollection<CurrentUserDto>(_mongoDbOptions.Collections.Users);

            var filter = Builders<CurrentUserDto>.Filter.Eq("_id", _currentUserService.UserId);
            var currentUser = await usersCollection.Find(filter).SingleOrDefaultAsync(cancellationToken);

            if (currentUser == null) throw new Exception("Пользователь не найден");

            currentUser.SetBriefName();
            currentUser.IsUserArchivist = currentUser.Roles
                .Any(role => role == Roles.Архивариус);

            return currentUser;
        }
    }
}