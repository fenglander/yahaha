<!-- Created by 337547038 on 2021/9/27. -->
<template>
  <template v-if="!itemInfo.invisible">
    <el-form-item v-bind="itemInfo.item" :prop="tProp || itemInfo.Name" :class="config.className"
      :label="getLabel(itemInfo.item as FormItem)">
      <template #label>
        <el-tooltip :disabled="isEmptyRoNull(config.help)" :content="config.help" placement="top">
          <el-text :style="{ color: textColor }" tag="b">{{ getLabel(itemInfo.item) }}</el-text>
        </el-tooltip>
      </template>
      <component :is="curWidget(itemInfo.type)" :widgetConfig="itemInfo" v-model="value" @blur="blurEvent" />
    </el-form-item>
  </template>
</template>

<script lang="ts" setup>
import {
  inject,
  onMounted,
  computed,
  onUnmounted,
  ref,
  watch,
} from 'vue'
import { FormItem, FormList } from '../../types'
import getWidget from './widgets/getWidget'
import {
  constControlChange,
  constFormProps,
  constblurEvent,
  isEmptyRoNull, jsonParseStringify
} from '../../utils/'

const props = withDefaults(
  defineProps<{
    data: FormList
    modelValue?: any // 子表和弹性布局时时有传
    tProp?: string // 子表时的form-item的prop值，用于子表校验用
    editIndex?: number // 子表当前活动行行
  }>(),
  {
  }
)

const emits = defineEmits<{
  (e: 'update:modelValue', val: any): void
  (e: 'update:data', val: any): void
}>()

const itemInfo = ref(props.data);


const formProps = inject(constFormProps, {}) as any
const type = computed(() => {
  return formProps.value.type
})

const config = computed(() => {
  return props.data.config || {}
})


// const setReadonly = (info: FormList) => {
//   let res: boolean = false;
//   info.origReadonly = info.origReadonly ? false : true;
//   if (type.value === 3) {
//     res = true // 查看模式，为不可编辑状态
//   }
//   else if ([1, 2].includes(type.value) && info.origReadonly) {
//     res = true // 编辑模式
//   }
//   else if (props.tProp) {
//     if (props.index == props.editIndex) {
//       res = false
//     } else {
//       res = true
//     }
//   }
//   return res;
// }

const changeEvent = inject(constControlChange, '') as any
const updateModel = (val: any) => {
  changeEvent &&
    changeEvent({
      key: props.data.Name,
      value: val,
      data: itemInfo,
      tProp: props.tProp
    })
}
const triggeredEvent = inject(constblurEvent, '') as any
const blurEvent = () => {
  triggeredEvent &&
    triggeredEvent(
      props.data.Name,
      props.data,
      props.tProp,
    )
}

const value = computed({
  get() {
    if (props.tProp) {
      // 表格和弹性布局
      return props.modelValue
    } else {
      return formProps.value.model[props.data.Name]
    }
  },
  set(newVal: any) {
    updateModel(newVal)
  }
})


const curWidget = (name: string) => {
  //写的时候，组件的起名一定要与dragList中的element名字一模一样，不然会映射不上
  return getWidget[name]
}

// 当通用修改属性功能添加新字段时，数组更新但toRefs没更新

const getLabel = (ele: FormItem | undefined) => {
  const showColon = formProps.value.showColon ? ':' : ''
  if (ele) {
    return ele.showLabel ? '' : ele.label + showColon
  } else {
    return ''
  }
}
const textColor = computed(() => {
  if ([1, 2].includes(type.value) && props.data.origRequired) {
    return 'var(--el-color-danger)'
  } else {
    return 'var(--el-text-color-primary)'
  }
})


// 从接口返回的dict会在这里触发
// watch(
//   () => formProps.value.dict,
//   (val: any) => {
//     setFormDict(val)
//   },
//   {
//     /*deep: true*/
//   }
// )


const setCurrStatus = (data: any) => {
  if (type.value === 3) {
    data.readonly = true // 查看模式，为不可编辑状态
  }
  else if ([1, 2].includes(type.value) && data.origReadonly) {
    data.readonly = true // 编辑模式
  }
  else if (props.tProp) {
    if (props.tProp.split('.')[1] == props.editIndex?.toString()) {
      data.readonly = false
    } else {
      data.readonly = true
    }
  }
  data.validateReq = data.origRequired && !value.value;
  return data;
}

watch(
  [() => props.data, () => props.editIndex, () => type.value, () => value.value],
  () => {
    if (type.value === 5) { return; } // 设计模式时不做判断
    let temp = jsonParseStringify(props.data); // 切断响应
    itemInfo.value = setCurrStatus(temp);
    //console.log('Index',props.tProp?.split('.')[1],'editIndex',props.editIndex?.toString(),'itemInfo',itemInfo.value.readonly,'temp',temp.readonly)
  },
  {
    deep: true, immediate: true
  }
);

// watch(
//   () => props.data,
//   () => {
//     if (type.value === 5) { return; } // 设计模式时不做判断
//     let temp = deepClone(props.data); 
//     itemInfo.value = temp;
//   },
//   {
//     deep: true, immediate: true
//   }
// );

// treeSelect
// const filterMethod = (val: string) => {
//   if (props.data.type === 'treeSelect') {
//     // 请求参数名，可使用config.queryName传进来
//     const queryName = config.value.queryName || 'name'
//     control.value.filterMethod && control.value.filterMethod(val)
//     getAxiosOptions({ [queryName]: val })
//   }
// }
onMounted(() => {
})
onUnmounted(() => { })
</script>
<style scoped>
/* .form-item-lable {
  color: var(--el-text-color-primary);
} */
</style>