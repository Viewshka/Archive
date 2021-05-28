<template>
  <DxDropDownBox
      :ref="dropDownBoxRefName"
      :drop-down-options="dropDownOptions"
      :data-source="dataSourceDocuments"
      :value="currentValues"
      :display-expr="displayValue"
      value-expr="id"
      content-template="contentTemplate"
      placeholder="Выберите документы"
      :show-clear-button="true"
      :show-drop-down-button="true"
  >
    <template #contentTemplate="{}">
      <div>
        <DxTreeList
            :data-source="dataSourceDocuments"
            :allow-column-resizing="true"
            :focused-row-enabled="true"
            :render-async="true"
            :allow-column-reordering="true"
            :auto-expand-all="true"
            :selected-row-keys="currentValues"
            :focused-row-key="currentValues"
            :on-selection-changed="onSelectionChanged"
            :column-min-width="50"
            :root-value="null"
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

          <DxSearchPanel :visible="true" :width="550"/>
          <DxPaging :enabled="true" :page-size="20"/>
          <DxScrolling mode="virtual" row-rendering-mode="virtual" column-rendering-mode="virtual"/>
          <DxSelection :recursive="false" mode="multiple" show-check-boxes-mode="always" :allow-select-all="false"/>
        </DxTreeList>
      </div>
    </template>
  </DxDropDownBox>
</template>

<script>
import DxDropDownBox from "devextreme-vue/drop-down-box";
import DxTreeList, {
  DxColumn,
  DxLookup,
  DxPaging,
  DxScrolling,
  DxSearchPanel,
  DxSelection,
} from "devextreme-vue/tree-list";

import data from '../../data';

const dropDownBoxRefName = 'dropDownBoxRef';
export default {
  name: "DocumentsDropDownBox",
  props: {
    values: {
      type: Array,
      default: null
    },
    onValuesChanged: {
      type: Function,
      default: () => function () {
      }
    },
    dataSourceDocuments: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      dropDownBoxRefName,
      currentValues: this.values,
      dropDownOptions: {width: 600},
      dataSourceDocumentType: data.documentTypes,
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
    displayValue(data) {
      let type = this.dataSourceDocumentType.find(t => t.id === data.type).name;
      return `${data.name}(${type})`;
    },
    onSelectionChanged(selectionChangedArgs) {
      this.currentValues = selectionChangedArgs.selectedRowKeys;
      if (selectionChangedArgs.selectedRowKeys.length > 0) {
        this.onValuesChanged(this.currentValues);
      }
    },
  }
}
</script>

<style>

</style>