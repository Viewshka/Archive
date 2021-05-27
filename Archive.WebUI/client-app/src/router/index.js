import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from '../views/Home.vue';
import defaultLayout from '../layouts/SideNavOuterToolbar';
import NomenclatureGrid from "../views/NomenclatureGrid";
import DocumentGrid from "../views/DocumentGrid";
import Requisition from "../views/Requisition";
import DocumentOfNomenclature from "../views/DocumentOfNomenclature";
import Users from "../views/Users.vue";
import Inventory from "../views/Inventory.vue";
import Akt from "../views/Akt.vue";

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'Home',
        components: {
            layout: defaultLayout,
            content: Home
        }
    },
    {
        path: '/nomenclatures',
        name: 'nomenclatures',
        components: {
            layout: defaultLayout,
            content: NomenclatureGrid
        }
    },
    {
        path: '/documents',
        name: 'documents',
        components: {
            layout: defaultLayout,
            content: DocumentGrid
        }
    },
    {
        path: '/history',
        name: 'history',
        components: {
            layout: defaultLayout,
            content: Requisition
        }
    },
    {
        path: '/users',
        name: 'users',
        components: {
            layout: defaultLayout,
            content: Users
        }
    },
    {
        path: '/inventories',
        name: 'inventories',
        components: {
            layout: defaultLayout,
            content: Inventory
        }
    },
    {
        path: '/akt',
        name: 'akt',
        components: {
            layout: defaultLayout,
            content: Akt
        }
    },
    {
        path: `/documents-of-nomenclature/:nomenclatureId`,
        name: 'documents-of-nomenclature',
        components: {
            layout: defaultLayout,
            content: DocumentOfNomenclature
        }
    },
    {
        path: "*",
        redirect: "/"
    },
]

const router = new VueRouter({
    routes
})

export default router
