using System;
using Archive.Core.Collections.Common;
using Archive.Core.Enums;

namespace Archive.Core.Collections.Document
{
    public class Akt : Entity
    {
        /// <summary>
        /// Наименовнаие документа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата документа
        /// </summary>
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Тип документа
        /// </summary>
        public DocumentTypeEnum Type { get; set; }

        /// <summary>
        /// Путь к файлу документа
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Id номенклатурного дела
        /// </summary>
        public string NomenclatureId { get; set; }

        public Priority Priority { get; set; }

        /// <summary>
        /// Тип носителя
        /// </summary>
        public MediaType MediaType { get; set; }

        public string Number { get; set; }
    }
}