<template>
  <div id="root" class="dx-swatch-arktika-scheme" v-if="currentUser"
       v-bind:class="{ 'dx-swatch-arktika-scheme-dark': isDark }">
    <div :class="cssClasses">
      <RouterView
          name="layout"
          :navTitle="navTitle"
          :title="title"
          :is-x-small="screen.isXSmall"
          :is-large="screen.isLarge"
      >
        <div class="content">
          <RouterView name="content"/>
        </div>
        <template #footer>
          <Footer/>
        </template>
      </RouterView>
    </div>
  </div>
</template>

<script>
import Footer from "./components/static/Footer";
import {sizes, subscribe, unsubscribe} from "./utils/media-query";

import {mapState} from 'vuex';

function getScreenSizeInfo() {
  const screenSizes = sizes();

  return {
    isXSmall: screenSizes["screen-x-small"],
    isLarge: screenSizes["screen-large"],
    cssClasses: Object.keys(screenSizes).filter(cl => screenSizes[cl])
  };
}

function getFavoriteTheme() {
  return window.localStorage.getItem('favorite-theme') === 'dark'
}

export default {
  name: 'app',
  data() {
    return {
      title: this.$appInfo.title,
      navTitle: this.$appInfo.navTitle,
      screen: getScreenSizeInfo(),
      isDark: getFavoriteTheme()
    };
  },
  components: {
    Footer
  },
  mounted() {
    subscribe(this.screenSizeChanged);
  },

  beforeDestroy() {
    unsubscribe(this.screenSizeChanged);
  },
  computed: {
    cssClasses() {
      return ["app"].concat(this.screen.cssClasses);
    },
    ...mapState(["currentUser"]),
  },
  methods: {
    screenSizeChanged() {
      this.screen = getScreenSizeInfo();
    }
  },
  created() {
    this.$store.dispatch('INIT_CURRENT_USER');
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

#root {
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

.dx-swatch-arktika-scheme-dark {
  .app {
    @import "./themes/generated/variables.additional.scss";
    background-color: lighten($base-bg, 5) !important;
  }
}

.dx-toolbar-after {
  margin-right: 10px;
}

.dx-fileuploader-content > .dx-fileuploader-upload-button {
  display: none;
}
</style>
