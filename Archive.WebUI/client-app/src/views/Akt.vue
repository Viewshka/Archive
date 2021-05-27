<template>
  <div class="akt-grid-height">
    <h2 style="margin-left: 5px">Акты</h2>
    <DxDataGrid
        :ref="gridRefName"
        :data-source="dataSource"
        :focused-row-enabled="true"
        :allow-column-resizing="true"
        :render-async="true"
        @toolbar-preparing="toolbarPreparing($event)"
    >
      <DxColumn
          caption="Номер"
          data-field="number"
          data-type="string"
      />
      <DxColumn
          caption="Наименование"
          data-field="name"
          data-type="string"
      />
      <DxColumn
          caption="Дата документа"
          data-field="documentDate"
          data-type="date"
      />
      <DxColumn
          caption="Номенклатурное дело"
          data-field="nomenclatureId"
      >
        <DxLookup :data-source="dataSourceNomenclatures" value-expr="id" :display-expr="nomenclatureDisplayExpr"/>
      </DxColumn>

      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>
      <DxHeaderFilter :visible="true"/>
      <DxLoadPanel :enabled="true" :show-pane="true" :show-indicator="true"/>
      <DxPaging :enabled="true" :page-size="20"/>

    </DxDataGrid>
    <PreviewForm
        v-if="previewFormData.visible && previewFormData.url"
        :visible.sync="previewFormData.visible"
        :document-subject="previewFormData.title"
        :url="previewFormData.url"
    />
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
  DxPaging
} from "devextreme-vue/data-grid";
import DxButton from "devextreme-vue/button";
import PreviewForm from "../components/forms/PreviewForm";

import * as AspNetData from 'devextreme-aspnet-data-nojquery'
import axios from "axios";

const dataSource = AspNetData.createStore({
  key: 'id',
  loadUrl: `/api/document/akt`,
  onBeforeSend: (method, ajaxOptions) => {
    ajaxOptions.xhrFields = {withCredentials: true};
  },
});

export default {
  name: "Akt",
  data() {
    return {
      dataSource,
      gridRefName: "dataGrid",
      dataSourceNomenclatures: [],
      previewFormData: {
        title: "",
        visible: false,
        url: null
      },
    }
  },
  components: {
    PreviewForm,
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
    DxButton
  },
  async created() {
    await this.initNomenclatures()
  },
  methods: {
    async initNomenclatures() {
      await axios.get(`api/nomenclature`)
          .then(response => {
            this.dataSourceNomenclatures = response.data;
          })
          .catch(response => {
            console.log(response)
          })
    },
    nomenclatureDisplayExpr(data) {
      return `${data.index} - ${data.name}`;
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
              onClick: this.refreshDataGrid.bind(this)
            }
          },
      )
    },
    async refreshDataGrid() {
      this.$refs[this.gridRefName].instance.refresh();
    },
  }
}
</script>

<style>
.akt-grid-height {
  height: calc(100vh - 150px);
}
</style>