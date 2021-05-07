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
