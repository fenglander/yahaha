<template>
  <yhhText v-if="editDisabled" v-model="value" ></yhhText>
  <el-date-picker v-else v-model="value" :type="componentType" :disabled="editDisabled"
    :placeholder="config.placeholder" editable format="YYYY-MM-DD HH:mm:ss" />
</template>
  
<script setup lang="ts">
import { inject, computed, toRefs } from 'vue';
import {
  constFormProps,
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
      return modelValue?.value
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


const componentType = computed(() => {

  return config.value.componentType
})


</script>
  