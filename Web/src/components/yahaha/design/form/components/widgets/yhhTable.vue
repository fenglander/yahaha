<!-- Created by 337547038 on 2021/9/29. -->
<template>
  <el-table v-bind="widgetConfig.control" :data="listData">
    <el-table-column template v-for="(item) in config.list" :prop="item.name" :key="item.Id" :width="item.width"
      :minWidth="item.minWidth" :fixed="emptyToNull(item.fixed)" :sortable="item.sortable" align="center" header-align="center">
      <template #header>
        <el-tooltip :disabled="isEmptyRoNull(item.Help)" :content="item.Help" placement="top">
          <span @click.stop="groupClick(item)">{{ item.Description }}</span>
        </el-tooltip>
      </template>
      <template #default="scope">
        <form-item :data="item" :tProp="`${widgetConfig.name}.${scope.$index}.${item.name}`"
          v-model="scope.row[item.name]" />
      </template>
    </el-table-column>
    <el-table-column v-if="!editDisabled" prop="action" label="操作" fixed="right">
      <template #default="scope">
        <el-button link type="primary" @click="delColumn(scope.$index)">删除
        </el-button>
      </template>
    </el-table-column>
  </el-table>
  <el-button v-if="!editDisabled" class="mt-4" style="width: 100%" @click="addColumn">Add Item</el-button>
</template>

<script setup lang="ts">
import FormItem from '../formItem.vue'
import { inject, computed, ref, watch } from 'vue'
import { emptyToNull, isEmptyRoNull, constFormProps } from '../../../utils'
import { useDesignFormStore } from '../../../store/designForm'
import md5 from 'md5'
const props = withDefaults(
  defineProps<{
    widgetConfig: any,
    modelValue?: any,
  }>(),
  {
  }
)
const emits = defineEmits<{
  (e: 'update:modelValue', val: any): void
}>()

const store = useDesignFormStore() as any

const formProps = inject(constFormProps, {}) as any
const widgetConfig = computed(() => {
  return props.widgetConfig;
}) as any
const config = computed(() => {
  return props.widgetConfig.config;
}) as any
//const tableDataNew: any = toRef(props.data, 'tableData')
//const tableDataNew: any = toRef(formProps.value.model, props.data.name)

const listData = ref<any[]>([]);

const modeType = computed(() => {
  return formProps.value.type
})
// 如果编辑页禁用时，则返回true
const editDisabled = computed(() => {
  if (modeType.value === 3) {
    return true // 查看模式，为不可编辑状态
  }
  if (modeType.value === 1 && config.value.readonly) {
    return true
  }
  if (modeType.value === 2 && config.value.readonly) {
    return true // 编辑模式
  }
  return false
})

const getGroupName = (item: any) => {
  if (item.key) {
    return item.key
  } else {
    return md5(JSON.stringify(item))
  }
}

const groupClick = (item: any, ele?: string) => {
  // 设计模式下才执行
  if (modeType.value !== 5) {
    return
  }
  if (ele) {
    item.type = ele
  }
  // 更新字段

  store.setActiveKey(getGroupName(item))
  store.setControlAttr(item)
}

const getlistData = () => {
  const hasValue = props.modelValue && props.modelValue.length > 0
  let data: any[] = [];
  if (!hasValue) {
    const result: { [key: string]: null } = {};
    if (widgetConfig.value.SubFields) {
      widgetConfig.value.SubFields.forEach((item: any) => {

        result[item.name] = null;
      });
    }
    data.push(result);
  } else {
    data = props.modelValue
  }
  listData.value = data
}

const delColumn = (index: number) => {
  listData.value.splice(index, 1)
}

const addColumn = () => {
  if (config.value.list.length > 0) {
    const temp: any = {}
    config.value.list.forEach((item: any) => {
      if (item.name) {
        temp[item.name] = item.control.modelValue
      }
    })
    listData.value.push(temp)
  }
}

// 监听双向绑定值改变，用于回显
watch(
  () => props.modelValue,
  () => {
    getlistData();
  },
  {
    deep: true, immediate: true
  }
);

watch(
  () => listData.value,
  (newVal) => {
    emits('update:modelValue', newVal);
  },
  {
    deep: true, immediate: true
  }
);

</script>
<style  lang="scss">
$border-color: #3498db;
$hover-border-color: darken($border-color, 10%); // 鼠标悬停时的颜色

.yhh-table-design {
  width: 200px;
  height: 100px;
  border: 1px dashed $border-color;
  transition: border 0.3s;

  &:hover {
    border: 2px solid $hover-border-color;
    font-weight: bold;
  }
}
</style>