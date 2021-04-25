import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from '../views/Home.vue';
import login from "../views/Login";
import simpleLayout from '../layouts/SingleCard';
import defaultLayout from '../layouts/SideNavOuterToolbar';

import NomenclatureGrid from "../views/NomenclatureGrid";
import DocumentTreeList from "../views/DocumentTreeList";

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
      content: DocumentTreeList
    }
  },
  {
    path: "/login",
    name: "login",
    components: {
      layout: simpleLayout,
      content: login
    },
    props: {
      layout: {
        title: "Войти"
      }
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
