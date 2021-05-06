<template>
  <div class="document-tree-list">
    <h2 style="margin-left: 5px">Документы</h2>
    <DxTreeList
        :ref="gridRefName"
        :data-source="dataSource"
        :allow-column-resizing="true"
        :focused-row-enabled="true"
        :render-async="true"
        parent-id-expr="parentId"
        data-structure="plain"
        key-expr="id"
        @row-dbl-click="treeListRowDblClick"
        @toolbar-preparing="toolbarPreparing($event)"
    >
      <DxColumn
          caption="Номер документа"
          data-field="number"
          data-type="string"
      />
      <DxColumn
          caption="Наименование"
          data-field="subject"
          data-type="string"
      />
      <DxColumn
          caption="Дата"
          data-field="date"
          data-type="date"
      />
      <DxColumn
          caption="Тип документа"
          data-field="type"
      >
        <DxLookup :data-source="dataSourceDocumentTypes" value-expr="id" display-expr="name"/>
      </DxColumn>
      <DxColumn
          caption="Примечание"
          data-field="note"
          data-type="string"
      />
      <DxColumn
          caption="Управление"
          type="buttons"
          :hiding-priority="10"
          cell-template="buttonControl"
          alignment="center"
      />
      <template #buttonControl="{data}">
        <div class="dx-command-edit dx-command-edit-with-icons">
          <a href="#"
             class="dx-link dx-icon-edit dx-link-icon"
             title="Редактировать"
             v-on:click="updateDocument(data.data)"
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

      <template #buttonAddDocumentTemplate>
        <DxButton
            text="Добавить"
            type="normal"
            icon="plus"
            @click="buttonAddDocumentClick"
        />
      </template>
    </DxTreeList>
    <PreviewForm
        v-if="previewFormData.visible"
        :visible.sync="previewFormData.visible"
        :document-subject="previewFormData.documentSubject"
    />
    <ConstructDocumentEditForm
        v-if="documentEditFormData.visible"
        :visible.sync="documentEditFormData.visible"
        :title="documentEditFormData.title"
        :form-data="documentEditFormData.formData"
        :data-source-nomenclatures="dataSourceNomenclatures"
        :data-source-departments="dataSourceDepartments"
        :data-source-documents="dataSourceDocuments"
    />
    <DocumentTypeForm
        v-if="documentTypeFormVisible"
        :visible.sync="documentTypeFormVisible"
        :documentType.sync="documentType"
    />
  </div>
</template>

<script>

import PreviewForm from "../components/forms/PreviewForm";
import ConstructDocumentEditForm from "../components/forms/ConstructDocumentEditForm";
import DocumentTypeForm from "../components/forms/DocumentTypeForm";

import DxTreeList, {
  DxColumn,
  DxScrolling,
  DxColumnChooser,
  DxFilterRow,
  DxSearchPanel,
  DxHeaderFilter,
  DxLoadPanel,
  DxPaging,
  DxLookup
}
  from 'devextreme-vue/tree-list';
import DxButton from "devextreme-vue/button";
import notify from "devextreme/ui/notify";
import axios from "axios";
import * as AspNetData from "devextreme-aspnet-data-nojquery";
import data from '../data';
const dataSource = AspNetData.createStore({
  key: 'id',
  loadUrl: `/api/document`,
  onBeforeSend: (method, ajaxOptions) => {
    ajaxOptions.xhrFields = {withCredentials: true};
  },
});

export default {
  name: "DocumentGrid",
  data() {
    return {
      gridRefName: 'dataGrid',
      dataSource,
      dataSourceDocumentTypes: data.documentTypes,
      dataSourceNomenclatures: [],
      dataSourceDepartments: [],
      dataSourceDocuments: [],
      previewFormData: {
        visible: false,
        documentSubject: ''
      },
      documentEditFormData: {
        visible: false,
        title: '',
        formData: {}
      },
      documentTypeFormVisible: false,
      documentType: null,
    }
  },
  components: {
    PreviewForm,
    ConstructDocumentEditForm,
    DocumentTypeForm,
    DxTreeList,
    DxColumn,
    DxScrolling,
    DxColumnChooser,
    DxFilterRow,
    DxSearchPanel,
    DxHeaderFilter,
    DxLoadPanel,
    DxPaging,
    DxButton,
    DxLookup,
  },
  watch: {
    documentType: async function (value) {
      if (!!parseInt(this.documentType))
        this.openNeededForm(value);
    }
  },
  async created() {
    await Promise.all(
        [
          await this.initNomenclatures(),
          await this.initDepartments(),
          await this.initDocuments()
        ]
    )
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
    async initDepartments() {
      await axios.get(`api/department`)
          .then(response => {
            this.dataSourceDepartments = response.data;
          })
          .catch(response => {
            console.log(response)
          })
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
    openNeededForm(documentType) {
      if (documentType === this.$enums.documentTypes.drawing ||
          documentType === this.$enums.documentTypes.specification) {
        this.documentTypeFormVisible = false;
        this.documentType = documentType;
        this.documentEditFormData.visible = true;
      } else {
        notify('В разработке', 'info', 3000);
      }
    },
    updateDocument(data) {
      this.documentEditFormData.formData = data;
      this.documentEditFormData.title = 'Редактирование документа';
      this.documentEditFormData.visible = false;
      this.openNeededForm(data.type);
    },
    treeListRowDblClick(row) {
      this.previewFormData.documentSubject = `${row.data.subject} - ${data.types.find(t => t.id === row.data.type).name}`;
      this.previewFormData.visible = true;
    },
    buttonAddDocumentClick() {
      this.documentEditFormData.formData = {};
      this.documentEditFormData.title = 'Добавление документа';
      this.documentEditFormData.visible = false;
      this.documentTypeFormVisible = true;
      this.documentType = null;
    },
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'after',
            template: 'buttonAddDocumentTemplate'
          },
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
.document-tree-list {
  height: calc(100vh - 150px);
}
</style>