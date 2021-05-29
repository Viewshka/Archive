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
        @focused-row-changed="focusedRowChanged"
    >
      <DxColumn
          caption="Номер"
          data-field="number"
          data-type="string"
      />
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
        <div class="dx-command-edit dx-command-edit-with-icons"
             v-if="data.data.status !== $enums.requisitionStatus.canceled ||
                  data.data.static !== $enums.requisitionStatus.isDenied"
        >
          <a v-if="!data.data.isDenied && data.data.dateOfGiveOut && data.data.dateOfReturn === null"
             href="#"
             class="dx-link dx-icon-check dx-link-icon"
             title="Вернуть"
             v-on:click="returnDocument(data)"
          ></a>
          <a v-if="currentUser.isUserArchivist && data.data.status === $enums.requisitionStatus.new"
             href="#"
             class="dx-link dx-icon-close dx-link-icon"
             title="Отказать"
             v-on:click="deniedRequisition(data.data.id)"
          ></a>
          <a v-if="currentUser.isUserArchivist && data.data.status === $enums.requisitionStatus.new"
             href="#"
             class="dx-link dx-icon-box dx-link-icon"
             title="Готово к выдаче"
             v-on:click="readyToGiveOut(data.data.id)"
          ></a>
          <a v-if="!currentUser.isUserArchivist && data.data.status === $enums.requisitionStatus.new"
             href="#"
             class="dx-link dx-icon-close dx-link-icon"
             title="Отозвать заявку"
             v-on:click="canceledRequisition(data.data.id)"
          ></a>
          <a v-if="currentUser.isUserArchivist && data.data.status === $enums.requisitionStatus.readyToGiveOut && !data.data.dateOfGiveOut"
             href="#"
             class="dx-link dx-icon-chevronright dx-link-icon"
             title="Выдать документы"
             v-on:click="giveOutDocument(data.data.id)"
          ></a>
        </div>
      </template>
      <template #previewButtonTemplate="{data}">
        <DxButton
            id="preview-button"
            :text="focusedRow.buttonText"
            type="default"
            :disabled="focusedRow.buttonDisabled"
            @click="openPreview"
        />
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
    <RequisitionForm
        v-if="requisitionForm.visible"
        :visible.sync="requisitionForm.visible"
        :form-data="requisitionForm.formData"
        @submit="requisitionFormSubmit"
    />
    <RequisitionFormForArchivist
        v-if="requisitionFormForArchivist.visible"
        :visible.sync="requisitionFormForArchivist.visible"
        :form-data="requisitionFormForArchivist.formData"
        :data-source-users="dataSourceUsers"
        @submit="requisitionFormSubmit"
    />
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

import RequisitionForm from "../components/forms/requisition/RequisitionForm";
import RequisitionFormForArchivist from "../components/forms/requisition/RequisitionFormForArchivist";
import PreviewForm from "../components/forms/PreviewForm";

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
      requisitionForm: {
        visible: false,
        formData: {}
      },
      requisitionFormForArchivist: {
        visible: false,
        formData: {}
      },
      focusedRow: {
        requisitionId: null,
        buttonText: 'Предпросмотр',
        buttonDisabled: true,
      },
      previewFormData: {
        title: "",
        visible: false,
        url: null
      },
    }
  },
  computed: {
    ...mapState(['currentUser']),
    dataGrid: function () {
      return this.$refs[this.gridRefName].instance;
    },
  },
  components: {
    RequisitionFormForArchivist,
    DocumentsMasterDetail,
    RequisitionForm,
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
    openPreview() {
      this.previewFormData.url = `api/file/requisition/${this.focusedRow.requisitionId}`;
      this.previewFormData.visible = true;
    },
    focusedRowChanged(e) {
      let data = e.row && e.row.data;
      this.focusedRow.requisitionId = data.id;
      this.focusedRow.buttonText = `Предпросмотр ${data.number}`;
      this.focusedRow.buttonDisabled = false;
      this.previewFormData.title = `Заявка на выдачу документов №${data.number}`;
    },
    requisitionFormSubmit(formData) {
      axios.post(`api/requisition`, formData)
          .then(response => {
            this.requisitionForm.visible = false;
            this.requisitionFormForArchivist.visible = false;
            this.refreshDataGrid();
            notify('Заявка создана', 'success', 3000);
          })
          .catch(response => {
            notify('Во время обработки запроса произошла ошибка', 'error', 3000);
          })
    },
    createRequisition() {
      if (this.currentUser.isUserArchivist) {
        this.requisitionFormForArchivist.formData = {};
        this.requisitionFormForArchivist.visible = true;
      } else {
        this.requisitionForm.formData = {recipientId: this.currentUser.id};
        this.requisitionForm.visible = true;
      }
    },
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
            location: 'before',
            template: 'previewButtonTemplate'
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
              onClick: () => this.refreshDataGrid()
            }
          },
          {
            location: 'after',
            widget: 'dxButton',
            locateInMenu: 'auto',
            showText: 'inMenu',
            options: {
              text: 'Создать заявку',
              hint: 'Создать заявку',
              icon: 'plus',
              type: 'normal',
              stylingMode: 'contained',
              onClick: () => this.createRequisition()
            }
          },
      )
    },
    returnDocument(data) {
      confirm(`Вернуть документы?`, "Возврат")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/document/${data.data.id}/return`)
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
    canceledRequisition(id) {
      confirm(`Отменить заявку?`, "Отмена")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/requisition/${id}/canceled`)
                  .then(() => {
                    notify('Заявка отменена', 'success', 3000)
                    this.refreshDataGrid();
                  })
                  .catch(reason => {
                    console.log(reason)
                    notify('Во время обработки запроса произошла ошибка', 'error', 3000)
                  });
            }
          });
    },
    deniedRequisition(id) {
      confirm(`Отказаться от заявки?`, "Отказ")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/requisition/${id}/denied`)
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
    readyToGiveOut(id) {
      confirm(`Сообщить о готовности выдать документы?`, "Готовность к выдаче")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/requisition/${id}/ready`)
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
    giveOutDocument(id) {
      confirm(`Выдать документы?`, "Выдача")
          .then((dialogResult) => {
            if (dialogResult) {
              axios.put(`/api/requisition/${id}/give-out`)
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