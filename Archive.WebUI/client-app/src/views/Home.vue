<template>
  <div class="home">
    <h2>Архив Предприятия</h2>
  </div>
</template>

<script>

export default {
  name: 'Home',
  data() {
    return {}
  },
  components: {}
}
</script>
<style lang="scss">
.home {
  height: calc(100vh - 150px);
}

.demo-container, #page {
  height: 100%;
  min-height: 300px;
}

.item {
  height: 100%;
}

#page p {
  font-size: 20px;
  padding-top: 10px;
  text-align: center;
}
</style>

