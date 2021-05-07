<template>
  <DxDropDownBox
      :ref="dropDownBoxRefName"
      :drop-down-options="dropDownOptions"
      :data-source="dataSource"
      :value="currentValue"
      display-expr="shortName"
      value-expr="id"
      content-template="contentTemplate"
      :show-clear-button="true"
      :show-drop-down-button="true"
  >
    <template #contentTemplate="{}">
      <div>
        <DxDataGrid
            id="typeDropBox"
            :data-source="dataSource"
            :height="400"
            :render-async="true"
            :remote-operations="false"
            :selected-row-keys="[currentValue]"
            :hover-state-enabled="true"
            :show-row-lines="true"
            :column-auto-width="true"
            :allow-column-reordering="true"
            :column-min-width="50"
            :allow-column-resizing="true"
            :word-wrap-enabled="true"
            :on-selection-changed="onSelectionChanged"
            :focused-row-enabled="true"
            :focused-row-key="currentValue"
            value-expr="id"
            key-expr="id"
        >
          <DxColumn data-field="shortName" caption="Подразделение" :calculate-display-value="calculateDisplayValue"/>

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

import DxDropDownBox from 'devextreme-vue/drop-down-box'
import DxDataGrid, {
  DxColumn,
  DxSearchPanel,
  DxPaging,
  DxScrolling,
  DxSelection,
  DxLookup
} from 'devextreme-vue/data-grid'


const dropDownBoxRefName = 'dropDownBoxRef';
export default {
  name: "DepartmentDropDownBox",
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
    dataSource: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      dropDownBoxRefName,
      currentValue: this.value,
      dropDownOptions: {width: 500},
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
    calculateDisplayValue(item){
      return item && `${item.fullName} (${item.shortName})`;
    },
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

<style scoped>

</style>