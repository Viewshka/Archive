<template>
  <DxPopup
      :height="800"
      :width="800"
      position="center"
      :title="title"
      :show-title="true"
      :resize-enabled="true"
      :visible="visible"
      :close-on-outside-click="true"
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
            name="basicInfo"
        >
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Сообщение'}"
              editor-type="dxTextArea"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Email для связи'}"
              editor-type="dxTextBox"
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
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";

export default {
  name: "ConstructDocumentEditForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    formData:{
      type: Object,
      required: true
    }
  },
  data() {
    return {
      formRefName: 'form',
    }
  },
  components: {
    DxPopup,
    DxForm,
    DxSimpleItem,
    DxGroupItem,
    DxButtonItem,
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
      // if (validateResult.isValid) {
      //   this.$emit('submit', this.formData);
      // }
    },
  }
}
</script>

<style scoped>

</style>