<template>
  <DxDropDownBox
      :ref="dropDownBoxRefName"
      :drop-down-options="dropDownOptions"
      :data-source="dataSourceDocuments"
      :value.sync="currentValue"
      display-expr="subject"
      value-expr="id"
      content-template="contentTemplate"
  >
    <template #contentTemplate="{}">
      <div>
        <DxTreeList
            :data-source="dataSourceDocuments"
            :allow-column-resizing="true"
            :focused-row-enabled="true"
            :render-async="true"
            :auto-expand-all="true"
            :selected-row-keys="[currentValue]"
            :focused-row-key="currentValue"
            :on-selection-changed="onSelectionChanged"
            :column-min-width="50"
            items-expr="structure"
            data-structure="tree"
            value-expr="id"
            key-expr="id"
        >
          <DxColumn data-field="subject" caption="Документ"/>

          <DxSearchPanel :visible="true" :width="450"/>
          <DxPaging :enabled="true" :page-size="20"/>
          <DxScrolling mode="virtual" row-rendering-mode="virtual" column-rendering-mode="virtual"/>
          <DxSelection mode="single"/>
        </DxTreeList>
      </div>
    </template>
  </DxDropDownBox>
</template>

<script>
import data from "../../data";
import DxDropDownBox from "devextreme-vue/drop-down-box";
import DxTreeList, {
  DxColumn,
  DxLookup,
  DxPaging,
  DxScrolling,
  DxSearchPanel,
  DxSelection
} from "devextreme-vue/tree-list";

const dropDownBoxRefName = 'dropDownBoxRef';
export default {
  name: "DocumentDropDownBox",
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
      dataSourceDocuments: data.documents,
    }
  },
  components: {
    DxDropDownBox,
    DxTreeList,
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