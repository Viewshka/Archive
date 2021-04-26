<template>
  <DxDropDownBox
      :ref="dropDownBoxRefName"
      :drop-down-options="dropDownOptions"
      :data-source="dataSourceNomenclatures"
      :value.sync="currentValue"
      display-expr="fullName"
      value-expr="id"
      content-template="contentTemplate"
  >
    <template #contentTemplate="{}">
      <div>
        <DxDataGrid
            id="typeDropBox"
            :data-source="dataSourceNomenclatures"
            :height="400"
            :render-async="true"
            :remote-operations="true"
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
          <DxColumn data-field="departmentId" caption="Подразделение">
            <DxLookup :data-source="dataSourceDepartments" value-expr="id" display-expr="shortName"/>
          </DxColumn>
          <DxColumn data-field="fullName" caption="Номенклатурное дело"/>
          
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

import data from '../../data';

const dropDownBoxRefName = 'dropDownBoxRef';
export default {
  name: "NomenclatureDropDownBox",
  props: {
    value: {
      type: Number,
      default: null
    },
    onValueChanged: {
      type: Function,
      default: () => function () {
      }
    },
  },
  data() {
    return {
      dropDownBoxRefName,
      currentValue: this.value,
      dropDownOptions: {width: 500},
      dataSourceNomenclatures: data.nomenclatures,
      dataSourceDepartments: data.departments,
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

<style scoped>

</style>