<template>
  <yhhText v-if="readonly" v-model="display"></yhhText>
  <el-input-number :class="{ 'validateReqFailed': validateReq, 'yhh-input-number': true }" v-model="value" v-else
    :placeholder="config.placeholder" :controls="false" :precision="inputPrecision" />
</template>
  
<script setup lang="ts">
import { computed, toRefs, } from 'vue';
import { formatNumber } from '/@/components/yahaha/design/utils'
import yhhText from './yhhText.vue'
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

const readonly = computed(() => {
  return props.widgetConfig.readonly
})

const validateReq = computed(() => {
  return props.widgetConfig.validateReq
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
.yhh-input-number {
  width: 100%;
  :deep(.el-input__inner) {
    text-align: left;
  }
  
}


.validateReqFailed {
  :deep(.el-input__wrapper) {
    background-color: var(--el-color-danger-light-3);
  }
}
</style>