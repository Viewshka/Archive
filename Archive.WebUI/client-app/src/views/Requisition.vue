<template>
  <div style="height: calc(100vh - 150px);">
    <h2 style="margin-left: 5px">Использование документов</h2>
    <DxDataGrid
        :ref="gridRefName"
        :data-source="dataSource"
        :focused-row-enabled="true"
        :allow-column-resizing="true"
        :render-async="true"
        @toolbar-preparing="toolbarPreparing($event)"
        @selection-changed="selectionChanged"
    >
      <DxColumn
          caption="Выдавший"
          data-field="giverId"
      >
        <DxLookup :data-source="dataSourceUsers" value-expr="userId" display-expr="briefName"/>
      </DxColumn>
      <DxColumn
          caption="Дата выдачи"
          data-field="dateOfGiveOut"
          data-type="date"
      />
      <DxColumn
          caption="Получатель"
          data-field="recipientId"
      >
        <DxLookup :data-source="dataSourceUsers" value-expr="userId" display-expr="briefName"/>
      </DxColumn>
      <DxColumn
          caption="Дата возврата"
          data-field="dateOfReturn"
          data-type="date"
      />
      <DxColumn
          caption="Характер использования"
          data-field="usageType"
      >
        <DxLookup :data-source="dataSourceUsageType" value-expr="id" display-expr="name"/>
      </DxColumn>

      <DxMasterDetail
          style="background: grey"
          :enabled="false"
          template="masterDetailTemplate"
      />
      
      <template #masterDetailTemplate="{data}">
        <DocumentsMasterDetail :data-source="getDataSourceDocumentsFiltered(data)"/>
      </template>
      
      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>
      <DxHeaderFilter :visible="true"/>
      <DxLoadPanel :enabled="true" :show-pane="true" :show-indicator="true"/>
      <DxPaging :enabled="true" :page-size="20"/>
      <DxSelection mode="single"/>
    </DxDataGrid>
  </div>
</template>

<script>
import DxDataGrid, {
  DxColumn,
  DxColumnChooser,
  DxFilterRow,
  DxHeaderFilter,
  DxScrolling,
  DxSearchPanel,
  DxLoadPanel,
  DxLookup,
  DxPaging,
  DxMasterDetail,
  DxSelection
} from "devextreme-vue/data-grid";
import DxButton from "devextreme-vue/button";
import {confirm} from 'devextreme/ui/dialog'
import data from '../data'
import * as AspNetData from 'devextreme-aspnet-data-nojquery'
import axios from "axios";
import notify from "devextreme/ui/notify";
import DocumentsMasterDetail from "../components/DocumentsMasterDetail";

const dataSource = AspNetData.createStore({
  key: 'id',
  loadUrl: `/api/requisition`,
  onBeforeSend: (method, ajaxOptions) => {
    ajaxOptions.xhrFields = {withCredentials: true};
  },
});

export default {
  name: "Requisition",
  data() {
    return {
      dataSource,
      gridRefName: "dataGrid",
      dataSourceDocuments: [],
      dataSourceUsers: [],
      dataSourceUsageType: data.documentUsageTypes,
    }
  },
  computed: {
    dataGrid: function () {
      return this.$refs[this.gridRefName].instance;
    }
  },
  components: {
    DocumentsMasterDetail,
    DxDataGrid,
    DxColumn,
    DxScrolling,
    DxColumnChooser,
    DxFilterRow,
    DxSearchPanel,
    DxHeaderFilter,
    DxLookup,
    DxLoadPanel,
    DxPaging,
    DxButton,
    DxMasterDetail,
    DxSelection
  },
  async created() {
    await Promise.all([
      this.initDocuments(),
      this.initUsers()
    ])
  },
  methods: {
    getDataSourceDocumentsFiltered(data){
      return this.dataSourceDocuments.filter(doc=>data.data.documents.includes(doc.id));
    },
    selectionChanged(e) {
      e.component.collapseAll(-1);
      e.component.expandRow(e.currentSelectedRowKeys[0]);
    },
    async initDocuments() {
      await axios.get(`api/document`)
          .then(response => {
            this.dataSourceDocuments = response.data;
          })
          .catch(response => {
            console.log(response)
          })
    },
    async initUsers() {
      await axios.get(`api/user/users`)
          .then(response => {
            this.dataSourceUsers = response.data;
          })
          .catch(response => {
            console.log(response)
          })
    },
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'after',
            widget: 'dxButton',
            locateInMenu: 'auto',
            showText: 'inMenu',
            options: {
              text: 'Обновить',
              hint: 'Обновить',
              icon: 'refresh',
              type: 'normal',
              stylingMode: 'contained',
              onClick: this.refreshDataGrid()
            }
          },
      )
    },
    async refreshDataGrid() {
      this.dataGrid.refresh();
    },
  }
}
</script>

<style scoped>

</style>