const documentTypes = [
    {
        id: 1,
        name: 'Чертеж детали',
        shortName: 'Чертеж детали'
    },
    {
        id: 8,
        name: 'Сборочный чертеж',
        shortName: 'Сборочный чертеж'
    },
    {
        id: 2,
        name: 'Спецификация',
        shortName: 'Спецификация'
    },
    {
        id: 3,
        name: 'Комплект конструкторской документации',
        shortName: 'Комплект КД'
    }
];

const documentUsageTypes = [
    {
        id: 1,
        name: 'Копирование'
    },
    {
        id: 2,
        name: 'Просмотр'
    }
];

const requisitionStatus = [
    {
        id: 1,
        name: 'Новая'
    },
    {
        id: 2,
        name: 'Готово к выдаче'
    },
    {
        id: 3,
        name: 'Выдано'
    },
    {
        id: 4,
        name: 'Отказано'
    },
    {
        id: 5,
        name: 'Возвращено'
    },
    {
        id: 6,
        name: 'Отменено'
    },
];

const priority = [
    {
        id: 1,
        name: 'Секретно'
    },
    {
        id: 2,
        name: 'Конфиденциально'
    },
    {
        id: 3,
        name: 'Коммерческая'
    },
    {
        id: 3,
        name: 'Служебная'
    },
    {
        id: 4,
        name: 'Общедоступная'
    },
];

const mediaType = [
    {
        id: 1,
        name: 'Бумажный'
    },
    {
        id: 2,
        name: 'Электронный'
    },
];

export default {
    documentTypes, documentUsageTypes, requisitionStatus, priority, mediaType
}