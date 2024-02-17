<template>
  <yhhText v-if="readonly" v-model="display"></yhhText>
  <component v-else :is="componentType" v-model="val"></component>
</template>
  
<script setup lang="ts">
import { computed } from 'vue';
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


const readonly = computed(() => {
  return props.widgetConfig.readonly
})

const componentType = computed(() => {
  return `el-${config.value.componentType}`
})


</script>
  