<template>
  <div>
    <DxDataGrid
        style="height: 800px"
        :data-source="dataSource"
        :focused-row-enabled="true"
        key-expr="id"
    >
      <DxColumn data-field="id"/>
      <DxColumn data-field="name"/>
      <DxColumn data-field="age"/>
      <DxScrolling mode="virtual"/>
    </DxDataGrid>
  </div>
</template>

<script>
import DxDataGrid, {DxColumn, DxScrolling} from 'devextreme-vue/data-grid'

export default {
  name: "DocumentGrid",
  data() {
    return {

      dataSource: [
        {
          id: 1,
          name: "Name 1",
          age: 12
        },
        {
          id: 2,
          name: "Name 2",
          age: 23
        },
        {
          id: 3,
          name: "Name 3",
          age: 17
        },
        {
          id: 4,
          name: "Name 4",
          age: 52
        },
        {
          id: 5,
          name: "Name 5",
          age: 23
        },
        {
          id: 6,
          name: "Name 6",
          age: 65
        }
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

<style scoped>

</style>