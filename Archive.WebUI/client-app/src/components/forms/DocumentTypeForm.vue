<template>
  <DxPopup
      :height="800"
      :width="360"
      position="center"
      title="Выберите тип документа"
      :visible="visible"

      :show-title="true"
      :resize-enabled="false"
      :close-on-outside-click="true"
      :hover-state-enabled="true"

      @hidden="cancel"
  >
    <div style="height: 100%">
      <DxList
          :data-source="dataSource"
          @item-click="onItemClick"
          key-expr="id"
          display-expr="name"
      />
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import DxList from "devextreme-vue/list";
import data from '../../data';

export default {
  name: "DocumentTypeForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    documentType: {
      type: Number,
    }
  },
  data() {
    return {
      dataSource: data.documentTypes,
    }
  },
  components: {
    DxPopup,
    DxList
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