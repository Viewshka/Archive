<template>
  <DxPopup
      height="auto"
      :width="600"
      position="center"
      title="Выдача документов"
      :visible="visible"

      :show-title="true"
      :resize-enabled="false"
      :close-on-outside-click="true"
      :hover-state-enabled="true"

      @hidden="cancel"
  >
    <div style="height: 100%">

    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";

export default {
  name: "GiveOutDocumentsForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    }
  },
  data() {
    return {
    }
  },
  components: {
    DxPopup,
  },
  methods: {
    cancel: function () {
      this.$emit('update:visible', false);
    },
    onItemClick(item) {
      this.$emit('update:documentType', item.itemData.id);
    },
  },
}
</script>

<style scoped>

</style>