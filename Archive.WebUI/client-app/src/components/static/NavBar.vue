﻿<template>
  <header class="header-component">
    <DxToolbar class="header-toolbar">
      <DxItem
          :visible="menuToggleEnabled"
          location="before"
          css-class="menu-button"
      >
        <DxButton
            icon="menu"
            styling-mode="text"
            @click="toggleMenuFunc"
            slot-scope="_"
        />
      </DxItem>

      <DxItem
          v-if="title"
          location="before"
          css-class="header-title dx-toolbar-label"
      >
        <div slot-scope="_">{{ title }}</div>
      </DxItem>

      <DxItem
          location="after"
          locate-in-menu="auto"
          menu-item-template="menuUserItem"
      >
        <div slot-scope="_">
          <DxButton
              class="user-button authorization"
              width="100%"
              height="100%"
              styling-mode="text"
          >
            <UserPanel :menu-items="userMenuItems" menu-mode="context"/>
          </DxButton>
        </div>
      </DxItem>
    </DxToolbar>
  </header>
</template>

<script>
import DxButton from "devextreme-vue/button";
import DxToolbar, {DxItem} from "devextreme-vue/toolbar";
import DxScrollView from "devextreme-vue/scroll-view";
import UserPanel from "./UserPanel";
import notify from "devextreme/ui/notify";
import axios from "axios";
import {mapState} from 'vuex';

export default {

  name: "NavBar",
  props: {
    menuToggleEnabled: Boolean,
    title: String,
    toggleMenuFunc: Function,
    logOutFunc: Function
  },

  data() {
    return {
      userMenuItems: [
        {
          text: "Удалить настройки",
          icon: "clearformat",
          onClick: this.onClearStorage
        },
        {
          text: "Выйти",
          icon: "runner",
          onClick: this.onLogoutClick
        }
      ]
    };
  },
  methods: {
    toggleMenuFuncS() {
    },
    onLogoutClick() {
      axios.get('api/account/logout')
          .then((response) => {
            this.$store.dispatch('CLEAR_CURRENT_USER');
            window.location.href = "http://localhost:1111/account/Login";
          })
          .catch(c => {
            console.log(c)
            notify("Возникла ошибка выхода из системы", "error", 3000);
          });
    },
    onClearStorage() {
      window.localStorage.clear();
      window.location.reload();
    }
  },
  created() {
  },
  computed: {
    ...mapState(["currentUser"]),
  },
  components: {
    UserPanel,
    DxToolbar,
    DxItem,
    DxScrollView,
    DxButton,
  }
}
</script>

<style lang="scss">
@import "../../themes/generated/variables.base.scss";
@import "../../dx-styles.scss";

.header-component {
  flex: 0 0 auto;
  z-index: 1;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);

  .dx-toolbar .dx-toolbar-item.menu-button > .dx-toolbar-item-content .dx-icon {
    color: $base-accent;
  }
}

.dx-toolbar.header-toolbar .dx-toolbar-items-container .dx-toolbar-after {
  padding: 0 40px;

  .screen-x-small & {
    padding: 0 20px;
  }
}

.dx-toolbar .dx-toolbar-item.dx-toolbar-button.menu-button {
  width: $side-panel-min-width;
  text-align: center;
  padding: 0;
}

.header-title .DxItem-content {
  padding: 0;
  margin: 0;
}

.dx-theme-generic {
  .dx-toolbar {
    padding: 10px 0;
  }

  .user-button > .dx-button-content {
    //padding: 3px;
  }
}
</style>