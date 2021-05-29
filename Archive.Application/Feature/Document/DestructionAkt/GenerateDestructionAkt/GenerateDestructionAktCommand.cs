using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Core.Collections.Document;
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

namespace Archive.Application.Feature.Document.DestructionAkt.GenerateDestructionAkt
{
    public class GenerateDestructionAktCommand : IRequest
    {
        public string Number { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string Reason { get; set; }
        public IList<string> DocumentIds { get; set; }
        public string NomenclatureId { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime ProtocolDate { get; set; }
        public int Weigh { get; set; }
        public DestructionTypeEnum DestructionType { get; set; }
    }

    public class GenerateDestructionAktCommandHandler : IRequestHandler<GenerateDestructionAktCommand>
    {
        private const string NomenclatureId = "b0eae1ad-e5b5-445c-8f46-dcbc7d1ae5f4";

        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MongoDbOptions _options;

        public GenerateDestructionAktCommandHandler(IOptions<MongoDbOptions> options,
            IHostingEnvironment environment, IMediator mediator,ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager)
        {
            _environment = environment;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _options = options.Value;
        }

        public async Task<Unit> Handle(GenerateDestructionAktCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var documentCollection = database
                .GetCollection<Core.Collections.Document.Document>(_options.Collections.Documents);
            var inventoryCollection = database.GetCollection<Inventory>(_options.Collections.Documents);
            var aktCollection = database.GetCollection<Akt>(_options.Collections.Documents);
            var nomenclatureCollection = database
                .GetCollection<Core.Collections.Nomenclature>(_options.Collections.Nomenclatures);
            var templateCollection = database
                .GetCollection<Core.Collections.DocumentTemplate>(_options.Collections.DocumentTemplates);
            var templateFilter = Builders<Core.Collections.DocumentTemplate>.Filter
                .Eq("Type", DocumentTemplateEnum.АктСписания);
            var template = await templateCollection.Find(templateFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var inventoryNumber = inventoryCollection.AsQueryable()
                .Where(document => document.Type == DocumentTypeEnum.ОписьДела
                                   &&
                                   document.NomenclatureId == request.NomenclatureId)
                .Select(document => document.Number)
                .SingleOrDefault();

            var nomenclatureFilter = Builders<Core.Collections.Nomenclature>.Filter
                .Eq("_id", request.NomenclatureId);
            var nomenclature = await nomenclatureCollection.Find(nomenclatureFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var documentFilter = Builders<Core.Collections.Document.Document>.Filter
                .In("_id", request.DocumentIds);
            var documents = await documentCollection.Find(documentFilter).ToListAsync(cancellationToken);

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

                InsertDocumentDateNumber(bookmarkMap, request.DocumentDate ?? DateTime.Now, request.Number);
                InsertReason(bookmarkMap, request.Reason, nomenclature.Index);
                InsertDocumentTable(bookmarkMap, documents, inventoryNumber);
                InsertInventoryApprove (bookmarkMap,documents);
                InsertArchivist (bookmarkMap,currentUser);
                InsertProtocol (bookmarkMap,request.ProtocolDate,request.ProtocolNumber);
                InsertDocumentCount(bookmarkMap, documents.Count, request.Weigh, request.DestructionType);
                InsertDocumentAndDatePassed(bookmarkMap,currentUser);
                InsertChangesAndDateApplied(bookmarkMap,currentUser);

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

            var document = new Akt
            {
                Name = $"Акт о выделении к уничтожению документов №{request.Number}",
                Path = outputPath,
                Type = DocumentTypeEnum.Акт,
                DocumentDate = request.DocumentDate ?? DateTime.Now,
                NomenclatureId = NomenclatureId,
                Number = request.Number,
                MediaType = MediaType.Бумажный,
                Priority = Priority.Секретно
            };
            
            await aktCollection.InsertOneAsync(document, cancellationToken: cancellationToken);

            return Unit.Value;
        }

        private static void InsertDocumentAndDatePassed(Dictionary<string, BookmarkStart> bookmarkMap, 
            ApplicationUser currentUser)
        {
            var bookmark = bookmarkMap[Bookmark.DocumentPassed];
            var parentBookmark = bookmark.Parent;
            
            var text = new Text(currentUser.GetBriefNameWithJobTitle());
            var run = new Run(text);
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);

            bookmark = bookmarkMap[Bookmark.DatePassed];
            parentBookmark = bookmark.Parent;

            text = new Text(DateTime.Now.ToString("d"));
            parentBookmark.GetFirstChild<Run>().Append(text);
        } 
        
        private static void InsertChangesAndDateApplied(Dictionary<string, BookmarkStart> bookmarkMap, 
            ApplicationUser currentUser)
        {
            var bookmark = bookmarkMap[Bookmark.ChangesApplied];
            var parentBookmark = bookmark.Parent;
            
            var text = new Text(currentUser.GetBriefNameWithJobTitle());
            var run = new Run(text);
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);

            bookmark = bookmarkMap[Bookmark.DateApplied];
            parentBookmark = bookmark.Parent;

            text = new Text(DateTime.Now.ToString("d"));
            parentBookmark.GetFirstChild<Run>().Append(text);
        }

        private static void InsertDocumentCount(Dictionary<string, BookmarkStart> bookmarkMap, 
            int documentsCount, int weigh, DestructionTypeEnum destructionType)
        {
            var bookmark = bookmarkMap[Bookmark.DocumentCount];
            var parentBookmark = bookmark.Parent;
            
            var text = new Text(documentsCount.ToString());
            parentBookmark.GetFirstChild<Run>().Append(text);
            
            
            bookmark = bookmarkMap[Bookmark.PaperWeigh];

            var bookmarkText = bookmark.NextSibling<Run>();
            bookmarkText.GetFirstChild<Text>().Text = $"{weigh.ToString()} кг";
            
            bookmark = bookmarkMap[Bookmark.ElectronicDestructionType];
            parentBookmark = bookmark.Parent;
            
            text = new Text(" -");
            parentBookmark.GetFirstChild<Run>().Append(text);
        }

        private static void InsertProtocol(Dictionary<string, BookmarkStart> bookmarkMap, 
            DateTime protocolDate, string protocolNumber)
        {
            var bookmark = bookmarkMap[Bookmark.Protocol];
            var parentBookmark = bookmark.Parent;
            
            var text = new Text($"Протокол ЭК от {protocolDate:d} № {protocolNumber}");
            parentBookmark.GetFirstChild<Run>().Append(text);
        }

        private static void InsertArchivist(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap, 
            ApplicationUser user)
        {
            var bookmark = bookmarkMap[Bookmark.Archivist];
            var parentBookmark = bookmark.Parent;
            
            var text = new Text(user.GetBriefNameWithJobTitle());
            var run = new Run(text);
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);
        }

        private static void InsertInventoryApprove(Dictionary<string,BookmarkStart> bookmarkMap, 
            IList<Core.Collections.Document.Document> documents)
        {
            var minDate = documents
                .OrderBy(d => d.DocumentDate.Year)
                .Select(d => d.DocumentDate.Year)
                .FirstOrDefault();

            var maxDate = documents
                .OrderByDescending(d => d.DocumentDate.Year)
                .Select(d => d.DocumentDate.Year)
                .FirstOrDefault();

            var years = minDate == maxDate ? $"{minDate}" : $"{minDate}-{maxDate}";
            var yearWord = minDate == maxDate ? "год." : "годы.";
            
            var bookmark = bookmarkMap[Bookmark.InventoryApprove];
            var parentBookmark = bookmark.Parent;

            var text = $"Описи дел постоянного хранения за {years} {yearWord} утверждены ЭПК.";

            var runText = new Text(text);
            parentBookmark.GetFirstChild<Run>().Append(runText);
        }

        private static void InsertDocumentTable(Dictionary<string, BookmarkStart> bookmarkMap,
            IList<Core.Collections.Document.Document> documents, string inventoryNumber)
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
                    document.Name,
                    document.DocumentDate.ToString("d"),
                    inventoryNumber,
                    document.StorageDate.HasValue ? document.StorageDate.Value.ToString("d") : "-",
                    document.Note
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

            var bookmark = bookmarkMap[Bookmark.DocumentTable];
            var bookmarkParent = bookmark.Parent;

            bookmarkParent.InsertBeforeSelf(table);

            var totalBookmark = bookmarkMap[Bookmark.DocumentTableTotal];
            var totalParentBookmark = totalBookmark.Parent;

            var minDate = documents
                .OrderBy(d => d.DocumentDate.Year)
                .Select(d => d.DocumentDate.Year)
                .FirstOrDefault();

            var maxDate = documents
                .OrderByDescending(d => d.DocumentDate.Year)
                .Select(d => d.DocumentDate.Year)
                .FirstOrDefault();

            var years = minDate == maxDate ? $"{minDate}" : $"{minDate}-{maxDate}";
            var documentCountToString = GetDocumentCountString(documents.Count);
            var yearWord = minDate == maxDate ? "год." : "годы.";
            var totalText = new Text($"Итого {documents.Count} {documentCountToString} за {years} {yearWord}");

            totalParentBookmark.GetFirstChild<Run>().Append(totalText);
        }

        private static string GetDocumentCountString(int count)
        {
            return count switch
            {
                1 => "документ",
                2 => "документа",
                3 => "документа",
                4 => "документа",
                _ => "документов",
            };
        }

        private static void InsertTableHeaderRow(Table table)
        {
            var tableRow = new TableRow();

            var headerRow = new[]
            {
                "№", "Наименование документа", "Дата документа", "Номер описи", "Срок хранения", "Примечание"
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

        private static void InsertReason(Dictionary<string, BookmarkStart> bookmarkMap, string reason,
            string nomenclature)
        {
            var bookmark = bookmarkMap[Bookmark.Reason];
            var parentBookmark = bookmark.Parent;

            var text = $"На основании {reason} отобраны к уничтожению утратившие " +
                       $"практическое значение докуметы дела {nomenclature}";

            var run = new Run(new Text(text));
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);
            var paragraph = new Paragraph();
            paragraph.Append(run);
            parentBookmark.Append(paragraph);
        }

        private static void InsertDocumentDateNumber(Dictionary<string, BookmarkStart> bookmarkMap,
            DateTime documentDate, string number)
        {
            var bookmark = bookmarkMap[Bookmark.DocumentDateNumber];
            var parentBookmark = bookmark.Parent;

            var text = new Text($"{documentDate:d} № {number}");
            parentBookmark.GetFirstChild<Run>().Append(text);
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
        public static string DocumentDateNumber => "DocumentDateNumber";
        public static string Reason => "Reason";
        public static string DocumentTable => "DocumentTable";
        public static string DocumentTableTotal => "DocumentTableTotal";
        public static string InventoryApprove => "InventoryApprove";
        public static string Archivist => "Archivist";
        public static string Protocol => "Protocol";
        public static string DocumentCount => "DocumentCount";
        public static string PaperWeigh => "PaperWeigh";
        public static string ElectronicDestructionType => "ElectronicDestructionType";
        public static string DocumentPassed => "DocumentPassed";
        public static string DatePassed => "DatePassed";
        public static string ChangesApplied => "ChangesApplied";
        public static string DateApplied => "DateApplied";
    }
}