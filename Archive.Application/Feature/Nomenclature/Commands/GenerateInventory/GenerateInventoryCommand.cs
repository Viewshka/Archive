using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Archive.Application.Common.Options.MongoDb;
using Archive.Application.Extensions;
using Archive.Application.Feature.Document.Queries.GetDocumentsByNomenclature;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using Archive.Core.Collections.Document;
using Archive.Core.Enums;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Archive.Application.Feature.Nomenclature.Commands.GenerateInventory
{
    public class GenerateInventoryCommand : IRequest
    {
        public string NomenclatureId { get; set; }
    }

    public static class Bookmark
    {
        public static string NomenclatureNumber => "NomenclatureNumber";
        public static string DocumentTable => "DocumentTable";
        public static string DocumentTotal => "DocumentTotal";
        public static string CountList => "CountList";
        public static string Creator => "Creator";
        public static string Date => "Date";
    }

    public class GenerateInventoryCommandHandler : IRequestHandler<GenerateInventoryCommand>
    {
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly MongoDbOptions _options;

        public GenerateInventoryCommandHandler(IHostingEnvironment environment,
            IOptions<MongoDbOptions> options,
            IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
            _options = options.Value;
        }

        public async Task<Unit> Handle(GenerateInventoryCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(_options.ConnectionString);
            var database = client.GetDatabase(_options.DatabaseName);
            var nomenclatureCollection = database
                .GetCollection<Core.Collections.Nomenclature>(_options.Collections.Nomenclatures);
            var templateCollection = database
                .GetCollection<Core.Collections.DocumentTemplate>(_options.Collections.DocumentTemplates);

            var documentCollection = database
                .GetCollection<Inventory>(_options.Collections.Documents);

            var templateFilter = Builders<Core.Collections.DocumentTemplate>.Filter
                .Eq("Type", DocumentTemplateEnum.ОписьДела);
            var template = await templateCollection.Find(templateFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var nomenclatureFilter = Builders<Core.Collections.Nomenclature>.Filter
                .Eq("_id", request.NomenclatureId);

            var nomenclature = await nomenclatureCollection.Find(nomenclatureFilter)
                .SingleOrDefaultAsync(cancellationToken);

            var documents = await _mediator
                .Send(new GetDocumentsByNomenclatureQuery
                {
                    NomenclatureId = request.NomenclatureId
                }, cancellationToken);

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

                InsertNomenclatureNumber(bookmarkMap, nomenclature);
                InsertDocumentTable(bookmarkMap, documents);
                InsertDocumentTotal(bookmarkMap, documents.Count);
                await InsertCreator(bookmarkMap);
                InsertDate(bookmarkMap);

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

            var random = new Random();
            var document = new Inventory
            {
                Name = $"Внутрення опись документов дела \"{nomenclature.Index} - {nomenclature.Name}\"",
                Path = $"files/{newFileGuid}.pdf",
                Type = DocumentTypeEnum.ОписьДела,
                DocumentDate = DateTime.Now,
                NomenclatureId = nomenclature.Id,
                Number = random.Next(1, 5000).ToString()
            };
            var documentBuilder = Builders<Inventory>.Filter;
            var documentFilter = documentBuilder.Eq("Type", DocumentTypeEnum.ОписьДела) &
                                 documentBuilder.Eq("NomenclatureId", nomenclature.Id);

            var oldInventory = await documentCollection.Find(documentFilter)
                .SingleOrDefaultAsync(cancellationToken);

            if (oldInventory != null)
            {
                if (System.IO.File.Exists(oldInventory.Path))
                    System.IO.File.Delete(oldInventory.Path);
            }

            await documentCollection.DeleteOneAsync(documentFilter, cancellationToken);
            await documentCollection.InsertOneAsync(document, cancellationToken: cancellationToken);

            return Unit.Value;
        }

        private static void InsertDate(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap)
        {
            var bookmark = bookmarkMap[Bookmark.Date];
            var parentBookmark = bookmark.Parent;
            var text = DateTime.Now.ToString("d");
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

        private async Task InsertCreator(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap)
        {
            var creator = await _mediator.Send(new GetCurrentUserQuery());
            
            var bookmark = bookmarkMap[Bookmark.Creator];
            var parentBookmark = bookmark.Parent;
            var text = creator.GetBriefNameWithJobTitle();
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

        private static void InsertDocumentTotal(Dictionary<string, BookmarkStart> bookmarkMap, int count)
        {
            var bookmark = bookmarkMap[Bookmark.DocumentTotal];
            var parentBookmark = bookmark.Parent;
            var text = $"Итого {count} документов";
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

        private static void InsertDocumentTable(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap,
            IList<DocumentsByNomenclatureDto> documents)
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
                    document.DocumentDate.ToString("d"),
                    document.Name,
                    document.Note
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
                "№", "Регистрационный номер документа", "Дата документа", "Заголовок документа", "Примечание"
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

        private static void InsertNomenclatureNumber(IReadOnlyDictionary<string, BookmarkStart> bookmarkMap,
            Core.Collections.Nomenclature nomenclature)
        {
            var bookmark = bookmarkMap[Bookmark.NomenclatureNumber];
            var parentBookmark = bookmark.Parent;

            var run = new Run(new Text(nomenclature.Index));
            var runProperties = new RunProperties
            {
                FontSize = new FontSize {Val = new StringValue("24")},
                RunFonts = new RunFonts {Ascii = "Arial"},
            };
            run.PrependChild(runProperties);

            var paragraphProperties = new ParagraphProperties
            {
                Justification = new Justification {Val = JustificationValues.Center},
                Indentation = new Indentation {FirstLine = "0"},
            };
            var paragraph = new Paragraph();
            paragraph.Append(paragraphProperties);
            paragraph.Append(run);
            parentBookmark.Append(paragraph);
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
}