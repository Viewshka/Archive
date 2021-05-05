const documentTypes = Object.freeze({
    akt: 1,
    drawing: 2,
    specification: 3,
    inventory: 4,
    witnessSheet: 5,
    book: 6,
});

export default {
    install(Vue,options){
        Vue.prototype.$enums = {
            documentTypes
        };
    }
}