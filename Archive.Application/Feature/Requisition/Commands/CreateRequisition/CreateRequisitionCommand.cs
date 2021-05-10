using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Requisition.Commands.CreateRequisition
{
    public class CreateRequisitionCommand : IRequest
    {
        public string RecipientId { get; set; }
        public IList<string> Documents { get; set; }
        public DateTime? DateOfGiveOut { get; set; }
        public DocumentUsageType UsageType { get; set; }
    }

    public class CreateRequisitionCommandHandler : IRequestHandler<CreateRequisitionCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateRequisitionCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions)
        {
            _currentUserService = currentUserService;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var entity = new Core.Collections.Requisition
            {
                Documents = request.Documents,
                GiverId = _currentUserService.UserId,
                RecipientId = request.RecipientId,
                DateOfCreated = DateTime.Now,
                DateOfGiveOut = request.DateOfGiveOut ?? DateTime.Now,
                UsageType = request.UsageType
            };
            
            await requisitionCollection.InsertOneAsync(entity,cancellationToken:cancellationToken);
            
            return Unit.Value;
        }
    }
}