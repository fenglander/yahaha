<template>
    <el-switch v-model="internalValue" @change="updateFieldValue"/>
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, toRefs, onMounted } from 'vue';
const emit = defineEmits(['update:modelValue'])
const props = defineProps({
    modelValue: {
            type: Boolean as PropType<boolean>,
            required: false,
        },
});
const {modelValue} = toRefs(props);
const internalValue = ref(modelValue?.value);

const updateFieldValue = (newValue: any) => {
    emit('update:modelValue', newValue);
};

</script>
  