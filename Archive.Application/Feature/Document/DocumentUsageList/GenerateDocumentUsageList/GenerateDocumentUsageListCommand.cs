using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Application.Feature.Document.Queries.GetDocumentHistory;
using Archive.Core.Enums;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace Archive.Application.Feature.Document.DocumentUsageList.GenerateDocumentUsageList
{
    public class GenerateDocumentUsageListCommand : IRequest<Stream>
    {
        public string DocumentId { get; set; }
    }

    public class GenerateDocumentUsageListCommandHandler : IRequestHandler<GenerateDocumentUsageListCommand, Stream>
    {
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly MongoDbOptions _options;

        public GenerateDocumentUsageListCommandHandler(IOptions<MongoDbOptions> options,
            IHostingEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
            _options = options.Value;
        }

        public async Task<Stream> Handle(GenerateDocumentUsageListCommand request,
            CancellationToken cancellationToken)
        {
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);

            var documentTemplateCollection = database
                .GetCollection<Core.Collections.DocumentTemplate>(_options.Collections.DocumentTemplates);

            var documentCollection = database
                .GetCollection<Core.Collections.Document.Document>(_options.Collections.Documents);

            var templateFilter = Builders<Core.Collections.DocumentTemplate>.Filter
                .Eq("Type", DocumentTemplateEnum.ЛистИспользованияДокумента);
            var template = await documentTemplateCollection.Find(templateFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var requisitions = await _mediator
                .Send(new GetDocumentHistoryQuery {DocumentId = request.DocumentId}, cancellationToken);

            var documentFilter = Builders<Core.Collections.Document.Document>.Filter.Eq("_id", request.DocumentId);
            var document = await documentCollection.Find(documentFilter).SingleOrDefaultAsync(cancellationToken);

            var inputPathDocx = Path.Combine(_environment.WebRootPath, template.Path);
            var outputPathDocx = Path.Combine(_environment.WebRootPath, "temp/test.docx");
            var outputPathPdf = Path.Combine(_environment.WebRootPath, "temp/test.pdf");

            var tempFolder = Path.Combine(_environment.WebRootPath, "temp");
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
                Directory.CreateDirectory(tempFolder);
            }
            else
            {
                Directory.CreateDirectory(tempFolder);
            }

            var templateFs = new FileStream(inputPathDocx, FileMode.Open);
            var templateStream = new MemoryStream();
            await templateFs.CopyToAsync(templateStream, cancellationToken);
            templateFs.Close();

            using (var server = WordprocessingDocument.Open(templateStream, true))
            {
                var table = new Table();

                var bookmarkMap = new Dictionary<string, BookmarkStart>();

                foreach (var bookmarkStart in server.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                }

                var tableProperties = GetTableProperties();
                table.AppendChild(tableProperties);

                var bookmark = bookmarkMap[Bookmark.DocumentName];
                var bookmarkParent = bookmark.Parent;

                var type = document.Type.GetString().ToLower();
                var text = $"\"{type} - {document.Name}\"";

                var run = new Run(new Text(text));
                var paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification {Val = JustificationValues.Center}
                    }
                };
                paragraph.Append(run);
                bookmarkParent.Append(paragraph);

                InsertTableHeaderRow(table);
                InsertTableBody(table, requisitions);

                bookmark = bookmarkMap[Bookmark.UsageTable];
                bookmarkParent = bookmark.Parent;

                bookmarkParent.InsertAfterSelf(table);

                if (System.IO.File.Exists(outputPathDocx))
                    System.IO.File.Delete(outputPathDocx);
                var savedDoc = server.SaveAs(outputPathDocx);
                savedDoc.Close();
                server.Close();
            }

            var docxFileStream = new FileStream(outputPathDocx, FileMode.Open);
            var docxMemoryStream = new MemoryStream();
            await docxFileStream.CopyToAsync(docxMemoryStream, cancellationToken);
            docxFileStream.Close();
            docxMemoryStream.Position = 0;

            var pdfStream = await ConvertToPdf(cancellationToken, docxMemoryStream, template);

            return pdfStream;
        }

        private static void InsertTableBody(Table table, IList<HistoryDto> histories)
        {
            foreach (var history in histories)
            {
                var rowData = new[]
                {
                    history.Giver,
                    history.DateOfGiveOut.HasValue ? history.DateOfGiveOut.Value.ToString("d") : "-",
                    history.Recipient,
                    history.DateOfReturn.HasValue ? history.DateOfReturn.Value.ToString("d") : "-",
                    history.UsageType,
                    history.Status
                };

                var row = new TableRow();

                for (var i = 0; i < 6; i++)
                {
                    var tableCell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellVerticalAlignment = new TableCellVerticalAlignment
                            {
                                Val = TableVerticalAlignmentValues.Center
                            }
                        }
                    };
                    var tableCellWidth = new TableCellWidth
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = "2400",
                    };
                    var tableCellProperties = new TableCellProperties(tableCellWidth);
                    tableCell.Append(tableCellProperties);

                    var text = new Text(rowData[i]);
                    var run = new Run(text);
                    var runProperties = new RunProperties
                    {
                        FontSize = new FontSize {Val = new StringValue("20")},
                        RunFonts = new RunFonts {Ascii = "Arial"},
                    };
                    run.PrependChild(runProperties);

                    var paragraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification {Val = JustificationValues.Center},
                        Indentation = new Indentation {FirstLine = "0"},
                        SpacingBetweenLines = new SpacingBetweenLines
                            {After = "0", Line = "0", LineRule = LineSpacingRuleValues.AtLeast},
                    };

                    var paragraph = new Paragraph();
                    paragraph.Append(paragraphProperties);
                    paragraph.Append(run);

                    tableCell.Append(paragraph);
                    row.Append(tableCell);
                }

                table.Append(row);
            }
        }

        private static void InsertTableHeaderRow(Table table)
        {
            var tableRow = new TableRow();

            var headerRow = new[]
            {
                "Кем выдано", "Дата выдачи", "Кому выдано", "Дата возврата", "Характер использования", "Статус"
            };
            for (var i = 0; i < 6; i++)
            {
                var tableCell = new TableCell
                {
                    TableCellProperties = new TableCellProperties
                    {
                        TableCellVerticalAlignment = new TableCellVerticalAlignment
                        {
                            Val = TableVerticalAlignmentValues.Center
                        }
                    }
                };
                var tableCellWidth = new TableCellWidth
                {
                    Type = TableWidthUnitValues.Dxa,
                    Width = "2400",
                };
                var tableCellProperties = new TableCellProperties(tableCellWidth);
                tableCell.Append(tableCellProperties);

                var text = new Text(headerRow[i]);
                var run = new Run(text);
                var runProperties = new RunProperties
                {
                    FontSize = new FontSize {Val = new StringValue("20")},
                    RunFonts = new RunFonts {Ascii = "Arial"},
                };
                run.PrependChild(runProperties);

                var paragraphProperties = new ParagraphProperties
                {
                    Justification = new Justification {Val = JustificationValues.Center},
                    Indentation = new Indentation {FirstLine = "0"},
                    SpacingBetweenLines = new SpacingBetweenLines
                        {After = "0", Line = "0", LineRule = LineSpacingRuleValues.AtLeast},
                };

                var paragraph = new Paragraph();
                paragraph.Append(paragraphProperties);
                paragraph.Append(run);

                tableCell.Append(paragraph);
                tableRow.Append(tableCell);
            }

            table.Append(tableRow);
        }

        private static TableProperties GetTableProperties()
        {
            return new TableProperties(
                new TableBorders(
                    new TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                    },
                    new BottomBorder
                        {Val = new EnumValue<BorderValues>(BorderValues.Single)},
                    new LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                    },
                    new RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                    },
                    new InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                    },
                    new InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                    }
                )
            );
        }

        private static void SetParagraphSettings()
        {
        }

        private static async Task<Stream> ConvertToPdf(CancellationToken cancellationToken, MemoryStream docxStream,
            Core.Collections.DocumentTemplate template)
        {
            const string url = @"http://localhost:5000/api/converter";
            var httpClient = new HttpClient();

            var byteArrayContent = new ByteArrayContent(docxStream.ToArray());
            var multiContent = new MultipartFormDataContent();
            multiContent.Add(byteArrayContent, "file", template.FileName);

            var response = await httpClient.PostAsync(url, multiContent, cancellationToken);

            return await response.Content.ReadAsStreamAsync(cancellationToken);
            ;
        }


        private static class Bookmark
        {
            public static string DocumentName => "НаименованиеДокумента";
            public static string UsageTable => "ТаблицаИспользования";
        }
    }
}