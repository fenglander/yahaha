<template>
    <el-select-v2 v-model="value" style="width : 100%;" filterable remote :remote-method="remoteMethod" clearable 
        :options="options" :loading="loading" placeholder="输入模型名称" />
</template>
  
<script setup lang="ts">
import { ref, toRefs, computed } from 'vue';
import { useSysModel } from '/@/stores/sysModel';
import { formatNumber } from '/@/components/yahaha/design/utils/'
const emit = defineEmits(['update:modelValue'])
const loading = ref(false);
const props = withDefaults(
  defineProps<{
    modelValue?: any,
  }>(),
  {
  }
)
const {modelValue} = toRefs(props);

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
    const result = useSysModel().sysModelList.find((item: any) => item.Id === newVal);
    emit('update:modelValue', result);
  }
})

interface optionsItem {
    model: string
    label: string
    value: string
    id: number
}
const options = ref<optionsItem[]>([]);

const remoteMethod = (query: string) => {
    loading.value = true;
    if (query !== '') {
        const res = useSysModel().sysModelList;
        options.value = res.filter((item: any) => item.Name !== null && item.Name.trim() !== "" && item.Name.toLowerCase().trim().indexOf(query.toLowerCase()) > -1)
            .map((item: any) => ({
                id: item.Id,
                model: item.Description,
                label: item.Name+ "[" + item.Description + "]",
                value: item.Id,
            })) as optionsItem[];
    } else {
        options.value = [];
    }
    loading.value = false;
}


</script>
<style>

</style>