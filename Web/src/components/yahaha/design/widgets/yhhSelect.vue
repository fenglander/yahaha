<template>
  <yhhText v-if="readonly" v-model="display"></yhhText>
  <el-select v-model="value" v-else clearable filterable :placeholder="config.placeholder">
    <el-option v-for="item in opations" :key="item.value" :label="item.description" :value="item.value" />
  </el-select>
</template>

<script setup lang="ts">
import { computed, toRefs } from 'vue';
import {
  formatNumber
} from '/@/components/yahaha/design/utils'
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
    return formatNumber(modelValue?.value)
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
  return JSON.parse(props.widgetConfig.EnumValue)

})

const readonly = computed(() => {
  return props.widgetConfig.readonly
})

const display = computed(() => {
  if (value?.value) {
    return opations.value.find((t: any) => t.value === value.value).description;
  } else {
    return null;
  }
})

// const formProps = inject(constFormProps, {}) as any


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