const documentTypes = [
    {
        id: 1,
        name: 'Чертеж',
        shortName: 'Чертеж'
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
        id:1,
        name: 'Копирование'
    },
    {
        id:2,
        name: 'Просмотр'
    },
    {
        id:3,
        name: 'Выписки'
    }
];

export default {
    documentTypes, documentUsageTypes
}