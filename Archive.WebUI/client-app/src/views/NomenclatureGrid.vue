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
    >
      <DxColumn
          caption="Индекс"
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
        </div>
      </template>

      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>
      <DxHeaderFilter :visible="true"/>
      <DxLoadPanel :enabled="true" :show-pane="true" :show-indicator="true"/>
      <DxPaging :enabled="true" :page-size="20"/>

      <template #buttonAddNomenclatureTemplate>
        <DxButton
            text="Добавить"
            type="normal"
            icon="plus"
            @click="addNomenclature"
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

import * as AspNetData from 'devextreme-aspnet-data-nojquery'
import axios from "axios";
import notify from "devextreme/ui/notify";

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
      }
    }
  },
  components: {
    NomenclatureEditForm,
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
    await this.initDepartments();
  },
  methods: {
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
    toolbarPreparing(e) {
      e.toolbarOptions.items.unshift(
          {
            location: 'after',
            template: 'buttonAddNomenclatureTemplate'
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
</style>