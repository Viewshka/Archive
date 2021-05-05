using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Department.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<string>
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string ParentId { get; set; }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateDepartmentCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<string> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var departmentsCollection =
                database.GetCollection<Core.Collections.Department>(_mongoDbOptions.Collections.Departments);

            var entity = new Core.Collections.Department
            {
                ShortName = request.ShortName,
                FullName = request.FullName,
                ParentId = request.ParentId,
            };

            await departmentsCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity.Id;
        }
    }
}