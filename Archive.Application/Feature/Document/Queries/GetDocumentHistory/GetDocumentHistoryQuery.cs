using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Core.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Document.Queries.GetDocumentHistory
{
    public class GetDocumentHistoryQuery : IRequest<IList<HistoryDto>>
    {
        public string DocumentId { get; set; }
    }

    public class GetDocumentHistoryQueryHandler : IRequestHandler<GetDocumentHistoryQuery, IList<HistoryDto>>
    {
        private readonly MongoDbOptions _mongoDbOptions;

        public GetDocumentHistoryQueryHandler(IOptions<MongoDbOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<IList<HistoryDto>> Handle(GetDocumentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var requisitionsCollection = database
                .GetCollection<Core.Collections.Requisition>(_mongoDbOptions.Collections.Requisitions);

            var result = await requisitionsCollection
                .Aggregate()
                .Lookup("users", "GiverId", "_id", "Giver")
                .Lookup("users", "RecipientId", "_id", "Recipient")
                .As<RequisitionInfoDto>()
                .ToListAsync(cancellationToken);


            return result.Where(dto => dto.Documents.Contains(request.DocumentId))
                .Select(s => new HistoryDto
                {
                    Giver = s.Giver.FirstOrDefault() is null ? "-" : s.Giver.FirstOrDefault().GetFullName(),
                    Recipient = s.Recipient.FirstOrDefault() is null ? "-" : s.Recipient.FirstOrDefault().GetFullName(),
                    UsageType = s.UsageType.GetString(),
                    DateOfReturn = s.DateOfReturn,
                    DateOfGiveOut = s.DateOfGiveOut,
                    Status = GetStatus(s).StatusToString()
                })
                .ToList();
        }

        private static RequisitionStatusEnum GetStatus(RequisitionInfoDto requisition)
        {
            return requisition.Canceled
                ? RequisitionStatusEnum.Отменено
                : requisition.IsDenied
                    ? RequisitionStatusEnum.Отказано
                    : requisition.DateOfReturn.HasValue
                        ? RequisitionStatusEnum.Возвращено
                        : !requisition.DateOfReturn.HasValue && !requisition.DateOfGiveOut.HasValue
                            ? RequisitionStatusEnum.Новая
                            : requisition.DateOfGiveOut.HasValue && !requisition.DateOfReturn.HasValue
                                ? RequisitionStatusEnum.Выдано
                                : RequisitionStatusEnum.ГотовоКВыдаче;
        }
    }
}