using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Identity;
using Archive.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MongoDbOptions _mongoDbOptions;

        public CreateRequisitionCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions,
            UserManager<ApplicationUser> userManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<Unit> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            var isUserArchivist = await _userManager.IsInRoleAsync(currentUser, Roles.Архивариус);

            var entity = new Core.Collections.Requisition
            {
                Documents = request.Documents,
                RecipientId = request.RecipientId,
                UsageType = request.UsageType,
                DateOfCreated = DateTime.Now,
                IsDenied = false,
                Canceled = false
            };

            if (isUserArchivist)
            {
                entity.GiverId = _currentUserService.UserId;
                entity.DateOfGiveOut = request.DateOfGiveOut ?? DateTime.Now;
            }

            await requisitionCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}