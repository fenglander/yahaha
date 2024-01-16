<template>
  <yhhText v-if="editDisabled" v-model="val" ></yhhText>
  <el-input v-model="val" v-else :placeholder="config.placeholder" :type="inputType" :autosize="inputAutosize"
    :show-word-limit="inputShowWordLimit">
    <template #prepend v-if="config.prepend">
      {{ config.prepend }}
    </template>
    <template #append v-if="config.append">
      {{ config.append }}
    </template>
  </el-input>
</template>
  
<script setup lang="ts">
import { inject, computed } from 'vue';
import { FormList } from '../../../types'
import yhhText  from './yhhText.vue'
import {
  constFormProps,
} from '../../../utils/'

const emit = defineEmits(['update:modelValue'])

const props = withDefaults(
  defineProps<{
    widgetConfig: FormList | any,
    modelValue?: any,
  }>(),
  {
  }
)

const val = computed({
  get() {
    return props.modelValue;
  }, set(newVal) {
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
  if (modeType.value === 1 && props.widgetConfig.readonly) {
    return true
  }
  if (modeType.value === 2 && props.widgetConfig.readonly) {
    return true // 编辑模式
  }
  return false
})

const formProps = inject(constFormProps, {}) as any

const modeType = computed(() => {
  return formProps.value.type
})

const inputAutosize = computed(() => {
  let res = {}
  if (config.value.minRows) {
    res = Object.assign(res, { minRows: config.value.minRows })
  }
  if (config.value.maxRows) {
    res = Object.assign(res, { maxRows: config.value.maxRows })
  }
  return res
})

const inputType = computed(() => {
  let res: string = "text"
  if (config.value.showPassword) {
    res = "password"
  } else if (config.value.textarea) {
    res = "textarea"
  }
  return res
})

const inputShowWordLimit = computed(() => {
  const type = ["textarea", "text", ""]
  if (config.value.showWordLimit && type.includes(inputType.value)) {
    return true
  }
  return false
})


</script>
  