<template>
    <yhhText v-model="val" ></yhhText>
  </template>
    
  <script setup lang="ts">
  import { computed } from 'vue';
  import { FormList } from '/@/components/yahaha/design/types'
  import yhhText  from './yhhText.vue'
  
  const props = withDefaults(
    defineProps<{
      widgetConfig: FormList | any,
      modelValue?: any,
    }>(),
    {
    }
  )
  
  const val = computed(() => {
    const count = props.modelValue ? props.modelValue.length.toString() : '0';
    return count + '记录'
  })
  
  
  
  </script>
  <style lang="scss" scoped>
  
  
  </style>
    