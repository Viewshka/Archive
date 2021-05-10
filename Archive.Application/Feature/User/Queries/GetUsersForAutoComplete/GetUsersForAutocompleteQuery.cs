using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using Archive.Core.Collections.Identity;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.User.Queries.GetUsersForAutoComplete
{
    public class GetUsersForAutocompleteQuery : IRequest<IList<UserDto>>
    {
    }

    public class GetUsersForAutocompleteQueryHandler : IRequestHandler<GetUsersForAutocompleteQuery, IList<UserDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetUsersForAutocompleteQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<UserDto>> Handle(GetUsersForAutocompleteQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var usersCollection = database.GetCollection<ApplicationUser>(_mongoDbOptions.Collections.Users);

            var projection = new FindExpressionProjectionDefinition<ApplicationUser, UserDto>(u
                => new UserDto
                {
                    UserId = u.Id,
                    BriefName = $"{u.FirstName} {u.MiddleName.FirstOrDefault().ToString()}.{u.LastName.FirstOrDefault().ToString()}."
                });
            var result = await usersCollection.Find(f => true).Project(projection).ToListAsync(cancellationToken);

            return result;
        }
    }
}