using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Department.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public DeleteDepartmentCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var departmentsCollection =
                database.GetCollection<Core.Collections.Department>(_mongoDbOptions.Collections.Departments);

            var filter = Builders<Core.Collections.Department>.Filter.Eq("_id", request.Id);
            var result = await departmentsCollection.DeleteOneAsync(filter, cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время удаления произошла ошибка");

            return Unit.Value;
        }
    }
}