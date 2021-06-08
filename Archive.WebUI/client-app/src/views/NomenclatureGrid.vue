<template>
  <div class="nomenclature-grid-height">
    <h2 style="margin-left: 5px">Номенклатура</h2>
    <DxDataGrid
        :ref="gridRefName"
        :data-source="dataSource"
        :focused-row-enabled="true"
        :allow-column-resizing="true"
        :render-async="true"
        @toolbar-preparing="toolbarPreparing($event)"
        @row-dbl-click="dataGridRowDblClick"
        @focused-row-changed="focusedRowChanged"
    >
      <DxColumn
          caption="Номер"
          data-field="index"
          data-type="string"
      />
      <DxColumn
          caption="Наименование"
          data-field="name"
          data-type="string"
      />
      <DxColumn
          caption="Подразделение"
          data-field="departmentId"
          data-type="string"
      >
        <DxLookup :data-source="dataSourceDepartments" value-expr="id" display-expr="shortName"/>
      </DxColumn>
      <DxColumn
          caption="Год"
          data-field="year"
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
             v-on:click="updateNomenclature(data.data)"
          ></a>
          <a href="#"
             class="dx-link dx-icon-trash dx-link-icon"
             title="Удалить"
             v-on:click="deleteNomenclature(data.data)"
          ></a>
          <a href="#"
             class="dx-link dx-icon-search dx-link-icon"
             title="Просмотреть опись дела"
             v-on:click="openInventory(data.data)"
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

      <template #buttonGenerateInventory="{data}">
        <DxButton
            :visible="currentUser.isUserArchivist"
            :text="focusedRow.buttonInventoryText"
            type="default"
            :disabled="focusedRow.buttonDisabled"
            @click="generateInventory"
        />
      </template>
      <template #destructionAkt="{data}">
        <DxButton
            id="destruction-button"
            :visible="currentUser.isUserArchivist"
            :text="focusedRow.buttonDestructionAktText"
            type="danger"
            :disabled="focusedRow.buttonDisabled"
            @click="destructionDocumentButtonClick"
        />
      </template>
    </DxDataGrid>
    <NomenclatureEditForm
        v-if="nomenclatureEditForm.visible"
        :title="nomenclatureEditForm.title"
        :visible.sync="nomenclatureEditForm.visible"
        :form-data="nomenclatureEditForm.formData"
        :data-source-department="dataSourceDepartments"
        @submit="nomenclatureSubmit"
    />
    <PreviewForm
        v-if="previewFormData.visible && previewFormData.url"
        :visible.sync="previewFormData.visible"
        :document-subject="previewFormData.title"
        :url="previewFormData.url"
    />
    <DestructionForm
        v-if="destructionForm.visible"
        :visible.sync="destructionForm.visible"
        :form-data="destructionForm.formData"
        :title="destructionForm.title"
        @submit="destructionFormSubmit"
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
import {confirm} from 'devextreme/ui/dialog'
import NomenclatureEditForm from "../components/forms/NomenclatureEditForm";
import PreviewForm from "../components/forms/PreviewForm";

import * as AspNetData from 'devextreme-aspnet-data-nojquery'
import axios from "axios";
import notify from "devextreme/ui/notify";
import {mapState} from "vuex";
import DestructionForm from "../components/forms/DestructionForm";

const dataSource = AspNetData.createStore({
  key: 'id',
  loadUrl: `/api/nomenclature`,
  onBeforeSend: (method, ajaxOptions) => {
    ajaxOptions.xhrFields = {withCredentials: true};
  },
});

export default {
  name: "NomenclatureGrid",
  data() {
    return {
      dataSource,
      gridRefName: "dataGrid",
      dataSourceDepartments: [],
      nomenclatureEditForm: {
        visible: false,
        formData: {},
        title: null
      },
      previewFormData: {
        title: "",
        visible: false,
        url: null
      },
      focusedRow: {
        nomenclatureId: null,
        index: null,
        buttonInventoryText: 'Сформировать опись дела',
        buttonDestructionAktText: 'Списание',
        buttonDisabled: true
      },
      destructionForm: {
        visible: false,
        formData: {},
        title: ''
      },
    }
  },
  components: {
    NomenclatureEditForm,
    PreviewForm,
    DestructionForm,
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
  watch: {},
  async created() {
    await this.initDepartments();
  },
  computed: {
    ...mapState(['currentUser']),
  },
  methods: {
    destructionFormSubmit(formData) {
      axios.post(`api/document/destruction-akt`, formData)
          .then(response => {
            console.log(response.data)
            notify("Акт создан", "success", 2000);
          })
          .catch(error => {
            console.log(error);
            notify("Не удалось создать акт списания", "error", 2000);
          });
    },
    focusedRowChanged(e) {
      let data = e.row && e.row.data;
      this.focusedRow.nomenclatureId = data.id;
      this.focusedRow.buttonInventoryText = `Сформировать опись дела: ${data.index}`;
      this.focusedRow.buttonDestructionAktText = `Списание ${data.index}`;
      this.focusedRow.buttonDisabled = false;
      this.focusedRow.index = data.index;
    },
    generateInventory() {
      axios.post(`api/nomenclature/${this.focusedRow.nomenclatureId}`)
          .then(response => {
            notify("Опись дела сформирована", "success", 3000);
          })
          .catch(error => {
            console.log(error);
            notify("Ошибка генерации описи дела", "error", 2000);
          });
    },
    openInventory(data) {
      this.previewFormData.title = `Внутрення опись документов дела: "${data.index} - ${data.name}"`;
      this.previewFormData.visible = true;
      this.previewFormData.url = `api/nomenclature/${data.id}/inventory`;
    },
    dataGridRowDblClick(data) {
      this.$router.push(`documents-of-nomenclature/${data.data.id}`);
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
    nomenclatureSubmit() {
      if (this.nomenclatureEditForm.formData.id) {
        axios.put(`api/nomenclature/${this.nomenclatureEditForm.formData.id}`, this.nomenclatureEditForm.formData)
            .then((response) => {
              notify('Номенклатурное дело успещно изменено', 'success', 3000);
              this.nomenclatureEditForm.visible = false;
              this.refreshDataGrid();
            })
            .catch((response) => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000)
            })
      } else {
        axios.post('api/nomenclature', this.nomenclatureEditForm.formData)
            .then((response) => {
              notify('Номенклатурное дело успещно создано', 'success', 3000);
              this.nomenclatureEditForm.visible = false;
              this.refreshDataGrid();
            })
            .catch((response) => {
              notify('Во время обработки запроса произошла ошибка', 'error', 3000);
            })
      }
    },
    addNomenclature() {
      this.nomenclatureEditForm.formData = {};
      this.nomenclatureEditForm.visible = true;
      this.nomenclatureEditForm.title = "Создание номенклатурного дела";
    },
    updateNomenclature(data) {
      this.nomenclatureEditForm.formData = data;
      this.nomenclatureEditForm.visible = true;
      this.nomenclatureEditForm.title = `Редактирование - ${data.index} - ${data.name}`;
    },
    deleteNomenclature(data) {
      confirm(`Вы уверены, что хотите удалить номенклатурное дело <b>'${data.index} - ${data.name}'</b>?`, "Удаление")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.delete(`/api/nomenclature/${data.id}`)
                  .then(() => {
                    this.refreshDataGrid();
                    notify('Номенклатурное дело удалено', 'success', 3000);
                  })
                  .catch(reason => {
                    console.log(reason)
                    notify('Во время обработки запроса произошла ошибка', 'error', 3000)
                  });
            }
          });
    },
    destructionDocumentButtonClick() {
      this.destructionForm.visible = true;
      this.destructionForm.formData = {nomenclatureId: this.focusedRow.nomenclatureId};
      this.destructionForm.title = `Списание документов дела ${this.focusedRow.index}`
    },
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'before',
            template: 'destructionAkt'
          },
          {
            location: 'before',
            template: 'buttonGenerateInventory'
          },
          {
            location: 'after',
            widget: 'dxButton',
            locateInMenu: 'auto',
            showText: 'inMenu',
            options: {
              text: "Добавить",
              hint: 'Добавить',
              type: "normal",
              icon: "plus",
              stylingMode: 'contained',
              onClick: () => this.addNomenclature()
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
  },
}
</script>

<style>
.nomenclature-grid-height {
  height: calc(100vh - 150px);
}

#destruction-button {
  margin-left: 10px;
}
</style>