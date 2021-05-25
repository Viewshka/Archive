<template>
  <DxPopup
      height="auto"
      :width="500"
      position="center"
      :title="title"
      :show-title="true"
      :resize-enabled="false"
      :visible="visible"
      :close-on-outside-click="false"
      :hover-state-enabled="true"

      @hidden="cancel"
  >
    <div>
      <DxForm
          :ref="formRefName"
          :form-data="formData"
          :col-count="2"
      >
        <DxGroupItem
            :col-count="2"
            :col-span="2"
        >
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Приоритет'}"
              template="priorityTemplate"
          />
        </DxGroupItem>

        <DxGroupItem
            :col-count="2"
            :col-span="2"
            vertical-alignment="bottom"
        >
          <DxButtonItem
              v-on:keyup="submit"
              :button-options="{text: 'Сохранить',icon:'check', type: 'default',onClick: submit}"
              horizontal-alignment="right"
          />
          <DxButtonItem
              v-on:keyup.esc="cancel"
              :button-options="{text: 'Отменить',icon:'close',type: 'danger',styling:'outline',onClick: cancel}"
              horizontal-alignment="left"
          />
        </DxGroupItem>

        <template #priorityTemplate="{data}">
          <DxSelectBox
              :data-source="dataSourcePriority"
              v-model:value="formData['priority']"
              value-expr="id"
              display-expr="name"

              :open-on-field-click="true"
              :show-drop-down-button="true"
              :show-clear-button="false"
          />
        </template>
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import DxPopup from "devextreme-vue/popup";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";
import DxSelectBox from 'devextreme-vue/select-box';
import data from '../../data'

export default {
  name: "PriorityEditForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    formData: {
      type: Object,
      required: true
    },
  },
  data() {
    return {
      formRefName: 'form',
      dataSourcePriority: data.priority
    }
  },
  components: {
    DxPopup,
    DxForm,
    DxSimpleItem,
    DxGroupItem,
    DxButtonItem,
    DxSelectBox,
  },
  computed: {
    form: function () {
      return this.$refs[this.formRefName].instance;
    },
  },
  methods: {
    cancel: function () {
      this.$emit('update:visible', false);
    },
    submit: function () {
      const validateResult = this.form.validate();

      if (validateResult.isValid) {
        this.$emit('submit', this.formData);
      }
    },
  },
}
</script>

<style scoped>

</style>