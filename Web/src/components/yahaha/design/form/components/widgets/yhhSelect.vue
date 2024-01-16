<template>
  <yhhText v-if="editDisabled" v-model="value" ></yhhText>
    <el-select v-model="value"  v-else clearable filterable :placeholder="config.placeholder">
    <el-option
      v-for="item in opations"
      :key="item.value"
      :label="item.description"
      :value="item.value"
    />
  </el-select>
</template>
  
<script setup lang="ts">
import { inject, computed, toRefs } from 'vue';
import {
  constFormProps,formatNumber
} from '../../../utils/'
import yhhText  from './yhhText.vue'
const emit = defineEmits(['update:modelValue'])

const props = withDefaults(
  defineProps<{
    widgetConfig: any,
    modelValue?: any,
  }>(),
  {
  }
)

const { modelValue } = toRefs(props);

const value = computed({
  get() {
    if (modelValue.value) {
      // 表格和弹性布局
      return formatNumber(modelValue?.value)
    } else {
      return undefined
    }
  },
  set(newVal: any) {
    emit('update:modelValue', newVal);
  }
})

const config = computed(() => {
  return props.widgetConfig.config || {}
})

const opations = computed(() => {
    // 把字典从字符串转为对象
    return  JSON.parse(props.widgetConfig.EnumValue) 

})

const editDisabled = computed(() => {
  if (modeType.value === 3) {
    return true // 查看模式，为不可编辑状态
  }
  if (modeType.value === 1 && config.value.addDisabled) {
    return true
  }
  if (modeType.value === 2 && config.value.editDisabled) {
    return true // 编辑模式
  }
  return false
})

const formProps = inject(constFormProps, {}) as any

const modeType = computed(() => {
  return formProps.value.type
})

// const inputPrecision = computed(() => {
//   console.log(props.widgetConfig)
//   const inttype = ['Int32','Int64']
//   if(inttype.includes(props.widgetConfig.tType)){
//     return 0
//   }else{
//     return 99
//   }
// })

</script>
  
<style lang="scss" scoped>
  .el-input-number .el-input__inner {
    text-align: left;
  }
</style>