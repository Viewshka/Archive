<template>
  <DxPopup
      :height="800"
      :width="800"
      position="center"
      :title="documentSubject"
      :show-title="true"
      :resize-enabled="true"
      :visible="visible"
      :close-on-outside-click="true"
      :hover-state-enabled="true"
      :full-screen="fullScreen"

      @hidden="cancel"
  >
    <div style="height: 100%">
      <iframe v-if="url" :src="url" height="100%" width="100%"></iframe>
    </div>

    <DxToolbarItem
        widget="dxButton"
        :options="buttonOptions"
        location="after">
    </DxToolbarItem>
  </DxPopup>
</template>

<script>
import DxPopup, {DxToolbarItem} from "devextreme-vue/popup";
import * as AspNetData from "devextreme-aspnet-data-nojquery";

export default {
  name: "PreviewForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    documentSubject: {
      type: String,
      required: true
    },
    url: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      fullScreen: false,
      buttonOptions: {
        icon: 'fullscreen',
        stylingMode: 'text',
        onClick: () => this.onClickFullScreenButton()
      }
    }
  },
  components: {
    DxPopup,
    DxToolbarItem
  },
  methods: {
    cancel: function () {
      this.$emit('update:visible', false);
    },
    onClickFullScreenButton() {
      this.fullScreen = !this.fullScreen;
    }
  },
}
</script>

<style scoped>

</style>