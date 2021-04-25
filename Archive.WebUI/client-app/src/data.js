const documents = [
    {
        id: 1,
        type: 6,
        path: '',
        nomenclatureId: 1,
        date: new Date(),
        number: 'АБВГД 834378.432.43',
        subject: 'Комплект конструкторской докуменации',
        note: 'примечание к документу',
        structure: [
            {
                id: 2,
                type: 2,
                path: '',
                nomenclatureId: 1,
                date: new Date(),
                number: 'АБВГД 834378.432.43',
                subject: 'Чертеж чертеж',
            },
            {
                id: 3,
                type: 3,
                path: '',
                nomenclatureId: 1,
                date: new Date(),
                number: 'АБВГД 834378.432.43',
                subject: 'Спецификация',
            }
        ]
    },

];

const types = [
    {
        id: 1,
        name: 'Акт'
    },
    {
        id: 2,
        name: 'Чертеж'
    }, 
    {
        id: 3,
        name: 'Спецификация'
    },
    {
        id: 4,
        name: 'Опись'
    },
    {
        id: 5,
        name: 'Лист-заверитель'
    },
    {
        id: 6,
        name: 'Конструкторская документация'
    },
    {
        id: 7,
        name: 'Книга учета и выбытия'
    }
]

export default {
    documents,types
}

