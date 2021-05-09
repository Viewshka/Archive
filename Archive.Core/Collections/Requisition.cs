using System;
using System.Collections.Generic;
using Archive.Core.Collections.Common;

namespace Archive.Core.Collections
{
    /// <summary>
    /// Заявки на выдачу
    /// </summary>
    public class Requisition : Entity
    {
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
        public DateTime DateOfGiveOut { get; set; }

        /// <summary>
        /// Дата возврата
        /// </summary>
        public DateTime? DateOfReturn { get; set; }
    }
}