<template>
  <DxPopup
      height="auto"
      :width="728"
      position="center"
      title="Управление ролями"
      :show-title="true"
      :resize-enabled="true"
      :visible="visible"
      :close-on-outside-click="true"
      @hidden="cancel"
  >
    <div>
      <DxDataGrid
          :data-source="dataSource"
      >
        <DxColumn
            data-field="userName"
            caption="Наименовнаие"
        />
        <DxColumn
            data-field="role"
            caption="Роль"
        />
        <DxEditing
            :allow-updating="true"
            :allow-deleting="true"
            :allow-adding="true"
            mode="row"
            :use-icons="true"
        />
      </DxDataGrid>
    </div>
  </DxPopup>
</template>

<script>
import {DxPopup} from "devextreme-vue/popup";
import DxDataGrid, {DxColumn, DxEditing, DxLookup, DxValidationRule} from "devextreme-vue/data-grid";
import * as AspNetData from "devextreme-aspnet-data-nojquery";

export default {
  name: "UserRoleForm",
  components: {
    DxPopup,
    DxDataGrid,
    DxEditing,
    DxColumn,
    DxValidationRule,
    DxLookup
  },
  props: {
    visible: {
      type: Boolean,
      required: true
    },
  },
  data: function () {
    return {
      popupVisible: false,
      formRefName: 'form',
      dataSource:[
        {
          userName:"Courier",
          role: "Курьер"
        },
        {
          userName:"Client",
          role: "Клиент"
        },
      ],
    }
  },

  methods: {
    cancel: function () {
      this.$emit('update:visible', false);
    },
  },
}
</script>

<style scoped>

</style>