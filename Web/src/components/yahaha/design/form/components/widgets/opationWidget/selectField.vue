<template>
  <el-table size="small" :data="fieldList" row-key="Id" ref="fieldTable">
    <el-table-column width="25" type="expand">
      <template #default="scope">
        <el-form>
          <el-form-item label="宽度">
            <el-input-number v-model="scope.row.width" :step="1" step-strictly
              @change="handleSelect(scope.row.width, $event)" />
          </el-form-item>
          <el-form-item label="最小宽度">
            <el-input-number v-model="scope.row.minWdith" :step="1" step-strictly
              @change="handleSelect(scope.row.minWdith, $event)" />
          </el-form-item>
          <el-form-item label="固定列">
            <el-select v-model="scope.row.fixed" clearable placeholder="please select"
              @clear="handleSelect(scope.row.fixed, null)" @change="handleSelect(scope.row.fixed, $event)">
              <el-option label="left" value="left" />
              <el-option label="right" value="right" />
            </el-select>
          </el-form-item>
          <el-form-item label="排序">
            <el-switch v-model="scope.row.sortable" @change="handleSelect(scope.row.sortable, $event)" />
          </el-form-item>
          <el-form-item lable="帮助">
            <el-input v-model="scope.row.help" @change="handleSelect(scope.row.help, $event)" />
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column width="28" header-align="center" type="index" align="center">
      <template #default="">
        <el-icon class="move-icon cursor-pointer"><ele-Rank /></el-icon>
      </template>
    </el-table-column>
    <el-table-column width="28" header-align="center" type="index" align="center">
      <template #header>
        <el-checkbox v-model="selAll" :indeterminate="isIndeterminate" size="small" />
      </template>
      <template #default="scope">
        <el-checkbox v-model="scope.row.sel" size="small" @change="handleSelect(scope.row.sel, $event)" />
      </template>
    </el-table-column>
    <el-table-column prop="label" label="字段" />
  </el-table>
</template>
  
<script lang="ts" setup>
import Sortable from 'sortablejs'
import { computed, nextTick, onMounted, ref } from 'vue'
const props = withDefaults(
  defineProps<{
    modelValue: any
    dict: any
  }>(),
  {
    modelValue: () => {
      return []
    },
  }
)
const fieldTable = ref();
const fieldList = ref();

const setFieldList = (() => {
  const dict = props.dict;
  const value = props.modelValue;
  dict.forEach((item: any) => {
    const filteredItems = value.filter((item2: any) => item.fieldName === item2.fieldName);
    item.sel = filteredItems.length > 0;
    item.label = item.Description + '[' + item.fieldName + ']';
    item.type = filteredItems.length > 0 ? filteredItems[0].type : item.type;
    item.width = filteredItems.length > 0 ? filteredItems[0].width : 0;
    item.minWdith = filteredItems.length > 0 ? filteredItems[0].minWdith : 0;
    item.fixed = filteredItems.length > 0 ? filteredItems[0].fixed : null;
    item.sortable = filteredItems.length > 0 ? filteredItems[0].sortable : false;
    item.help = filteredItems.length > 0 ? filteredItems[0].help : null;
  });
  fieldList.value = dict
});

setFieldList();

const selAll = computed({
  get() {
    return selCount.value === fieldList.value.length;
  },
  set(val) {
    fieldList.value.forEach((item: any) => {
      item.sel = val;
    });
    clickChild();
  },
});

const initSortable = () => {
  const el1 = fieldTable.value.$el.querySelector('.el-table__body tbody')
  Sortable.create(el1, {
    handle: '.move-icon',
    onEnd: (evt: any) => {
      const oldIndex = evt.oldIndex;  // element's old index within old parent
      const newIndex = evt.newIndex;  // element's new index within new parent
      const arr = fieldList.value
      const currRow = arr.splice(oldIndex, 1)[0]
      arr.splice(newIndex, 0, currRow)
      fieldList.value = []
      nextTick(() => {
        fieldList.value = arr
        clickChild();
      })
    }
  })
}

const handleSelect = (path: any, value?: boolean | null) => {
  path = value;
  clickChild()
}

const emit = defineEmits(['change'])
const clickChild = () => {
  const filteredList = fieldList.value.filter((item: { sel: boolean }) => item.sel);
  emit('change', filteredList)
}

const selCount = computed(() => {
  // 定义条件
  const condition = (element: { sel: boolean }) => element.sel === true;
  // 使用 filter 方法筛选出符合条件的元素
  return fieldList.value.filter(condition).length;

})

const isIndeterminate = computed(() => {

  if (0 < selCount.value && selCount.value < fieldList.value.length) {
    return true
  } else {
    return false
  }
})


onMounted(() => {
  nextTick(() => {
    initSortable();
  })
})

</script>
<style scoped>
.buttons {
  margin-top: 35px;
}

.el-checkbox-width {
  width: 30px;
}</style>