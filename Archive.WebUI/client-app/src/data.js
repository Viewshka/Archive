const documents = Object.freeze([
    {
        id: 1,
        type: 6,
        path: '',
        nomenclatureId: "b9f65372-d41f-4ab6-9363-102c3e592c5b",
        date: new Date(),
        number: 'А.53.750.001.01',
        subject: 'Комплект конструкторской докуменации',
        note: 'sss',
        parentId: 0,
    },
    {
        id: 2,
        type: 2,
        path: '',
        nomenclatureId: "b9f65372-d41f-4ab6-9363-102c3e592c5b",
        date: new Date(),
        number: 'А.53.750.001.01',
        subject: 'Крышка',
        parentId: 1
    },
    {
        id: 3,
        type: 3,
        path: '',
        nomenclatureId: "b9f65372-d41f-4ab6-9363-102c3e592c5b",
        date: new Date(),
        number: 'А.53.750.001.01',
        subject: 'Крышка',
        parentId: 1
    },
    {
        id: 4,
        type: 2,
        path: '',
        date: new Date(),
        number: '058.003.030.001',
        subject: 'Вал',
        note: '',
        parentId: 0
    }

]);

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

const nomenclatures = [
    {
        id: 1,
        departmentId: 2,
        fundId: 1,
        index: "03-01",
        name: "Распорядительная документация",
        year: 2021,
        fullName: "03-01 Распорядительная документация"
    },
    {
        id: 2,
        departmentId: 2,
        fundId: 1,
        index: "03-09",
        name: "Чертежи",
        year: 2021,
        fullName: "03-09 Чертежи"
    }
];

const departments = [
    {
        id: 1,
        fullName: "Предприятие полное",
        shortName: "Предприятия кр.",
        type: 1
    },
    {
        id: 2,
        fullName: "Отдел главного конструктора",
        shortName: "ОГК",
        type: 2,
        parentId: 1
    },
    {
        id: 3,
        fullName: "Отдел главного технолога",
        shortName: "ОГТ",
        type: 2,
        parentId: 1
    },
    {
        id: 4,
        fullName: "Подразделениие архив",
        shortName: "Архив",
        type: 3,
        parentId: 1
    }
];

const departmentTypes = [
    {
        id: 1,
        name: 'Предприятие'
    },
    {
        id: 2,
        name: 'Отдел'
    },
    {
        id: 3,
        name: 'Обособленное подразделение'
    }
]

export default {
    documents, types, departments, nomenclatures, departmentTypes
}

