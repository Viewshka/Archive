using System;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Department.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest
    {
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string ParentId { get; set; }
    }

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public UpdateDepartmentCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var departmentsCollection =
                database.GetCollection<Core.Collections.Department>(_mongoDbOptions.Collections.Departments);

            var filter = Builders<Core.Collections.Department>.Filter
                .Eq("_id", request.Id);

            var update = Builders<Core.Collections.Department>.Update
                .Set("ShortName", request.ShortName)
                .Set("FullName", request.FullName)
                .Set("ParentId", request.ParentId);

            var result = await departmentsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged)
                throw new Exception("Во время изменения произошла ошибка");

            return Unit.Value;
        }
    }
}