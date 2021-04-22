<template>
  <div>
    <DxTreeView
        :data-source="dataSource"
        :expand-all-enabled="true"
        display-expr="name"
        key-expr="id"
        parent-id-expr="parentId"
        data-structure="plain"
    >
    </DxTreeView>
  </div>
</template>

<script>
import DxTreeView from "devextreme-vue/tree-view"

export default {
  name: "TreeViewNavigation",
  data() {
    return {
      dataSource:[
        {
          id: 1,
          name: 'Stores',
        }, {
          id: 2,
          parentId: 1,
          name: 'Super Mart of the West',
        }, {
          id: 3,
          parentId: 2,
          name: 'Video Players'
        }, {
          id: 4,
          parentId: 3,
          name: 'HD Video Player',
          price: 220
        }, {
          id: 5,
          parentId: 3,
          name: 'SuperHD Video Player',
          price: 270
        }, {
          id: 6,
          parentId: 2,
          name: 'Televisions',
        },
      ]
    }
  },
  components: {
    DxTreeView
  }
}
</script>

<style scoped>

</style>