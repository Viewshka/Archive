const documentTypes = Object.freeze({
    akt: 1,
    construct: 2,
    order: 3,
    inventory: 4,
    witnessSheet: 5,
});

export default {
    install(Vue,options){
        Vue.prototype.$enums = {
            documentTypes
        };
    }
}