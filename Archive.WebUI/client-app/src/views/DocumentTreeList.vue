<template>
  <div class="document-grid">
    <h2 style="margin-left: 5px">Документы</h2>
    <DxTreeList
        :ref="treeListRefName"
        :data-source="dataSource"
        :allow-column-resizing="true"
        :focused-row-enabled="true"
        :render-async="true"
        items-expr="structure"
        data-structure="tree"
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
        <DxLookup :data-source="dataSourceTypes" value-expr="id" display-expr="name"/>
      </DxColumn>
      <DxColumn
          caption="Примечание"
          data-field="note"
          data-type="string"
      />

      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>
      <DxHeaderFilter :visible="true"/>
      <DxLoadPanel :enabled="true" :show-pane="true" :show-indicator="true"/>
      <DxPaging :enabled="true" :page-size="20"/>
    </DxTreeList>
    <PreviewForm :visible.sync="previewFormData.visible" :document-subject="previewFormData.documentSubject"/>
  </div>
</template>

<script>

import PreviewForm from "../components/forms/PreviewForm";

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
  from 'devextreme-vue/tree-list'

import data from '../data'

export default {
  name: "DocumentGrid",
  data() {
    return {
      treeListRefName: 'treeList',
      dataSource: data.documents,
      dataSourceTypes: data.types,
      previewFormData: {
        visible: false,
        documentSubject: ''
      },
    }
  },
  components: {
    PreviewForm,
    DxTreeList,
    DxColumn,
    DxScrolling,
    DxColumnChooser,
    DxFilterRow,
    DxSearchPanel,
    DxHeaderFilter,
    DxLoadPanel,
    DxPaging,
    DxLookup
  },
  methods: {
    treeListRowDblClick(row) {
      console.log(row.data)
      this.previewFormData.documentSubject = row.data.subject;
      this.previewFormData.visible = true;
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
              onClick: this.refreshTreeList.bind(this)
            }
          },
      )
    },
    async refreshTreeList() {
      this.$refs[this.treeListRefName].instance.refresh();
    },
  }
}
</script>

<style>
.document-grid {
  height: calc(100vh - 150px);
}
</style>