using Archive.Core.Enums;

namespace Archive.Application.Extensions
{
    public static class DestructionType
    {
        public static string GetString(this DestructionTypeEnum type)
        {
            return type switch
            {
                DestructionTypeEnum.УничтожениеНосителя => "Уничтожение носителя",
                DestructionTypeEnum.ОчисткаНосителя => "Очистка носителя",
                _ => string.Empty
            };
        }
    }
}