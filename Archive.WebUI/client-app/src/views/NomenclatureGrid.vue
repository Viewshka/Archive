<template>
  <div class="nomenclature-grid-height">
    <h2>Номенклатура</h2>
    <DxDataGrid
        style="height: 700px"
        :data-source="dataSource"
        :focused-row-enabled="true"
        key-expr="id"
    >
      <DxColumn data-field="name"/>
      <DxScrolling mode="virtual"/>
    </DxDataGrid>
  </div>
</template>

<script>
import DxDataGrid, {DxColumn, DxScrolling} from "devextreme-vue/data-grid";

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
  }
}
</script>

<style>
.nomenclature-grid-height{
  height: calc(100vh - 150px);
  margin: 10px 10px 10px 5px;
}
</style>