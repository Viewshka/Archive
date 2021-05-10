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
              :label="{text:'Получатель'}"
              data-field="userId"
              template="recipientTemplate"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text:'Дата выдачи'}"
              data-field="dateOfGiveOut"
              template="dateOfGiveOutTemplate"
          />
          <DxSimpleItem
              :col-span="2"
              :label="{text:'Характер использования'}"
              data-field="usageType"
              template="usageTypeTemplate"
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

        <template #usageTypeTemplate="{data}">
          <DxSelectBox
              v-model:value="formData[data.dataField]"
              :data-source="dataSourceUsageType"

              display-expr="name"
              value-expr="id"

              :search-enabled="true"
              :open-on-field-click="true"
              :show-drop-down-button="true"
              :show-clear-button="true"
          />
        </template>
        <template #recipientTemplate="{data}">
          <DxSelectBox
              v-model:value="formData['recipientId']"
              :data-source="dataSource"
              
              display-expr="briefName"
              value-expr="userId"
              search-expr="briefName"
              search-mode="contains"

              :search-enabled="true"
              :open-on-field-click="true"
              :show-drop-down-button="true"
              :show-clear-button="true"
          />
        </template>
        <template #dateOfGiveOutTemplate="{data}">
          <DxDateBox
            v-model:value="formData[data.dataField]"
            data-type="date"
            placeholder="Не обязательно"
          />
        </template>
      </DxForm>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import DxSelectBox from 'devextreme-vue/select-box';
import DxDateBox from "devextreme-vue/date-box";
import {DxButtonItem, DxForm, DxGroupItem, DxSimpleItem} from "devextreme-vue/form";
import data from '../../data'

export default {
  name: "GiveOutDocumentsForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    formData: {
      type: Object,
      required: true
    },
    dataSource: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      formRefName: 'form',
      dataSourceUsageType: data.documentUsageTypes
    }
  },
  computed: {
    form: function () {
      return this.$refs[this.formRefName].instance;
    },
  },
  components: {
    DxPopup,
    DxSelectBox,
    DxForm,
    DxSimpleItem,
    DxGroupItem,
    DxButtonItem,
    DxDateBox,
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
  },
}
</script>

<style scoped>

</style>