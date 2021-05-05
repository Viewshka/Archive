<template>
  <DxPopup
      height="auto"
      :width="700"
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
        >
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Индекс'}"
              data-field="index"
              editor-type="dxTextBox"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Наименование'}"
              editor-type="dxTextBox"
              data-field="name"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Подразделение'}"
              data-field="departmentId"
              template="departmentTemplate"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Год'}"
              editor-type="dxTextBox"
              data-field="year"
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
        
        <template #departmentTemplate="{data}">
          <DepartmentDropDownBox
            :value="formData[data.dataField]"
            :data-source="dataSourceDepartment"
          />
        </template>
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";

import NomenclatureDropDownBox from "../dropDowBoxes/NomenclatureDropDownBox";
import DepartmentDropDownBox from "../dropDowBoxes/DepartmentDropDownBox";

export default {
  name: "NomenclatureEditForm",
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
    dataSourceDepartment:{
      type: Array,
      required: true
    }
  },
  data() {
    return {
      formRefName: 'form',
    }
  },
  components: {
    NomenclatureDropDownBox,
    DepartmentDropDownBox,
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
      console.log(this.formData)
      if (validateResult.isValid) {
        this.$emit('submit', this.formData);
      }
    },
  }
}
</script>

<style scoped>

</style>