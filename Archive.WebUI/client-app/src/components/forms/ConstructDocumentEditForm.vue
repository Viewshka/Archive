<template>
  <DxPopup
      height="auto"
      :width="800"
      position="center"
      :title="title"
      :show-title="true"
      :resize-enabled="false"
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
              :label="{text:'Принадлежность'}"
              data-field="parentId"
              template="documentTemplate"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text:'Обозначение'}"
              data-field="designation"
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
              :label="{text: 'Дата документа'}"
              data-field="documentDate"
              editor-type="dxDateBox"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Дата поступления'}"
              data-field="incomingDate"
              template="incomingDateTemplate"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Дата хранения'}"
              data-field="storageDate"
              template="storageDateTemplate"
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

        <template #storageDateTemplate="data">
          <DxDateBox
              v-model:value="formData['storageDate']"
              placeholder="Необязательно"
          />
        </template>
        <template #incomingDateTemplate="data">
          <DxDateBox
              v-model:value="formData['incomingDate']"
              placeholder="Необязательно"
          />
        </template>
        <template #noteTemplate="{data}">
          <DxTextArea
              v-model:value="formData[data.dataField]"
              :height="150"
              :max-height="150"
          >
          </DxTextArea>
        </template>
        <template #nomenclatureTemplate="{data}">
          <NomenclatureDropDownBox
              :value="formData[data.dataField]"
              :on-value-changed="nomenclatureChanged"
              :data-source="dataSourceNomenclatures"
              :data-source-departments="dataSourceDepartments"
          />
        </template>
        <template #documentTemplate="{data}">
          <DocumentDropDownBox
              :value="parseInt(formData[data.dataField]) === 0 ? null : formData[data.dataField]"
              :on-value-changed="parentIdChanged"
              :data-source="dataSourceDocuments"
          />
        </template>
        <template #fileUploaderTemplate="{data}">
          <DxFileUploader
              upload-mode="useButtons"
              @value-changed="fileUploaderValueChanged"
          />
        </template>
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";
import DxTextArea from 'devextreme-vue/text-area';
import DxDateBox from 'devextreme-vue/date-box';
import DxFileUploader from 'devextreme-vue/file-uploader';
import DxSelectBox from 'devextreme-vue/select-box';

import NomenclatureDropDownBox from "../dropDowBoxes/NomenclatureDropDownBox";
import DocumentDropDownBox from "../dropDowBoxes/DocumentDropDownBox";
import axios from "axios";

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
    },
    dataSourceNomenclatures: {
      type: Array,
      required: true
    },
    dataSourceDepartments: {
      type: Array,
      required: true
    },
  },
  data() {
    return {
      formRefName: 'form',
      dataSourceDocuments: [],
      files: [],
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
    DxSelectBox,
    DxDateBox
  },
  computed: {
    form: function () {
      return this.$refs[this.formRefName].instance;
    },
  },
  created() {
    console.log(this.formData);
    axios.get('api/document')
        .then(response => {
          this.dataSourceDocuments = response.data;
        });
  },
  methods: {
    fileUploaderValueChanged(data) {
      this.files = data.value;
    },
    nomenclatureChanged(value) {
      this.formData['nomenclatureId'] = value;
    },
    parentIdChanged(value) {
      this.formData['parentId'] = value;
    },
    cancel: function () {
      this.$emit('update:visible', false);
    },
    submit: function () {
      const validateResult = this.form.validate();

      let formData = new FormData();
      if (this.files.length > 0)
        formData.append('file', this.files[0], this.files[0].name)

      if (validateResult.isValid) {
        this.$emit('submit', formData);
      }
    },
  }
}
</script>

<style>

</style>