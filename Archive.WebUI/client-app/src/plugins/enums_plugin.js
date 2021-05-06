const documentTypes = Object.freeze({
    akt: 1,
    drawing: 2,
    specification: 3,
    inventory: 4,
    witnessSheet: 5,
    book: 6,
});

const roles = Object.freeze({
    archivist: '4ae43207-f1ed-499e-89b8-2111fc378786',
    employee: 'fc8630e3-e025-4b41-8751-1a92b93154da'
});

export default {
    install(Vue, options) {
        Vue.prototype.$enums = {
            documentTypes, roles
        };
    }
}