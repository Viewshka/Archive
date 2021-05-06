using System;

namespace Archive.Core.Collections.Common
{
    public class History : Entity
    {
        /// <summary>
        /// Получатель документа
        /// </summary>
        public string RecipientId { get; set; }

        /// <summary>
        /// Дата выдачи
        /// </summary>
        public DateTime GiveOutDate { get; set; }

        /// <summary>
        /// Дата возрата
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Выдающий (дать) документ
        /// </summary>
        public string GiverId { get; set; }
    }
}