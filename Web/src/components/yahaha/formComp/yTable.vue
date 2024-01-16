<template>
    <el-popover :width="400" trigger="click" :fit="true" :fallback-placements="['bottom', 'top', 'right', 'left']">
        <template #reference>
            <el-text truncated>预览</el-text>
        </template>
        <el-table :data="internalValue" :show-overflow-tooltip="true">
            <el-table-column v-for="field in fields" :key="field.name" :prop="field.name" :label="field.description">
            </el-table-column>
        </el-table>
    </el-popover>
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, toRefs, onMounted } from 'vue';
import { useSysModel } from '/@/stores/sysModel';
import { SysFields } from '/@/api-services/models';
const emit = defineEmits(['update:modelValue'])
const props = defineProps({
    modelValue: {
        type: Object as any,
        required: false,
    },
    field: {
        type: Object as PropType<SysFields>,
        required: true,
    },
});
const row  = toRefs(props.modelValue);
const fieldInfo = toRefs(props.field);
//console.log(props.modelValue[props.field.name]);
const internalValue = ref(props.modelValue[props.field.name]);
const fields = ref<SysFields[]>([]);
//获取字段信息

const fetchData = () => {
    var res = useSysModel().getSysModels(props.field.tType);
    fields.value = res?.filter((item: any) => item.description !== null && item.description.trim() !== "")
    .map((item: any) => ({
      ...item as SysFields,
    })) as SysFields[];
    console.log(fields.value);
};

onMounted(() => {
  fetchData();
})

</script>
  