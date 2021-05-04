export default [
    {
        text: "Главная",
        path: "/home",
        icon: "home"
    },
    {
        text: "Номенклатура",
        path: "/nomenclatures",
        icon: "folder"
    },
    {
        text: "Документы",
        path: "/documents",
        icon: "file",
        items: [
            {
                text: "Все документы",
                path: "/all-documents",
            },
            {
                text: "Конструкторская документация",
                path: "",
                items: [
                    {
                        text: "Чертежи",
                        path: "/draws",
                    },
                    {
                        text: "Спецификации",
                        path: "/specifications",
                    },
                ]
            },
            {
                text: "Акты",
                path: "/akts",
            },
            {
                text: "Описи",
                path: "/inventory",
            },
        ]
    },
    {
        text: "Справочники",
        icon: "menu",
        items: [
            {
                text: "Подразделения",
                path: "/departments",
            },
        ]
    }
];
