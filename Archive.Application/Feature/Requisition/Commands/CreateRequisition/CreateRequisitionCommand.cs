using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Core.Collections.Identity;
using Archive.Core.Enums;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
        private readonly IMediator _mediator;
        private readonly IHostingEnvironment _environment;
        private readonly MongoDbOptions _options;

        public CreateRequisitionCommandHandler(ICurrentUserService currentUserService,
            IOptions<MongoDbOptions> options,
            UserManager<ApplicationUser> userManager,
            IMediator mediator,
            IHostingEnvironment environment)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mediator = mediator;
            _environment = environment;
            _options = options.Value;
        }

        public async Task<Unit> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var requisitionCollection = database
                .GetCollection<Core.Collections.Requisition>(_options.Collections.Requisitions);
            var templateCollection = database
                .GetCollection<Core.Collections.DocumentTemplate>(_options.Collections.DocumentTemplates);
            var templateFilter = Builders<Core.Collections.DocumentTemplate>.Filter
                .Eq("Type", DocumentTemplateEnum.Заявка);
            var template = await templateCollection.Find(templateFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var maxRequisitionNumber = requisitionCollection.AsQueryable()
                .OrderByDescending(requisition => requisition.Number)
                .Select(requisition => requisition.Number)
                .FirstOrDefault();

            var documentCollection = database
                .GetCollection<Core.Collections.Document.Document>(_options.Collections.Documents);
            var documentFilter = Builders<Core.Collections.Document.Document>.Filter
                .In("_id", request.Documents);
            var documents = await documentCollection.Find(documentFilter).ToListAsync(cancellationToken);

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            var recipient = await _userManager.FindByIdAsync(request.RecipientId);
            var isUserArchivist = await _userManager.IsInRoleAsync(currentUser, Roles.Архивариус);

            var newFileGuid = Guid.NewGuid().ToString();
            var inputPath = Path.Combine(_environment.WebRootPath, template.Path);
            var outputPath = Path.Combine(_environment.WebRootPath, $"files/{newFileGuid}.pdf");
            var tempPath = Path.Combine(_environment.WebRootPath, "temp/temp_file.docx");
            var tempFolder = Path.Combine(_environment.WebRootPath, "temp");

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            ClearTempDirectory(tempFolder);

            var templateFs = new FileStream(inputPath, FileMode.Open);
            var templateStream = new MemoryStream();
            await templateFs.CopyToAsync(templateStream, cancellationToken);
            templateFs.Close();

            using (var server = WordprocessingDocument.Open(templateStream, true))
            {
                var bookmarkMap = new Dictionary<string, BookmarkStart>();

                foreach (var bookmarkStart in server.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                }

                InsertDocumentNumber(bookmarkMap, maxRequisitionNumber);
                InsertDocumentTable(bookmarkMap, documents, request.UsageType);
                InsertRequester(bookmarkMap, recipient);

                if (isUserArchivist) InsertGiver(bookmarkMap, currentUser);

                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);

                server.SaveAs(tempPath).Close();
            }

            var docxFileStream = new FileStream(tempPath, FileMode.Open);
            var docxMemoryStream = new MemoryStream();
            await docxFileStream.CopyToAsync(docxMemoryStream, cancellationToken);
            docxFileStream.Close();
            docxMemoryStream.Position = 0;

            var pdfStream = await ConvertToPdf(cancellationToken, docxMemoryStream, template);
            var fs = new FileStream(outputPath, FileMode.Create);
            await pdfStream.CopyToAsync(fs, cancellationToken);
            fs.Close();

            var entity = new Core.Collections.Requisition
            {
                Documents = request.Documents,
                RecipientId = request.RecipientId,
                UsageType = request.UsageType,
                DateOfCreated = DateTime.Now,
                IsDenied = false,
                Canceled = false,
                Path = $"files/{newFileGuid}.pdf",
                FileName = $"{newFileGuid}.pdf",
                Number = maxRequisitionNumber + 1
            };

            if (isUserArchivist)
            {
                entity.GiverId = _currentUserService.UserId;
                entity.DateOfGiveOut = request.DateOfGiveOut ?? DateTime.Now;
            }

            await requisitionCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return Unit.Value;
        }

        private static void InsertGiver(Dictionary<string, BookmarkStart> bookmarkMap, ApplicationUser giver)
        {
            var bookmark = bookmarkMap[Bookmark.Giver];
            var parentBookmark = bookmark.Parent;

            var text = Regex.Replace(giver.GetBriefNameWithJobTitle(), "(^.)", m => m.Groups[1].Value.ToLower());
            var bookmarkText = new Text(text);
            var run = new Run(bookmarkText);
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);

            bookmark = bookmarkMap[Bookmark.GiverDate];
            parentBookmark = bookmark.Parent;

            bookmarkText = new Text(DateTime.Now.ToString("d"));
            run = new Run(bookmarkText);
            runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);
        }

        private static void InsertRequester(Dictionary<string, BookmarkStart> bookmarkMap, ApplicationUser recipient)
        {
            var bookmark = bookmarkMap[Bookmark.Requester];
            var parentBookmark = bookmark.Parent;
            var text = Regex.Replace(recipient.GetBriefNameWithJobTitle(), "(^.)", m => m.Groups[1].Value.ToLower());
            var bookmarkText = new Text(text);
            var run = new Run(bookmarkText);
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);

            bookmark = bookmarkMap[Bookmark.RequesterDate];
            parentBookmark = bookmark.Parent;

            bookmarkText = new Text(DateTime.Now.ToString("d"));
            run = new Run(bookmarkText);
            runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);
        }

        private static void InsertDocumentNumber(Dictionary<string, BookmarkStart> bookmarkMap,
            int maxRequisitionNumber)
        {
            var bookmark = bookmarkMap[Bookmark.Number];
            var parentBookmark = bookmark.Parent;
            var text = maxRequisitionNumber + 1;
            var bookmarkText = new Text(text.ToString());
            parentBookmark.GetFirstChild<Run>().Append(bookmarkText);
        }

        private static void InsertDocumentTable(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap,
            IList<Core.Collections.Document.Document> documents, DocumentUsageType usageType)
        {
            var table = new Table();

            var tableProperties = GetTableProperties();
            table.AppendChild(tableProperties);

            InsertTableHeaderRow(table);
            var index = 1;
            foreach (var document in documents)
            {
                var rowData = new[]
                {
                    index++.ToString(),
                    document.Designation,
                    document.Name,
                    document.Type.GetString(),
                    usageType.GetString()
                };

                var row = new TableRow();
                for (var i = 0; i < 5; i++)
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

            var bookmark = bookmarkMap[Bookmark.DocumentTable];
            var bookmarkParent = bookmark.Parent;

            bookmarkParent.InsertBeforeSelf(table);
        }

        private static void InsertTableHeaderRow(Table table)
        {
            var tableRow = new TableRow();

            var headerRow = new[]
            {
                "№", "Обозначение", "Наименование", "Тип документа", "Характер использования"
            };
            for (var i = 0; i < 5; i++)
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
        }

        private static void ClearTempDirectory(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            foreach (var file in dirInfo.GetFiles())
                file.Delete();
        }
    }

    public static class Bookmark
    {
        public static string DocumentTable => "DocumentTable";
        public static string Number => "Number";
        public static string Requester => "Requester";
        public static string RequesterDate => "RequesterDate";
        public static string Giver => "Giver";
        public static string GiverDate => "GiverDate";
    }
}