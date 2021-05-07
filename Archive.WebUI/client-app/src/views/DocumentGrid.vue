<template>
  <div class="document-tree-list">
    <h2 style="margin-left: 5px">Все документы</h2>
    <DxDataGrid
        :ref="gridRefName"
        :data-source="dataSource"

        :allow-column-resizing="true"
        :focused-row-enabled="true"
        :render-async="true"
        :hover-state-enabled="true"
        :show-row-lines="true"
        :allow-column-reordering="true"

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

      <template #buttonControl="{data}">
        <div class="dx-command-edit dx-command-edit-with-icons">
          <a href="#"
             class="dx-link dx-icon-edit dx-link-icon"
             title="Редактировать"
             v-on:click="updateDocument(data.data)"
          ></a>
        </div>
      </template>
    </DxDataGrid>
    <PreviewForm
        v-if="previewFormData.visible"
        :visible.sync="previewFormData.visible"
        :document-subject="previewFormData.documentSubject"
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
        :visible.sync="giveOutDocumentsFormVisible"
    />
  </div>
</template>

<script>

import PreviewForm from "../components/forms/PreviewForm";
import KitConstructDocumentEditForm from "../components/forms/KitConstructDocumentEditForm";
import ConstructDocumentEditForm from "../components/forms/ConstructDocumentEditForm";
import DocumentTypeForm from "../components/forms/DocumentTypeForm";
import GiveOutDocumentsForm from "../components/forms/GiveOutDocumentsForm";

import DxDataGrid, {
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
  from 'devextreme-vue/data-grid';
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
        documentSubject: ''
      },
      documentEditFormData: {
        title: '',
        formData: {}
      },
      documentEditFormVisible: false,
      kitDocumentsEditFormVisible: false,
      documentTypeFormVisible: false,
      documentType: null,
      giveOutDocumentsFormVisible: false,
    }
  },
  components: {
    PreviewForm,
    ConstructDocumentEditForm,
    DocumentTypeForm,
    KitConstructDocumentEditForm,
    GiveOutDocumentsForm,
    DxDataGrid,
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
    DxSelectBox
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
    }
  },
  async created() {
    await Promise.all(
        [
          this.initNomenclatures(),
          this.initDepartments(),
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
      this.previewFormData.documentSubject = `${row.data.subject} - ${data.documentTypes.find(t => t.id === row.data.type).name}`;
      this.previewFormData.visible = true;
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
      this.giveOutDocumentsFormVisible = true;
    },
    constructDocumentSubmit() {
      if (this.documentEditFormData.formData.id) {
        axios.put(`api/document/update-drawing/${this.documentEditFormData.formData.id}`,
            this.documentEditFormData.formData)
            .then(response => {
              this.documentEditFormVisible = false;
              this.refreshDataGrid();
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
              notify('Документ успешно зарегистрирован', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      }
    },
    kitConstructDocumentsSubmit() {
      if (this.documentEditFormData.formData.id) {
        axios.put(`api/document/update-kit-construct-documents/${this.documentEditFormData.formData.id}`,
            this.documentEditFormData.formData)
            .then(response => {
              this.kitDocumentsEditFormVisible = false;
              this.refreshDataGrid();
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
              notify('Документ успешно зарегистрирован', 'success', 3000);
            })
            .catch(response => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      }
    },
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'after',
            widget: 'dxButton',
            locateInMenu: 'auto',
            options: {
              text: 'Выдача',
              hint: 'Выдача',
              type: 'normal',
              stylingMode: 'contained',
              onClick: () => this.giveOutDocumentButtonClick(),
              visible: this.currentUser.isUserArchivist
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
</style>