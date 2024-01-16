<template>
  <yhhText v-if="editDisabled" v-model="display" ></yhhText>
  <el-select v-model="value" v-else clearable filterable remote :remote-method="remoteMethod" :loading="loading">
    <el-option v-for="item in options" :key="item.id" :label="item.description" :value="item.id" />
  </el-select>
</template>

<script setup lang="ts">
import { inject, computed, toRefs, ref, watch } from 'vue';
import {
  constFormProps, formatNumber, deepClone
} from '../../../utils/'
import yhhText  from './yhhText.vue'
import { selRelObjectQuery } from '/@/api/widget';
import { useSysModel } from '/@/stores/sysModel';
const emit = defineEmits(['update:modelValue'])

const props = withDefaults(
  defineProps<{
    widgetConfig: any,
    modelValue?: any,
  }>(),
  {
  }
)
const loading = ref(false)
const { modelValue } = toRefs(props);
const options = ref<any[]>([])
const value = computed({
  get() {
    if (modelValue.value) {
      // 表格和弹性布局
      return formatNumber(modelValue?.value.Id)
    } else {
      return undefined
    }
  },
  set(newVal: any) {
    const result = options.value.find((item: any) => item.id === newVal);
    emit('update:modelValue', result.value);
  }
})

const config = computed(() => {
  return props.widgetConfig.config || {}
})

const lableName = computed(() => {
  const lables = useSysModel().getSysModelLables(props.widgetConfig.RelModel.TableName);
  return lables[lables.length - 1]
})

const display = computed(() => {
  if (modelValue?.value) {
    return modelValue?.value[lableName.value];
  } else {
    return null;
  }
})

const remoteMethod = async (query: string) => {
  loading.value = true
  const params = {
    relModel: props.widgetConfig.RelModelName,
  }
  const res = await selRelObjectQuery(params);
  const temp = res.data.result.filter((it: any) => it[lableName.value].indexOf(query) > -1).map((it: any) => ({
    value: it,
    id: it.Id,
    description: it[lableName.value]
  }))
  options.value = deepClone(temp);
  loading.value = false
}

const init = () => {
  if (modelValue.value && options.value.length === 0) {
    const temp = {
      value: modelValue?.value,
      id: modelValue?.value.Id,
      description: modelValue?.value[lableName.value]
    }
    options.value.push(temp)
  }
}

watch(
  () => props.modelValue,
  () => {
    init();
  },
  {
    deep: true, immediate: true
  }
);


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

</script>

<style lang="scss" scoped>
.el-input-number .el-input__inner {
  text-align: left;
}
</style>