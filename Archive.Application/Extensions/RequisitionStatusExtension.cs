using Archive.Core.Enums;

namespace Archive.Application.Extensions
{
    public static class RequisitionStatusExtension
    {
        public static string StatusToString(this RequisitionStatusEnum status)
        {
            return status switch
            {
                RequisitionStatusEnum.Новая => "Новая",
                RequisitionStatusEnum.ГотовоКВыдаче => "Готов к выдаче",
                RequisitionStatusEnum.Выдано => "Выдано",
                RequisitionStatusEnum.Отказано => "Отказано",
                RequisitionStatusEnum.Возвращено => "Возвращено",
                RequisitionStatusEnum.Отменено => "Отменено",
                _ => string.Empty
            };
        }
    }
}