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
                text: "Акты",
                path: "/akt",
            },
            {
                text: "Описи дел",
                path: "/inventories",
            },
        ]
    },
    {

        text: "Заявки",
        path: "/history",
        icon: "alignleft",
    },
    {
        text: "Справочники",
        icon: "menu",
        items: [
            {
                text: "Подразделения",
                path: "/departments",
            },
            {
                text: "Пользователи",
                path: "/users",
            },
        ]
    }
];
