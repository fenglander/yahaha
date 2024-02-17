<template>
  <yhhText v-if="widgetConfig.readonly" v-model="val" ></yhhText>
  <el-input v-model="val" :class="{ 'validateReqFailed': widgetConfig.validateReq }" v-else :placeholder="config.placeholder" :type="inputType" :autosize="inputAutosize"
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
import { computed } from 'vue';
import { FormList } from '/@/components/yahaha/design/types'
import yhhText  from './yhhText.vue'
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
<style lang="scss" scoped>


.validateReqFailed {
  :deep(.el-input__wrapper) {
    background-color: var(--el-color-danger-light-3);
  }
}
</style>
  