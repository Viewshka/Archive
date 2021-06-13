using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Archive.Core.Collections.Common;
using Archive.Core.Enums;

namespace Archive.Core.Collections
{
    /// <summary>
    /// Заявки на выдачу
    /// </summary>
    public class Requisition : Entity
    {
        /// <summary>
        /// Получатель (он же создатель заявки)
        /// </summary>
        public string RecipientId { get; set; }

        /// <summary>
        /// Выдал документ
        /// </summary>
        public string GiverId { get; set; }

        /// <summary>
        /// Id документа
        /// </summary>
        public IList<string> Documents { get; set; }

        /// <summary>
        /// Дата создания заявки
        /// </summary>
        public DateTime DateOfCreated { get; set; }

        /// <summary>
        /// Дата выдачи документа
        /// </summary>
        public DateTime? DateOfGiveOut { get; set; }

        /// <summary>
        /// Дата возврата
        /// </summary>
        public DateTime? DateOfReturn { get; set; }

        /// <summary>
        /// Характер использования документа
        /// </summary>
        public DocumentUsageType UsageType { get; set; }

        /// <summary>
        /// Отказано
        /// </summary>
        public bool IsDenied { get; set; }

        /// <summary>
        /// Отмена
        /// </summary>
        public bool Canceled { get; set; }

        public string Path { get; set; }
        public string FileName { get; set; }
        public int Number { get; set; }
    }
}