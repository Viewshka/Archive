<template>
  <div class="users-grid-height">
    <h2 style="margin-left: 5px">Пользователи</h2>
    <DxDataGrid
        :ref="gridRefName"
        :data-source="dataSource"
        :focused-row-enabled="true"
        :allow-column-resizing="true"
        :allow-column-reordering="true"
        :render-async="true"
        @toolbar-preparing="toolbarPreparing($event)"
    >
      <DxColumn
          caption="ФИО"
          data-field="fullName"
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
          caption="Роль"
          data-field="roles"
      >
        <DxLookup :data-source="dataSourceRoles" value-expr="id" display-expr="name"/>
      </DxColumn>
      <DxColumn
          caption="Приоритет"
          data-field="priority"
      >
        <DxLookup :data-source="dataSourcePriority" value-expr="id" display-expr="name"/>
      </DxColumn>

      <DxColumn
          v-if="currentUser.isUserArchivist"
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
             title="Изменить приоритет"
             v-on:click="editPriority(data.data)"
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
    </DxDataGrid>
    <PriorityEditForm
        v-if="priorityForm.visible"
        :visible="priorityForm.visible"
        :form-data="priorityForm.formData"
        :title="priorityForm.title"
        @submit="prioritySubmit"
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
import PriorityEditForm from "../components/forms/PriorityEditForm";

import {confirm} from 'devextreme/ui/dialog'
import data from '../data';
import * as AspNetData from 'devextreme-aspnet-data-nojquery'
import axios from "axios";
import notify from "devextreme/ui/notify";
import {mapState} from "vuex";

const dataSource = AspNetData.createStore({
  key: 'id',
  loadUrl: `/api/user/all-users`,
  onBeforeSend: (method, ajaxOptions) => {
    ajaxOptions.xhrFields = {withCredentials: true};
  },
});

export default {
  name: "Users",
  data() {
    return {
      dataSource,
      gridRefName: "dataGrid",
      dataSourceDepartments: [],
      dataSourcePriority: data.priority,
      dataSourceRoles: [],
      priorityForm: {
        visible: false,
        formData: {},
        title: ''
      }
    }
  },
  components: {
    PriorityEditForm,
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
    await Promise.all([
      this.initDepartments(),
      this.initRoles()
    ])
  },
  computed: {
    ...mapState(['currentUser']),
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
    async initRoles() {
      await axios.get(`api/role`)
          .then(response => {
            this.dataSourceRoles = response.data;
          })
          .catch(response => {
            console.log(response)
          })
    },
    editPriority(data) {
      this.priorityForm.title = `Приоритет пользователя: ${data.fullName}`;
      this.priorityForm.formData = {id: data.id,priority: data.priority};
      this.priorityForm.visible = true;
    },
    prioritySubmit(formData) {
      axios.put(`api/user/priority`, formData)
          .then(response => {
            this.priorityForm.visible = false;
            this.refreshDataGrid();
            notify('Приоритет пользователя изменен', 'success', 3000);
          })
          .catch(error => {
            console.log(error);
            notify('Во время обработки запроса произошла ошибка', 'error', 3000);
          });
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
  },
}
</script>

<style>
.users-grid-height {
  height: calc(100vh - 150px);
}
</style>