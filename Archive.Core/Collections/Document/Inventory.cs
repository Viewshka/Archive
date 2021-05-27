using System;
using Archive.Core.Collections.Common;
using Archive.Core.Enums;

namespace Archive.Core.Collections.Document
{
    public class Inventory : Entity
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
    }
}