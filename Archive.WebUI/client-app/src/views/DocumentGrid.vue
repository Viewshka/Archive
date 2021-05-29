<template>
  <div class="document-tree-list">
    <h2 style="margin-left: 5px">Все документы</h2>
    <DxTreeList
        :ref="gridRefName"
        :data-source="dataSource"

        :allow-column-resizing="true"
        :focused-row-enabled="true"
        :render-async="true"
        :hover-state-enabled="true"
        :show-row-lines="true"
        :allow-column-reordering="true"
        :auto-expand-all="false"
        v-model:selected-row-keys="selectedRowKeys"
        :root-value="null"
        key-expr="id"
        parent-id-expr="parentId"
        data-structure="plain"

        @row-dbl-click="dataGridRowDblClick"
        @toolbar-preparing="toolbarPreparing($event)"
    >
      <DxColumn
          caption="Обозначение"
          data-field="designation"
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
          caption="Тип документа"
          data-field="type"
      >
        <DxLookup :data-source="dataSourceDocumentTypes" value-expr="id" display-expr="name"/>
      </DxColumn>
      <DxColumn
          caption="Номенклатура"
          data-field="nomenclatureId"
      >
        <DxLookup :data-source="dataSourceNomenclatures" value-expr="id" :display-expr="nomenclatureDisplayExpr"/>
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

      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>
      <DxHeaderFilter :visible="true"/>
      <DxLoadPanel :enabled="true" :show-pane="true" :show-indicator="true"/>
      <DxPaging :enabled="true" :page-size="20"/>
      <DxSelection :recursive="true" :allow-select-all="false" mode="multiple"/>

      <template #buttonControl="{data}">
        <div class="dx-command-edit dx-command-edit-with-icons">
          <a v-if="currentUser.isUserArchivist"
             href="#"
             class="dx-link dx-icon-edit dx-link-icon"
             title="Редактировать"
             v-on:click="updateDocument(data.data)"
          ></a>
          <a href="#"
             class="dx-link dx-icon-find dx-link-icon"
             title="История использования документа"
             v-on:click="openDocumentHistory(data.data)"
          ></a>
          <a href="#"
             class="dx-link dx-icon-doc dx-link-icon"
             title="Просмотреть лист использования документа"
             v-on:click="openDocumentUsageList(data.data)"
          ></a>
        </div>
      </template>
    </DxTreeList>
    <PreviewForm
        v-if="previewFormData.visible && previewFormData.url"
        :visible.sync="previewFormData.visible"
        :document-subject="previewFormData.documentSubject"
        :url="previewFormData.url"
    />
    <ConstructDocumentEditForm
        v-if="documentEditFormVisible"
        :visible.sync="documentEditFormVisible"
        :title="documentEditFormData.title"
        :form-data="documentEditFormData.formData"
        :data-source-nomenclatures="dataSourceNomenclatures"
        :data-source-departments="dataSourceDepartments"
        @submit="constructDocumentSubmit"
    />
    <KitConstructDocumentEditForm
        v-if="kitDocumentsEditFormVisible"
        :visible.sync="kitDocumentsEditFormVisible"
        :title="documentEditFormData.title"
        :form-data="documentEditFormData.formData"
        :data-source-nomenclatures="dataSourceNomenclatures"
        :data-source-departments="dataSourceDepartments"
        @submit="kitConstructDocumentsSubmit"
    />
    <DocumentTypeForm
        v-if="documentTypeFormVisible"
        :visible.sync="documentTypeFormVisible"
        :document-type.sync="documentType"
    />
    <GiveOutDocumentsForm
        v-if="giveOutDocumentsFormData.visible"
        :visible.sync="giveOutDocumentsFormData.visible"
        :form-data="giveOutDocumentsFormData.formData"
        :data-source="dataSourceUsersForAutocomplete"
        @submit="giveOutDocumentsSubmit"
    />
    <DocumentHistoryForm
        v-if="historyFormData.visible"
        :data-source="historyFormData.dataSource"
        :title="historyFormData.title"
        :visible.sync="historyFormData.visible"
    />
  </div>
</template>

<script>

import PreviewForm from "../components/forms/PreviewForm";
import KitConstructDocumentEditForm from "../components/forms/KitConstructDocumentEditForm";
import ConstructDocumentEditForm from "../components/forms/ConstructDocumentEditForm";
import DocumentTypeForm from "../components/forms/DocumentTypeForm";
import GiveOutDocumentsForm from "../components/forms/GiveOutDocumentsForm";
import DocumentHistoryForm from "../components/forms/DocumentHistoryForm";

import DxTreeList, {
  DxColumn,
  DxScrolling,
  DxColumnChooser,
  DxFilterRow,
  DxSearchPanel,
  DxHeaderFilter,
  DxLoadPanel,
  DxPaging,
  DxLookup,
  DxSelection
}
  from 'devextreme-vue/tree-list';
import DxButton from "devextreme-vue/button";
import DxSelectBox from "devextreme-vue/select-box";

import notify from "devextreme/ui/notify";
import axios from "axios";
import * as AspNetData from "devextreme-aspnet-data-nojquery";
import data from '../data';
import {mapState} from 'vuex';

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
      previewFormData: {
        visible: false,
        documentSubject: '',
        url: null
      },
      documentEditFormData: {
        title: '',
        formData: {}
      },
      giveOutDocumentsFormData: {
        visible: false,
        formData: {}
      },
      documentEditFormVisible: false,
      kitDocumentsEditFormVisible: false,
      documentTypeFormVisible: false,
      documentType: null,
      dataSourceUsersForAutocomplete: [],
      selectedRowKeys: [],
      historyFormData: {
        visible: false,
        title: null,
        dataSource: [],
      },
    }
  },
  components: {
    PreviewForm,
    ConstructDocumentEditForm,
    DocumentTypeForm,
    KitConstructDocumentEditForm,
    GiveOutDocumentsForm,
    DocumentHistoryForm,
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
    DxSelectBox,
    DxSelection,
  },
  computed: {
    ...mapState(['currentUser']),
  },
  watch: {
    documentType: function (value) {
      if (!!parseInt(value))
        this.documentEditFormData.title += ` - ${data.documentTypes.find(t => t.id === value).name}`
      this.openNeededForm(value);
    },
    isLoaded: function (value) {
      if (this.isLoaded) {
        this.isLoaded = false;
        this.dataSourceDocuments = this.$refs[this.gridRefName].instance.getDataSource()._items
      }
    },
    selectedRowKeys: function (value) {
      console.log(value)
    },
  },
  async created() {
    await Promise.all(
        [
          this.initNomenclatures(),
          this.initDepartments(),
          this.initUsersForAutoComplete()
        ]
    )
  },
  methods: {
    openDocumentUsageList(data) {
      this.previewFormData.documentSubject = `Лист использования документа: ${data.name}`;
      this.previewFormData.visible = true;
      this.previewFormData.url = `api/document/${data.id}/generate`;
    },
    openDocumentHistory(data) {
      axios.get(`api/document/${data.id}/history`)
          .then(response => {
            this.historyFormData.title = data.name;
            this.historyFormData.visible = true;
            this.historyFormData.loading = true;
            this.historyFormData.dataSource = response.data;
          })
          .catch(response => {
            notify("Во время запроса произошла ошибка", 'error', 3000);
          })
          .finally(() => {
            this.historyFormData.loading = false;
          })
    },
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
    async initUsersForAutoComplete() {
      await axios.get(`api/user/users`)
          .then(response => {
            this.dataSourceUsersForAutocomplete = response.data;
          })
          .catch(response => {
            console.log(response)
          })
    },
    openNeededForm(documentType) {
      this.documentEditFormData.formData['type'] = documentType;

      if (documentType === this.$enums.documentTypes.drawing ||
          documentType === this.$enums.documentTypes.specification) {
        this.documentTypeFormVisible = false;
        this.documentEditFormVisible = true;

      } else if (documentType === this.$enums.documentTypes.kitCD) {
        this.documentTypeFormVisible = false;
        this.kitDocumentsEditFormVisible = true;

      } else {
        notify('В разработке', 'info', 3000);
      }
    },
    updateDocument(data) {
      this.documentEditFormData.formData = data;
      this.documentEditFormData.title = 'Редактирование документа';
      this.documentEditFormVisible = false;
      this.openNeededForm(data.type);
    },
    dataGridRowDblClick(row) {
      this.previewFormData.documentSubject = `${row.data.name} - ${data.documentTypes.find(t => t.id === row.data.type).name}`;
      this.previewFormData.visible = true;
      this.previewFormData.url = `api/file/${row.data.id}`
    },
    buttonAddDocumentClick() {
      this.documentEditFormData.title = `Регистрация документа`;
      this.documentEditFormVisible = false;
      this.documentEditFormData.formData = {};
      this.documentTypeFormVisible = true;
    },
    nomenclatureDisplayExpr(data) {
      return `${data.index} - ${data.name}`;
    },
    giveOutDocumentButtonClick() {
      console.log(this.getSelectedRowKeys())
      this.giveOutDocumentsFormData.formData['documents'] = this.getSelectedRowKeys();
      this.giveOutDocumentsFormData.visible = true;
    },
    constructDocumentSubmit(fileFormData) {
      if (this.documentEditFormData.formData.id) {
        axios.put(`api/document/update-drawing/${this.documentEditFormData.formData.id}`,
            this.documentEditFormData.formData)
            .then(response => {
              this.documentEditFormVisible = false;
              this.refreshDataGrid();
              this.uploadFile(this.documentEditFormData.formData.id, fileFormData)
              notify('Документ успешно обновлен', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      } else {
        axios.post(`api/document/create-drawing`, this.documentEditFormData.formData)
            .then(response => {
              this.documentEditFormVisible = false;
              this.refreshDataGrid();
              this.uploadFile(response.data, fileFormData)
              notify('Документ успешно зарегистрирован', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      }
    },
    kitConstructDocumentsSubmit(fileFormData) {
      if (this.documentEditFormData.formData.id) {
        axios.put(`api/document/update-kit-construct-documents/${this.documentEditFormData.formData.id}`,
            this.documentEditFormData.formData)
            .then(response => {
              this.kitDocumentsEditFormVisible = false;
              this.refreshDataGrid();
              this.uploadFile(this.documentEditFormData.formData.id, fileFormData)
              notify('Документ успешно обновлен', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      } else {
        axios.post(`api/document/create-kit-construct-documents`, this.documentEditFormData.formData)
            .then(response => {
              this.kitDocumentsEditFormVisible = false;
              this.refreshDataGrid();
              this.uploadFile(response.data, fileFormData)
              notify('Документ успешно зарегистрирован', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      }
    },
    getSelectedRowKeys() {
      return this.$refs[this.gridRefName].instance.getSelectedRowKeys("all");
    },
    giveOutDocumentsSubmit(formData) {
      axios.post(`api/requisition`, formData)
          .then(response => {
            this.giveOutDocumentsFormData.visible = false;
            this.refreshDataGrid();
            notify('Документ выдан', 'success', 3000);
          })
          .catch(response => {
            notify('Во время обработки запроса произошла ошибка', 'error', 3000);
          })
    },
    uploadFile(documentId, fileFormData) {
      axios.post(`api/file/upload/${documentId}`,
          fileFormData,
          {
            headers: {
              'Content-Type': 'multipart/form-data'
            }
          })
          .then(response => {
            console.log(response)
          })
          .catch(error => {
            console.log(error)
          })
    },
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'before',
            widget: 'dxButton',
            locateInMenu: 'auto',
            options: {
              text: 'Выдать документы',
              hint: 'Выдать документы',
              type: 'default',
              stylingMode: 'contained',
              visible: this.currentUser.isUserArchivist,
              elementAttr: {id: 'give-out-button'},
              onClick: () => this.giveOutDocumentButtonClick()
            }
          },
          {
            location: 'after',
            widget: 'dxButton',
            locateInMenu: 'auto',
            showText: 'inMenu',
            options: {
              text: 'Добавить',
              hint: 'Добавить',
              icon: 'plus',
              type: 'normal',
              stylingMode: 'contained',
              onClick: () => this.buttonAddDocumentClick(),
              visible: this.currentUser.isUserArchivist
            }
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

<style lang="scss">
.document-tree-list {
  height: calc(100vh - 150px);
}
#give-out-button{
 margin-left: 10px; 
}
</style>