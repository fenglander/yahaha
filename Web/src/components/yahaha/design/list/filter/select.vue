<template >
    <div class="yhh-search-select-style">
        <el-select id="ysearchselect" ref="ySelect" v-model="selectedValue" clearable multiple filterable
            default-first-option :popper-append-to-body="false" type="daterange"
            :placeholder="'请选择' + props.field?.Description" @change="updateValue">
            <el-option v-for="item in comparisons" :key="item.key" :label="item.description"
                :value="item.value"></el-option>
        </el-select>

    </div>
</template>
  
<script setup lang="ts">
import { PropType, ref, onMounted, computed } from 'vue';
import { fieldFilter } from '/@/api-services/models';
import { stringToObj } from '/@/components/yahaha/design/utils'
const emit = defineEmits(['update:modelValue'])
const props = defineProps({
    modelValue: {
        required: false,
    },
    field: {
        type: Object as PropType<fieldFilter>,
        required: true,
    }
});
const selectedValue = ref([] as string[]);
const ySelect = ref();

const comparisons = computed(() => {
    const EnumValue = stringToObj(props.field.EnumValue ?? "")
    return EnumValue;
});


const updateValue = (newValue: any) => {
    let value = null
    if (newValue.length > 0) {
        value = JSON.stringify({ ConditionalType: "In", value: newValue.join(',') });
    }
    emit('update:modelValue', value);
};


onMounted(() => {

});


</script>

<style lang="scss" scoped>
.yhh-search-select-style {
    width: 100%;

    :deep(.el-select) {
        width: 100%;
    }

    :deep(.el-tag) {
        background-color: var(--el-color-primary-light-9) !important;

    }

    :deep(.el-select__tags-text) {
        color: var(--el-color-primary) ;

    }

    :deep(.el-tag__close) {
        color: var(--el-color-primary);
    }
}
</style>

  