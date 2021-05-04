import Vue from 'vue'
import Vuex from 'vuex'
import axios from "axios";
import notify from "devextreme/ui/notify";

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        currentUser: {}       
    },
    mutations: {
        SET_CURRENT_USER: function (state, user) {
            state.currentUser = user;
        },
    },
    actions: {
        INIT_CURRENT_USER(context) {
            axios.get(`api/user/current-user`)
                .then((response) => {
                    context.commit('SET_CURRENT_USER', response.data);
                })
                .catch((error) => {
                    notify(error.response.data, 'error', 3000);
                })
        },
        CLEAR_CURRENT_USER(context){
            context.commit('SET_CURRENT_USER', {});
        }
    },
    modules: {}
})
