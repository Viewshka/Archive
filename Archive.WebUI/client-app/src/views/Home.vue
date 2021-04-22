<template>
  <div>
    <h2 class="content-block">Домашняя страница</h2>
  </div>
</template>

<script>
import DxDataGrid, {DxColumn} from 'devextreme-vue/data-grid'

export default {
  name: 'Home',
  data() {
    return {}
  },
  components: {
    DxDataGrid,
    DxColumn
  }
}
</script>
<style lang="scss">
.logos-container {
  margin: 20px 0 40px 0;
  text-align: center;

  svg {
    display: inline-block;
  }
}

.devextreme-logo {
  width: 200px;
  height: 34px;
  margin-bottom: 17px;
}

.vue-logo {
  width: 180px;
  height: 62px;
}

.plus {
  margin: 20px 10px;
  width: 22px;
  height: 22px;
}

.screen-x-small .logos-container {
  svg {
    width: 100%;
    display: block;

    &.plus {
      margin: 0;
    }
  }
}
</style>

