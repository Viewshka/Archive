<template>
  <DxPopup
      :height="800"
      :width="800"
      position="center"
      title="Управление доступом"
      :show-title="true"
      :resize-enabled="true"
      :visible="visible"
      :close-on-outside-click="false"
      :hover-state-enabled="true"

      @hidden="cancel"
  >
    <div>Приоритет</div>
  </DxPopup>
</template>

<script>
import DxPopup from "devextreme-vue/popup";

export default {
  name: "PriorityEditForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
  },
  data() {
    return {
      formRefName: 'form',
    }
  },
  components: {
    DxPopup
  },
  computed: {
    // form: function () {
    //   return this.$refs[this.formRefName].instance;
    // },
  },
  methods: {
    cancel: function () {
      this.$emit('update:visible', false);
    },
  },
}
</script>

<style scoped>

</style>