using System;
using Archive.Core.Enums;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Archive.Application.Extensions
{
    public static class DocumentTypeExtension
    {
        public static string GetString(this DocumentTypeEnum type)
        {
            return type switch
            {
                DocumentTypeEnum.СборочныйЧертеж => "Сборочный чертеж",
                DocumentTypeEnum.ЧертежДетали => "Чертеж детали",
                DocumentTypeEnum.Спецификация => "Спецификация",
                DocumentTypeEnum.КомплектКД => "Комплект конструкторской документации",
                DocumentTypeEnum.Заявка => "Заявка",
                _ => string.Empty
            };
        }
    }
}