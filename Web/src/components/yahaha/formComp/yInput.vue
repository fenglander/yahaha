<template>
    <el-input v-model="text" :clearable="true" @change="updateFieldValue"/>
</template>
  
<script setup lang="ts">
import { ref, toRefs } from 'vue';
const emit = defineEmits(['update:modelValue'])
const props = defineProps({
    modelValue: {
            required: false,
        },
});
const {modelValue} = toRefs(props);
const text = ref(modelValue?.value);

const updateFieldValue = (newValue: any) => {
    emit('update:modelValue', newValue);
};

</script>
  