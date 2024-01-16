<template>
  <yhhText v-if="editDisabled" v-model="display"></yhhText>
  <component v-else :is="componentType" v-model="val"></component>
</template>
  
<script setup lang="ts">
import { inject, computed } from 'vue';
import {
  constFormProps,
} from '../../../utils/'
import yhhText  from './yhhText.vue'
const emit = defineEmits(['update:modelValue'])

const props = withDefaults(
  defineProps<{
    widgetConfig?: any,
    modelValue?: any,
  }>(),
  {
  }
)

const val = computed({
  get() {
    if (props.modelValue) {
      return props.modelValue;
    } else {
      return false
    }
  },
  set(newVal) {
    emit('update:modelValue', newVal);
  }
})

const display = computed(() => {
  if (val?.value) {
    return "是";
  } else {
    return "否";
  }
})

const config = computed(() => {
  return props.widgetConfig.config || {}
})


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

const formProps = inject(constFormProps, {}) as any

const modeType = computed(() => {
  return formProps.value.type
})


const componentType = computed(() => {
  return `el-${config.value.componentType}`
})


</script>
  