<template>
  <div>
    <DxDataGrid
        :ref="dataGridRefName"
        :data-source="dataSource"

        :allow-column-resizing="true"
        :focused-row-enabled="true"
        :render-async="true"
        :hover-state-enabled="true"
        :allow-column-reordering="true"
        :auto-expand-all="true"
        
        :show-row-lines="true"
        :show-borders="true"
        
        key-expr="id"
    >
      <DxColumn
        caption="Обозначение"
        data-field="designation"
        data-type="string"
      />
      <DxColumn
        caption="Документ"
        data-field="name"
        data-type="string"
      />
      <DxColumn
          caption="Тип документа"
          data-field="type"
      >
        <DxLookup :data-source="dataSourceDocumentType" value-expr="id" display-expr="name"/>
      </DxColumn>
    </DxDataGrid>
  </div>
</template>

<script>
import DxDataGrid, {
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
  from 'devextreme-vue/data-grid';
import data from '../data'

export default {
  name: "DocumentsTreeList",
  props: {
    dataSource: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      dataGridRefName: 'dataGridRefName',
      dataSourceDocumentType: data.documentTypes
    }
  },
  created() {
    console.log(this.dataSource)
  },
  components: {
    DxDataGrid,
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
  },
}
</script>

<style scoped>

</style>