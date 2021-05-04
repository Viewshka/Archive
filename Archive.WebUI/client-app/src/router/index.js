import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from '../views/Home.vue';
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
    path: '/all-documents',
    name: 'documents',
    components: {
      layout: defaultLayout,
      content: DocumentTreeList
    }
  },
  {
    path: '/draws',
    name: 'draws',
    components: {
      layout: defaultLayout,
      content: DocumentTreeList
    }
  },
  {
    path: '/specifications',
    name: 'specifications',
    components: {
      layout: defaultLayout,
      content: DocumentTreeList
    }
  },
  {
    path: '/akts',
    name: 'akts',
    components: {
      layout: defaultLayout,
      content: DocumentTreeList
    }
  },
  {
    path: '/inventory',
    name: 'inventory',
    components: {
      layout: defaultLayout,
      content: DocumentTreeList
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
