<template >
        <el-select class="yhh-search-num-style" id="ysearchnum" ref="ySelect" v-model="selectedValue" clearable multiple filterable default-first-option
            :popper-append-to-body="false" type="daterange" :placeholder="'请输入' + props.field?.Description"
            :filter-method="handleFilter" @change="updateValue" @remove-tag="cleanTagOption" @clear="cleanAllTagOption"
            @visible-change="updateValueByVisible">
            <el-option-group v-for="group in comparisons" :key="group.label">
                <el-option v-for="item in group.options" :key="item.key" :label="item.name" :value="item.value"
                    :disabled="item.disabled"></el-option>
            </el-option-group>
        </el-select>
</template>
  
<script setup lang="ts">
import { PropType, ref, onMounted } from 'vue';
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
const selectedValue = ref([] as string[]);
const ySelect = ref();
const comparisons = ref([
    {
        label: 'new',
        options: [
            { index: 0, name: "等于", label: "等于", key: "Equal", value: "", disabled: true },
            { index: 1, name: "大于等于", label: "大于等于", key: "GreaterThanOrEqual", value: "", disabled: true },
            { index: 2, name: "小于等于", label: "小于等于", key: "LessThanOrEqual", value: "", disabled: true },
            { index: 3, name: "大于", label: "大于", key: "GreaterThan", value: "", disabled: true },
            { index: 4, name: "小于", label: "小于", key: "LessThan", value: "", disabled: true },
            { index: 5, name: "空", label: "空", key: "IsNullOrEmpty", value: JSON.stringify({ ConditionalType: "IsNullOrEmpty", value: "null" }), disabled: false },
            { index: 6, name: "不是空", label: "不是空", key: "IsNot", value: JSON.stringify({ ConditionalType: "IsNot", value: "null" }), disabled: false },
        ],
    },
    {
        label: 'selected',
        options: [

        ],
    },
]);


const handleFilter = (value: string) => {
    const newLabel = comparisons.value.find((item) => item.label === 'new');

    //console.log(value);
    if (newLabel) {
        newLabel.options.forEach((item) => {
            //查看是否已被选择
            let isSelected = comparisons.value[1].options.find((option) => option.value === JSON.stringify({ ConditionalType: item.key, value: value }));

            if (item.index < 5 && value && !isSelected) {
                item.name = item.label + ":" + value;

                item.value = JSON.stringify({ ConditionalType: item.key, value: value });
                item.disabled = false;
            } else if (item.index >= 5) {
                item.disabled = false;
            } else {
                item.disabled = true;
            }
        });
    }
    return comparisons;
};

const updateValue = (newValue: any) => {
    emit('update:modelValue', newValue);
    let curr = newValue[newValue.length - 1]

    const valueToMove = comparisons.value[0].options.find((option) => option.value === curr);

    if (valueToMove) {
        // 将找到的字典复制到 "label: 'selected'" 的 "options" 下
        comparisons.value[1].options.push({
            index: valueToMove.index,
            name: valueToMove.name,
            label: valueToMove.label,
            key: valueToMove.key,
            value: valueToMove.value,
            disabled: false
        });
    }
    cleanNewValue();

    ySelect.value.blur();
    ySelect.value.focus();
};

const updateValueByVisible = (visible: boolean) => {
    if (!visible) {
        cleanNewValue();
    }
    //console.log(comparisons);
};

const cleanNewValue = () => {

    const newLabel = comparisons.value.find((item) => item.label === 'new');
    if (newLabel) {
        // 清理临时数据
        newLabel.options.forEach((item) => {
            if (item.index < 5) {
                item.name = item.label;
                item.value = "";
                item.disabled = true;
            }
            else if (item.index >= 5) {
                item.disabled = false;
            }
        });
    }
};

const cleanTagOption = (tagValue: any) => {
    const selectedLabel = comparisons.value.find((item) => item.label === 'selected');
    if (selectedLabel) {

        const indexToRemove = selectedLabel.options.findIndex((option) => option.value === tagValue);
        if (indexToRemove !== -1) {
            // 如果找到了，就从数组中删除该对象
            selectedLabel.options.splice(indexToRemove, 1);
        }
    }
};


const cleanAllTagOption = () => {

    comparisons.value[1].options = comparisons.value[1].options.filter(option => option.value == option.value);

};

//const selectRef = ref(); // 初始化为 null
// 处理输入事件
const handleInput = (Value: any) => {

    // 移除非数字、非小数点、非负号的字符
    let newValue = Value.replace(/[^-0-9.]/g, '');

    // 处理负号，确保只能出现在开头且只出现一次
    if (newValue.indexOf('-') !== 0) {
        // 如果负号不在开头，将其移除
        newValue = newValue.replace('-', '');
    } else if (newValue.indexOf('-', 1) !== -1) {
        // 如果负号出现在非开头位置，将其移除
        newValue = '-' + newValue.replace(/-/g, '');
    }

    // 处理小数点，确保只能出现一次
    const decimalPointIndex = newValue.indexOf('.');
    if (decimalPointIndex !== -1) {
        const parts = newValue.split('.');
        newValue = parts[0] + '.' + parts.slice(1).join('');
    }
    return newValue;
};

onMounted(() => {
    // 在组件渲染后，通过$refs获取el-select元素
    const selectElement = ySelect.value.$el;

    // 获取内部的input元素
    const inputElement = selectElement.querySelector('input');
    // 添加input事件监听器
    inputElement.addEventListener('input', (event: any) => {
        inputElement.value = handleInput(event.target.value);
    });
});


</script>

<style lang="scss" scoped>
.yhh-search-num-style {
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

  