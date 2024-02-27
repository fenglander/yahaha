<template>
  <el-button ref="selButRef" type="primary" icon="ele-Tickets" circle v-click-outside="onClickOutside" @click="onClick"/>

  <el-popover popper-class="yhh-popover" :width="341" :visible="popoverVisible" ref="selPopoverRef" :virtual-ref="selButRef" trigger="click"
    :fallback-placements="['bottom', 'top', 'right']" virtual-triggering>
    <el-table ref="selTableRef" :data="fields" :row-key="getRowKey" @select="selectAction"
      @select-all="selectAllAction">
      <el-table-column type="selection" width="45" />
      <el-table-column width="100" property="Description" label="描述" />
      <el-table-column width="100" property="Name" label="名称" />
      <el-table-column width="70" label="固定列" align="center">
        <template v-slot="scope">
          <el-select v-model="scope.row.fixed" placeholder=" " clearable @change="fixedChange(scope.row, $event)">
            <el-option label="左" value="left" />
            <el-option label="右" value="right" />
          </el-select>
        </template>
      </el-table-column>
    </el-table>
  </el-popover>
</template>
  
<script setup lang="ts">
import { computed, nextTick, ref, unref } from 'vue';
import { ClickOutside as vClickOutside } from 'element-plus'
const emit = defineEmits(['update:modelValue'])

const props = withDefaults(
  defineProps<{
    fields: any[],
    modelValue?: any,
  }>(),
  {
  }
)
const selTableRef = ref();
const selPopoverRef = ref();
const selButRef = ref();
const popoverVisible = ref(false);
const selectedFields = computed<any[]>({
  get() {
    if (props.modelValue) {
      toggleSelection();
      return props.modelValue
    } else {
      return []
    }
  },
  set(newVal: any) {
    emit('update:modelValue', newVal)
  }
})

const toggleSelection = () => {
  nextTick(() => {
    selectedFields.value.forEach((item: any) => {
      const rowToSelect = props.fields.find((row: any) => row.Name === item.Name);
      if (rowToSelect) {
        rowToSelect.fixed = item.fixed;
        selTableRef.value!.toggleRowSelection(rowToSelect, true);
      }
    });
  });
}

const getRowKey = (row:any) =>{
  return row.Id
}

const onClickOutside = () => {
  unref(selPopoverRef).popperRef?.delayHide?.();
}

const onClick = () => {
  popoverVisible.value = true;
  toggleSelection();
}

const fixedChange = (row: any, val: any) => {
  row.fixed = ["left", "right"].includes(val) ? val : null;
  //console.log(row)
  let temp = selectedFields.value;
  const field = temp.find((item: any) => row.Name === item.Name);
  field.fixed = row.fixed;
  temp = fixedOrder(temp);
  selectedFields.value = temp;
}

const fixedOrder = (data: any[]) => {
  // 找到所有带有 fixed 属性的对象
  const fixedItems = data.filter(item => item.fixed);
  // 从原数组中移除所有带有 fixed 属性的对象
  data = data.filter(item => !item.fixed);
  // 将带有 fixed 属性的对象按照 "left" 和 "right" 分别添加到数组的开头和末尾
  fixedItems.forEach(item => {
    if (item.fixed === "left") {
      data.unshift(item);
    } else if (item.fixed === "right") {
      data.push(item);
    }
  });
  return data;
}

const selectAction = async (selection: any, row?: any) => {
  //console.log('selection', selection, 'row', row, 'selected', selectedFields.value);
  let temp = selectedFields.value;
  //是否单选
  let selected = selection.length && selection.findIndex((it: any) => it.Name === row.Name) !== -1; // ID是会变的
  if (selected) {
    temp.push(row);
  } else {
    const indexToRemove: number = temp.findIndex((it: any) => it.Name === row.Name);
    if (indexToRemove !== -1) {
      // 使用 splice 删除元素
      temp.splice(indexToRemove, 1);
    }
  }

  temp = fixedOrder(temp);
  selectedFields.value = temp;
}

const selectAllAction = (selection: any) => {
  let temp: any[] = [];
  if (selection.length) {
    selection.forEach((it: any) => {
      temp.push(it);
    })
  }
  temp = fixedOrder(temp);
  selectedFields.value = temp;
}


</script>
<style lang="scss">
.yhh-popover{
  height: 500px; 
  overflow: auto;
}
</style>