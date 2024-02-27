<template >
    <div class="yhh-search-bool-style">
        <el-select ref="ysearchbool" v-model="selectedValue" clearable default-first-option type="daterange"
            :placeholder="'请选择'" @change="updateValue">
            <el-option v-for="item in comparisons" :key="item.key" :label="item.name" :value="item.value"
                :disabled="item.disabled"></el-option>
        </el-select>

    </div>
</template>
  
<script setup lang="ts">
import { PropType, ref, toRefs, toRef } from 'vue';
import { fieldFilter } from '/@/api-services/models';
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
const { modelValue } = toRefs(props);
const text = ref(modelValue?.value);
const selectedValue = ref([] as string[]);
const ysearchbool = ref();
const Separator = " || "
const comparisons = ref([
    { index: 0, name: "是", label: "是", key: "IsNullOrEmpty", value: JSON.stringify({ ConditionalType: "Equal", value: "true" }), disabled: false },
    { index: 1, name: "否", label: "否", key: "IsNot", value: JSON.stringify({ ConditionalType: "Equal", value: "false" }), disabled: false },
]);



const updateValue = (newValue: any) => {
    emit('update:modelValue', [newValue]);

    ysearchbool.value.blur();
    ysearchbool.value.focus();
};

const updateValueByVisible = (visible: boolean) => {
    if (!visible) {
        cleanNewValue();
    }
    //console.log(comparisons);
};

const cleanNewValue = () => {

    // const newLabel = comparisons.value.find((item) => item.label === 'new');
    // if (newLabel) {
    //     // 清理临时数据
    //     newLabel.options.forEach((item) => {
    //         if (item.index < 5) {
    //             item.name = item.label;
    //             item.value = "";
    //             item.disabled = true;
    //         }
    //         else if (item.index >= 5) {
    //             item.disabled = false;
    //         }
    //     });
    // }
};

const cleanTagOption = (tagValue: any) => {
    


};


const cleanAllTagOption = () => {

};

</script>

<style lang="scss" scoped>
.yhh-search-bool-style {
    width: 100%;

    .el-select {
        width: 100%;
    }

    .el-tag {
        background-color: var(--el-color-primary) !important;

    }

    .el-select__tags-text {
        color: var(--el-fill-color) !important;

    }

    .el-tag__close {
        color: var(--el-fill-color);
    }
}
</style>

  