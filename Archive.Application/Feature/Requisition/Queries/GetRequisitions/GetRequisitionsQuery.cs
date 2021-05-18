using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using Archive.Core.Collections.Identity;
using Archive.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Archive.Application.Feature.Requisition.Queries.GetRequisitions
{
    public class GetRequisitionsQuery : IRequest<IList<RequisitionDto>>
    {
    }

    public class GetRequisitionsQueryHandler : IRequestHandler<GetRequisitionsQuery, IList<RequisitionDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetRequisitionsQueryHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> mongoDbOptions,
            UserManager<ApplicationUser> userManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<RequisitionDto>> Handle(GetRequisitionsQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionsCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            var isUserArchivist = await _userManager.IsInRoleAsync(currentUser, Roles.Архивариус);

            var query = requisitionsCollection.AsQueryable();

            if (!isUserArchivist)
                query = query.Where(requisition => requisition.RecipientId == _currentUserService.UserId);

            return query
                .Select(requisition => new RequisitionDto
                {
                    Id = requisition.Id,
                    Documents = requisition.Documents,
                    GiverId = requisition.GiverId,
                    RecipientId = requisition.RecipientId,
                    UsageType = requisition.UsageType,
                    DateOfCreated = requisition.DateOfCreated,
                    DateOfGiveOut = requisition.DateOfGiveOut,
                    DateOfReturn = requisition.DateOfReturn,
                    Status = requisition.Canceled
                        ? RequisitionStatusEnum.Отменено
                        : requisition.IsDenied
                            ? RequisitionStatusEnum.Отказано
                            : requisition.DateOfReturn.HasValue
                                ? RequisitionStatusEnum.Возвращено
                                : !requisition.DateOfReturn.HasValue && !requisition.DateOfGiveOut.HasValue
                                    ? RequisitionStatusEnum.Новая
                                    : requisition.DateOfGiveOut.HasValue && !requisition.DateOfReturn.HasValue
                                        ? RequisitionStatusEnum.Выдано
                                        : RequisitionStatusEnum.ГотовоКВыдаче
                })
                .ToList();
        }
    }
}