<template>
  <DxPopup
      :max-height="800"
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
      <DxList
          :data-source="dataSource"
          :scrolling-enabled="true"
          :scroll-by-content="true"
          :scroll-by-thumb="true"
          show-scrollbar="always"
          :height="700"
      >
        <template #item="{ data: item }">
          <div>
            <span><b>Выдал:</b> {{ item.giver }} <br></span>
            <span v-if="item.dateOfGiveOut"><b>Дата выдачи:</b> {{ $moment(item.dateOfGiveOut).locale('ru').format('L') }}<br></span>
            <span v-else><b>Дата выдачи:</b> - <br></span>
            <span> <b>Получатель:</b> {{ item.recipient }}<br></span>
            <span v-if="item.dateOfReturn"><b>Дата возврата:</b> {{ $moment(item.dateOfReturn).locale('ru').format('L') }}<br></span>
            <span v-else><b>Дата возврата:</b> - <br></span>
            <span><b>Характер использования:</b> {{item.usageType}}<br></span>
            <span><b>Статус:</b> {{item.status}}</span>
          </div>
        </template>
      </DxList>
    </div>

  </DxPopup>
</template>

<script>
import DxPopup from "devextreme-vue/popup";
import DxList from "devextreme-vue/list";

export default {
  name: "DocumentHistoryForm",
  props: {
    visible: {
      type: Boolean,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    dataSource: {
      type: Array,
      required: true
    }
  },
  data() {
    return {}
  },
  created() {

  },
  components: {
    DxPopup,
    DxList
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