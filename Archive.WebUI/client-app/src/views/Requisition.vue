<template>
  <div style="height: calc(100vh - 150px);">
    <h2 v-if="currentUser.isUserArchivist" style="margin-left: 5px">Заявки на выдачу документов</h2>
    <h2 v-else style="margin-left: 5px">Мои заявки</h2>
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
      <DxColumn
          caption="Статус"
          data-field="status"
      >
        <DxLookup :data-source="dataSourceStatus" value-expr="id" display-expr="name"/>
      </DxColumn>
      <DxColumn
          caption="Управление"
          type="buttons"
          :hiding-priority="10"
          cell-template="buttonControl"
          alignment="center"
      />

      <DxMasterDetail
          style="background: grey"
          :enabled="false"
          template="masterDetailTemplate"
      />

      <template #masterDetailTemplate="{data}">
        <DocumentsMasterDetail :data-source="getDataSourceDocumentsFiltered(data)"/>
      </template>

      <template #buttonControl="{data}">
        <div class="dx-command-edit dx-command-edit-with-icons">
          <a v-if="currentUser.isUserArchivist && data.data.dateOfReturn === null"
             href="#"
             class="dx-link dx-icon-check dx-link-icon"
             title="Возврашен"
             v-on:click="returnDocument(data.data)"
          ></a>
        </div>
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
import {mapState} from "vuex";
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
      dataSourceStatus: data.requisitionStatus,
    }
  },
  computed: {
    ...mapState(['currentUser']),
    dataGrid: function () {
      return this.$refs[this.gridRefName].instance;
    },
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
    getDataSourceDocumentsFiltered(data) {
      return this.dataSourceDocuments.filter(doc => data.data.documents.includes(doc.id));
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
              onClick: () => this.refreshDataGrid()
            }
          },
      )
    },
    returnDocument(data) {
      confirm(`Вернуть документы?`, "Возврат")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/document/${data.id}/return`)
                  .then(() => {
                    this.refreshDataGrid();
                  })
                  .catch(reason => {
                    console.log(reason)
                    notify('Во время обработки запроса произошла ошибка', 'error', 3000)
                  });
            }
          });
    },
    async refreshDataGrid() {
      this.dataGrid.refresh();
    },
  }
}
</script>

<style scoped>

</style>