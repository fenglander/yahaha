<template>
  <yhhText v-if="readonly" v-model="value" ></yhhText>
  <el-date-picker v-else v-model="value" :type="componentType" :disabled="readonly"
    :placeholder="config.placeholder" editable format="YYYY-MM-DD HH:mm:ss" />
</template>
  
<script setup lang="ts">
import { computed, toRefs } from 'vue';
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

const readonly = computed(() => {
  return props.widgetConfig.readonly
})


const componentType = computed(() => {

  return config.value.componentType
})


</script>
  