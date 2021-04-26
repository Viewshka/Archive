<template>
  <DxPopup
      height="auto"
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
        <template #noteTemplate="{data}">
          <DxTextArea
              :height="150"
              :max-height="150"
          >
          </DxTextArea>
        </template>
        <template #nomenclatureTemplate="{data}">
          <NomenclatureDropDownBox/>
        </template>
        <template #documentTemplate="{data}">
          <DocumentDropDownBox/>
        </template>
        <template #fileUploaderTemplate="{data}">
          <DxFileUploader
              upload-mode="useButtons"
          />
        </template>
        <DxGroupItem
            :col-count="2"
            :col-span="2"
        >
          <DxSimpleItem
              :col-span="2"
              :label="{text:'Принадлежность'}"
              data-field="parentId"
              template="documentTemplate"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text:'Номер документа'}"
              data-field="number"
              editor-type="dxTextBox"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Наименование'}"
              editor-type="dxTextBox"
              data-field="subject"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Примечание'}"
              data-field="note"
              template="noteTemplate"
          />
     
          <DxSimpleItem
              :col-span="2"
              :label="{text: 'Номенклатура'}"
              data-field="nomenclatureId"
              template="nomenclatureTemplate"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Дата'}"
              data-field="date"
              editor-type="dxDateBox"
          />
        </DxGroupItem>
        <DxGroupItem
            :col-count="2"
            :col-span="2"
        >
          <DxSimpleItem template="fileUploaderTemplate"/>
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
import DxTextArea from 'devextreme-vue/text-area'
import DxFileUploader from 'devextreme-vue/file-uploader'
import DxSelectBox from 'devextreme-vue/select-box'

import NomenclatureDropDownBox from "../dropDowBoxes/NomenclatureDropDownBox";
import DocumentDropDownBox from "../dropDowBoxes/DocumentDropDownBox";

import data from '../../data'

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
    formData: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      formRefName: 'form',
      dataSourceNomenclature: data.nomenclatures,
    }
  },
  components: {
    NomenclatureDropDownBox,
    DocumentDropDownBox,
    DxPopup,
    DxForm,
    DxSimpleItem,
    DxGroupItem,
    DxButtonItem,
    DxTextArea,
    DxFileUploader,
    DxSelectBox
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