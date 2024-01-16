<template>
  <yhhText v-if="disabled" v-model="display" ></yhhText>
  <el-input-number v-model="value" v-else :placeholder="config.placeholder" :controls="false"
    :precision="inputPrecision">

  </el-input-number>
</template>
  
<script setup lang="ts">
import { inject, computed, toRefs, } from 'vue';
import {
  constFormProps, formatNumber
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
      return 0
    }
  },
  set(newVal: any) {
    emit('update:modelValue', newVal);
  }
})

const display = computed(() => {
  if (value?.value) {
    return value?.value;
  } else {
    return null;
  }
})

const config = computed(() => {
  return props.widgetConfig.config || {}
})

const disabled = computed(() => {
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

const inputPrecision = computed(() => {
  const inttype = ['Int32', 'Int64']
  if (inttype.includes(props.widgetConfig.tType)) {
    return 0
  } else if (config.value.DecimalDigits) {
    return config.value.DecimalDigits
  } else if (config.value.precision) {
    return config.value.precision
  } else {
    return 7
  }
})

</script>
  
<style lang="scss" scoped>
.el-input-number .el-input__inner {
  text-align: left;
}
</style>