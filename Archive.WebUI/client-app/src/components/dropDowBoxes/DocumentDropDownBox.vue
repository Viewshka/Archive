﻿<template>
  <DxDropDownBox
      :ref="dropDownBoxRefName"
      :drop-down-options="dropDownOptions"
      :data-source="dataSource"
      :value="currentValue"
      display-expr="name"
      value-expr="id"
      content-template="contentTemplate"
      placeholder="Выберите документ (необязательно)"
      :show-clear-button="true"
      :show-drop-down-button="true"
  >
    <template #contentTemplate="{}">
      <div>
        <DxDataGrid
            :data-source="dataSource"
            :allow-column-resizing="true"
            :focused-row-enabled="true"
            :render-async="true"
            :remote-operations="false"
            :selected-row-keys="[currentValue]"
            :focused-row-key="currentValue"
            :on-selection-changed="onSelectionChanged"
            :column-min-width="50"
            value-expr="id"
            key-expr="id"
        >
          <DxColumn 
              data-field="name" 
              caption="Документ"
          />
          <DxColumn 
              data-field="type" 
              caption="Тип документа"
          >
            <DxLookup :data-source="dataSourceDocumentType" value-expr="id" display-expr="name"/>
          </DxColumn>

          <DxSearchPanel :visible="true" :width="450"/>
          <DxPaging :enabled="true" :page-size="20"/>
          <DxScrolling mode="virtual" row-rendering-mode="virtual" column-rendering-mode="virtual"/>
          <DxSelection mode="single"/>
        </DxDataGrid>
      </div>
    </template>
  </DxDropDownBox>
</template>

<script>
import DxDropDownBox from "devextreme-vue/drop-down-box";
import DxDataGrid, {
  DxColumn,
  DxLookup,
  DxPaging,
  DxScrolling,
  DxSearchPanel,
  DxSelection,
} from "devextreme-vue/data-grid";

import data from '../../data';

const dropDownBoxRefName = 'dropDownBoxRef';
export default {
  name: "DocumentDropDownBox",
  props: {
    value: {
      type: String,
      default: null
    },
    onValueChanged: {
      type: Function,
      default: () => function () {
      }
    },
    dataSource:{
      type: Array,
      required: true
    }
  },
  data() {
    return {
      dropDownBoxRefName,
      currentValue: this.value,
      dropDownOptions: {width: 500},
      dataSourceDocumentType: data.documentTypes,
    }
  },
  components: {
    DxDropDownBox,
    DxDataGrid,
    DxColumn,
    DxSearchPanel,
    DxPaging,
    DxScrolling,
    DxSelection,
    DxLookup,
  },
  methods: {
    onSelectionChanged(selectionChangedArgs) {
      this.currentValue = selectionChangedArgs.selectedRowKeys[0];
      if (selectionChangedArgs.selectedRowKeys.length > 0) {
        this.onValueChanged(this.currentValue);
        this.$refs[dropDownBoxRefName].instance.close();
      }
    },
  }
}
</script>

<style>

</style>