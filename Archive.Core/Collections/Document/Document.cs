﻿using System;
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
    }
}