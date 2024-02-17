<!-- Created by 337547038 on 2021/9/29. -->
<template>
  <el-table v-bind="config.control" :data="childData" @row-click="rowclickEvent">
    <el-table-column template v-for="item in config.child" :prop="item.Name" :key="item.Id" :width="item.width"
      :minWidth="item.minWidth" :fixed="emptyToNull(item.fixed)" :sortable="item.sortable" align="center"
      header-align="center">
      <template #header>
        <el-tooltip :disabled="isEmptyRoNull(item.Help)" :content="item.Help" placement="top">
          <el-text :class="getDynamicClass(item)" @click.stop="groupClick(item)" tag="b">{{ item.Description }}</el-text>
        </el-tooltip>
      </template>
      <template #default="scope">
        <component :is="curWidget(item.curWidget)" @blur="blurEvent" :widgetConfig="setCurrStatus(item, scope)" v-model="scope.row[item.Name]"
           />
      </template>
    </el-table-column>
    <el-table-column v-if="!readonly" prop="action" label="操作" fixed="right">
      <template #default="scope">
        <el-button link type="primary" @click="delRow(scope.$index)">删除
        </el-button>
      </template>
    </el-table-column>
  </el-table>
  <el-button v-if="!readonly" class="mt-4" style="width: 100%" @click="addRow">Add Item</el-button>
</template>

<script setup lang="ts">
//import FormItem from '../formItem.vue'
import getWidget from '../widgets/getWidget'
import { inject, computed, ref, watch, } from 'vue'
import { emptyToNull, isEmptyRoNull, constFormProps, deepClone } from '/@/components/yahaha/design/utils'
import { debounce } from 'lodash-es';
import { useDesignFormStore } from '/@/stores/designForm'
import { useSysModel } from '/@/stores/sysModel';
import md5 from 'md5'
import { FormList } from '/@/components/yahaha/design/types'
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
  (e: 'update:widgetConfig', val: any): void
  (e: 'blur', val: any): void // 表单组件值发生变化时
}>()

const blurEvent = (item: any, event: any) => {
  // console.log('item',item,'event',event)
  TrigRelateFieldVals()
  emits('blur', event);
}

const store = useDesignFormStore() as any
let updatingModelValue = false;
const formProps = inject(constFormProps, {}) as any

//const config = ref(props.widgetConfig)
const curWidget = (name: string) => {
  //写的时候，组件的起名一定要与dragList中的element名字一模一样，不然会映射不上
  return getWidget[name]
}

const config = computed({
  get() {
    return props.widgetConfig;
    // if (modeType.value === 5) {
    //   return props.widgetConfig;
    // } else {
    //   return deepClone(props.widgetConfig);
    // }
  },
  set(newVal: any) {
    if (modeType.value === 5) {
      emits('update:widgetConfig', newVal)
    }
  }
}) as any

const editIndex = ref(0);

const rowclickEvent = (row: any) => {
  editIndex.value = childData.value.findIndex((it: any) => it.index === row.index);
}
const childData = ref<any[]>([]);


const modeType = computed(() => {
  return formProps.value.type
})
// 如果编辑页禁用时，则返回true
const readonly = computed(() => {
  return props.widgetConfig.readonly
})


const setCurrStatus = (item: any, scope: any) => {
  let temp = item;
  const isCur = scope.$index === editIndex.value;
  if (modeType.value !== 5) {
    temp = deepClone(item);
  }
  if (modeType.value === 3) {
    temp.readonly = true; // 查看模式，为不可编辑状态
  } else if ([1, 2].includes(modeType.value) && (readonly.value || item.origReadonly)) {
    temp.readonly = true; // 编辑模式但只读
  } else if (isCur || modeType.value === 5) {// 是否当前行
    temp.readonly = false;
  } else {
    temp.readonly = true;
  }
  const value = scope.row[temp.Name];
  temp.validateReq = item.origRequired && !value;
  // if(isCur){
  //   console.log(temp)
  // }
  return temp
}

const getGroupName = (item: any) => {
  if (item.key) {
    return item.key
  } else {
    return md5(JSON.stringify(item))
  }
}

const groupClick = (item: any) => {
  // 设计模式下才执行
  if (modeType.value !== 5) {
    return
  }
  // 更新字段

  store.setActiveKey(getGroupName(item))
  store.setControlAttr(item)
}

const TrigRelateFieldVals = () => {
  const relateFieldList = useSysModel().getRelateFieldList(config.value.RelModel.Id);
  const rowData = childData.value[editIndex.value];
  if (relateFieldList.length > 0) {
    relateFieldList.forEach((it: any) => {
      let relValue = rowData;
      for (const prop of it.Related.split('.')) {
        if (relValue && relValue.hasOwnProperty(prop)) {
          relValue = relValue[prop];
        } else {
          // 属性不存在时可以选择处理错误或提供默认值
          relValue = null;
          break;
        }
      }
      rowData[it.Name] = relValue;
    });
  }
}

const getEmptyData = () => {
  let data: any[] = [];
  const result: { [key: string]: null } = {};
  if (config.value.SubFields) {
    config.value.SubFields.forEach((item: any) => {
      result[item.Name] = null;
    });
  }
  data.push(result);
  return data
}

const delRow = (index: number) => {
  childData.value.splice(index, 1)
}

const addRow = () => {
  if (config.value.child.length > 0) {
    const temp: any = {}
    config.value.child.forEach((item: any) => {
      if (item.Name) {
        temp[item.Name] = item.control.modelValue
      }
    })

    childData.value.push({ ...temp, index: childData.value.length })
  }
}

const getDynamicClass = (item: FormList) => {
  // 在这里根据条件动态返回class
  return {
    'design-lable': modeType.value === 5,
    'required-lable': [1, 2].includes(modeType.value) && item.origRequired,
  };
}

const updateModelValue = debounce(
  async function (newVal) {
    if (!updatingModelValue) { // 如果不是在更新 childData 的过程中
      updatingModelValue = true; // 设置标志位，表示开始更新 modelValue
      await emits('update:modelValue', newVal);
      updatingModelValue = false; // 更新完成，重置标志位
    }
  },
  1000
);

// 监听双向绑定值改变，用于回显
watch(
  () => props.modelValue,
  () => {
    if (!updatingModelValue) { // 如果不是在更新 modelValue 的过程中
      const hasValue = props.modelValue && props.modelValue.length > 0;
      let temp;
      if (hasValue) {
        temp = props.modelValue;
      } else {
        temp = getEmptyData();
      }
      const tempAddIndex = temp.map((item: any, index: any) => ({ ...item, index }));
      childData.value = tempAddIndex;
    }
  },
  {
    deep: true, immediate: true
  }
);

watch(
  () => childData.value,
  async (newVal) => {
    await updateModelValue(newVal);
  },
  {
    deep: true
  }
);

</script>
<style scoped lang="scss">
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

.required-lable {
  color: var(--el-color-danger);
}

.design-lable {
  text-decoration: underline;
}
</style>