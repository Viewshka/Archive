<template>
  <div class="nomenclature-grid-height">
    <h2 style="margin-left: 5px">Номенклатура</h2>
    <DxDataGrid
        style="height: 700px"
        :data-source="dataSource"
        :focused-row-enabled="true"
        key-expr="id"
    >
      <DxColumn data-field="name"/>
      <DxScrolling mode="virtual"/>
      <DxColumnChooser :enabled="true" mode="select"/>
      <DxSearchPanel :visible="true"/>
      <DxFilterRow :visible="true"/>,
      <DxHeaderFilter :visible="true"/>
    </DxDataGrid>
  </div>
</template>

<script>
import DxDataGrid, {
  DxColumn,
  DxColumnChooser,
  DxFilterRow,
  DxHeaderFilter,
  DxScrolling,
  DxSearchPanel
} from "devextreme-vue/data-grid";

export default {
  name: "NomenclatureGrid",
  data() {
    return {
      dataSource: [
        {
          id: 1,
          name: "Номенклатура подразделение 1",
        },
        {
          id: 2,
          name: "Номенклатура подразделение 2",
        },
        {
          id: 3,
          name: "Номенклатура подразделение 3",
        },
      ]
    }
  },
  components: {
    DxDataGrid,
    DxColumn,
    DxScrolling,
    DxColumnChooser,
    DxFilterRow,
    DxSearchPanel,
    DxHeaderFilter
  }
}
</script>

<style>
.nomenclature-grid-height{
  height: calc(100vh - 150px);
}
</style>