<template>
  <div id="app">
    <router-view/>
  </div>
</template>

<script>

export default {
  name: 'app',
  data() {
    return {
      title: this.$appInfo.title,
      navTitle: this.$appInfo.navTitle,
    };
  },
  computed: {},
  methods: {},
  created() {
  },
}
</script>

<style lang="scss">
html,
body {
  margin: 0;
  min-height: 100%;
  height: 100%;
}

* {
  box-sizing: border-box;
}

.app {
  @import "./themes/generated/variables.base.scss";
  background-color: darken($base-bg, 5);
  display: flex;
  height: 100%;
  width: 100%;
}
</style>
