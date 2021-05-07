const documentTypes = Object.freeze({
    drawing: 1,
    specification: 2,
    kitCD : 3
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