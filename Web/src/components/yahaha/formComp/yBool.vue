<template>
    <el-tag v-if="internalValue"> 是 </el-tag>
    <el-tag type="danger" v-else> 否 </el-tag>
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, toRefs, onMounted } from 'vue';
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

const updateFieldValue = (newValue: any) => {
    emit('update:modelValue', newValue);
};

</script>
  