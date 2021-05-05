using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Archive.Application.Feature.Department.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQuery : IRequest<IList<DepartmentDto>>
    {
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, IList<DepartmentDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetAllDepartmentsQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<DepartmentDto>> Handle(GetAllDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var departmentsCollection = database.GetCollection<DepartmentDto>(_mongoDbOptions.Collections.Departments);

            var filter = new BsonDocument();

            return await departmentsCollection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}