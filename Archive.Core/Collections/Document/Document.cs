using System;
using System.Collections.Generic;
using Archive.Core.Collections.Common;
using Archive.Core.Enums;

namespace Archive.Core.Collections.Document
{
    public class Document : Entity
    {
        /// <summary>
        /// Наименовнаие документа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Обозначение
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }

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
        /// Дата поступления в архив
        /// </summary>
        public DateTime IncomingDate { get; set; }

        /// <summary>
        /// Дата выбытия из архива
        /// </summary>
        public DateTime? OutgoingDate { get; set; }

        /// <summary>
        /// Id номенклатурного дела
        /// </summary>
        public string NomenclatureId { get; set; }

        /// <summary>
        /// Принадлежность к другому документу
        /// </summary>
        public string ParentId { get; set; }

        public DateTime? StorageDate { get; set; }
    }
}