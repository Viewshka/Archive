<template>
  <DxPopup
      height="auto"
      :width="900"
      position="center"
      :title="title"
      :visible="visible"

      :show-title="true"
      :resize-enabled="false"
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
              :col-span="1"
              :label="{text: 'Номер акта'}"
              data-field="number"
              editor-type="dxTextBox"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Дата акта'}"
              :editor-options="{placeholder:'Необязательно'}"
              editor-type="dxDateBox"
              data-field="documentDate"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text:'Документы'}"
              data-field="documentIds"
              template="documentsTemplate"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Номер протокола'}"
              data-field="protocolNumber"
              editor-type="dxTextBox"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text: 'Дата протокола'}"
              editor-type="dxDateBox"
              data-field="protocolDate"
          />
          <DxSimpleItem
              :col-span="1"
              :label="{text:'Вес документов'}"
              data-field="weigh"
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

        <template #documentsTemplate="{data}">
          <DocumentsDropDownBox
              :values="formData[data.dataField]"
              :data-source-documents="dataSourceDocuments"
              :on-values-changed="documentsValueChanged"
          />
        </template>
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import DxSelectBox from "devextreme-vue/select-box";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";
import DxDateBox from "devextreme-vue/date-box";
import axios from "axios";
import DocumentsDropDownBox from "../dropDowBoxes/DocumentsDropDownBox";

export default {
  name: "DestructionForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    formData: {
      type: Object,
      required: true
    },
    title:{
      type: String,
      required: true
    }
  },
  data() {
    return {
      formRefName: 'form',
      dataSourceDocuments: [],
    }
  },
  created() {
    axios.get('api/document')
        .then(response => {
          this.dataSourceDocuments = response.data;
        });
  },
  computed: {
    form: function () {
      return this.$refs[this.formRefName].instance;
    },
  },
  components: {
    DocumentsDropDownBox,
    DxPopup,
    DxSelectBox,
    DxForm,
    DxSimpleItem,
    DxGroupItem,
    DxButtonItem,
    DxDateBox,
  },
  methods: {
    documentsValueChanged(value) {
      this.formData['documentIds'] = value;
    },
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
  },
}
</script>

<style scoped>

</style>