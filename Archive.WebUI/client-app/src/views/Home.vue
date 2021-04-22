<template>
  <DxResponsiveBox id="app">
    <DxRow :ratio="0"/>
    <DxRow :ratio="0"/>
    <DxRow :ratio="0"/>
    <DxCol :ratio="0.7" screen="md lg"/>
    <DxCol :ratio="2"/>

    <DxItem>
      <DxLocation screen="sm md lg" :row="0" :col="0" :colspan="3"/>
      <DxLocation screen="xs " :row="0" :col="0"/>
      <template #default>
        <div>
          <NavBar/>
        </div>
      </template>
    </DxItem>

    <DxItem>
      <DxLocation screen="md lg" :row="1" :col="1"/>
      <DxLocation screen="xs sm" :row="1" :col="0"/>
      <template #default>
        <div>
          <DocumentGrid/>
        </div>
      </template>
    </DxItem>

    <DxItem>
      <DxLocation screen="md lg" :row="1" :col="0"/>
      <template #default>
        <div>
          <TreeViewNavigation/>
        </div>
      </template>
    </DxItem>

    <DxItem>
      <DxLocation screen="md lg" :row="2" :col="0" :colspan="3"/>
      <DxLocation screen="xs sm" :row="2" :col="0"/>
      <template #default>
        <Footer/>
      </template>
    </DxItem>
  </DxResponsiveBox>
</template>

<script>
import {DxResponsiveBox, DxItem, DxLocation, DxCol, DxRow} from 'devextreme-vue/responsive-box';
import DxScrollView from "devextreme-vue/scroll-view";

import Footer from "../components/static/Footer"
import NavBar from "../components/static/NavBar";
import DocumentGrid from "./DocumentGrid";
import TreeViewNavigation from "../components/TreeViewNavigation";

export default {
  name: 'Home',
  data() {
    return {
      screen(width) {
        return (width < 900) ? 'sm' : 'lg';
      },
    }
  },
  components: {
    Footer,
    NavBar,
    DocumentGrid,
    TreeViewNavigation,
    DxResponsiveBox,
    DxItem,
    DxLocation,
    DxCol,
    DxRow,
    DxScrollView
  }
}
</script>
<style lang="scss">
.logos-container {
  margin: 20px 0 40px 0;
  text-align: center;

  svg {
    display: inline-block;
  }
}

.demo-container, #page {
  height: 100%;
  min-height: 300px;
}

.item {
  height: 100%;
}

#page p {
  font-size: 20px;
  padding-top: 10px;
  text-align: center;
}
</style>

