using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Document;
using Archive.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Archive.Application.Feature.Document.Queries.GetInventoryByNomenclature
{
    public class GetInventoryByNomenclatureCommand : IRequest<MemoryStream>
    {
        public string NomenclatureId { get; set; }
    }

    public class
        GetInventoryByNomenclatureCommandHandler : IRequestHandler<GetInventoryByNomenclatureCommand, MemoryStream>
    {
        private readonly IMediator _mediator;
        private readonly IHostingEnvironment _environment;
        private readonly MongoDbOptions _mongoDbOptions;

        public GetInventoryByNomenclatureCommandHandler(IOptions<MongoDbOptions> mongoDbOptions,
            IMediator mediator, IHostingEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public async Task<MemoryStream> Handle(GetInventoryByNomenclatureCommand request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(_mongoDbOptions.DatabaseName);
            var documentCollection = database
                .GetCollection<Inventory>(_mongoDbOptions.Collections.Documents);

            var builder = Builders<Inventory>.Filter;

            var filter = builder.Eq("NomenclatureId", request.NomenclatureId) &
                         builder.Eq("Type", DocumentTypeEnum.ОписьДела);

            var inventory = await documentCollection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);

            if (inventory is null)
            {
                var response = GenerateNullResponse();
                return response;
            }

            var resultStream = new MemoryStream();
            var path = Path.Combine(_environment.WebRootPath, inventory.Path);
            var fileStream = new FileStream(path, FileMode.Open);
            await fileStream.CopyToAsync(resultStream, cancellationToken);
            resultStream.Position = 0;
            fileStream.Close();

            return resultStream;
        }

        private static MemoryStream GenerateNullResponse()
        {
            var document = new PdfDocument();
            var page = document.AddPage();

            var graphics = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 20);
            graphics.DrawString("Файл еще не создан", font, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height), XStringFormats.TopCenter);

            var resultStream = new MemoryStream();
            document.Save(resultStream);
            resultStream.Position = 0;
            return resultStream;
        }
    }
}