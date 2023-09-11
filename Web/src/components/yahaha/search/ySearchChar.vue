<template >
    <div class="yhh-search-char-style">
        <el-select ref="ysearchchar" v-model="selectedValue" clearable multiple filterable default-first-option
            :popper-append-to-body="false" type="daterange" :placeholder="'请输入' + props.field?.description"
            :filter-method="handleFilter" @change="updateValue" @remove-tag="cleanTagOption" @clear="cleanAllTagOption"
            @visible-change="updateValueByVisible">
            <el-option-group v-for="group in comparisons" :key="group.label">
                <el-option v-for="item in group.options" :key="item.key" :label="item.name" :value="item.value"
                    :disabled="item.disabled"></el-option>
            </el-option-group>
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
const ysearchchar = ref();
const Separator = " || "
const comparisons = ref([
    {
        label: 'new',
        options: [
            { index: 0, name: "包含", label: "包含", key: "Like", value: "", disabled: true },
            { index: 1, name: "等于", label: "等于", key: "Equal", value: "", disabled: true },
            { index: 2, name: "不等于", label: "不等于", key: "NoEqual", value: "", disabled: true },
            { index: 3, name: "起始于", label: "起始于", key: "LikeLeft", value: "", disabled: true },
            { index: 4, name: "结束于", label: "结束于", key: "LikeRight", value: "", disabled: true },
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

    if (newLabel) {
        newLabel.options.forEach((item) => {
            //查看是否已被选择
            let isSelected = comparisons.value[1].options.find((option) => option.value === JSON.stringify({ ConditionalType: item.key, value: value }));

            if (item.index < 5 && value && !isSelected) {
                item.name = item.label + ":" + value;

                item.value = JSON.stringify({ ConditionalType: item.key, value: value });
                item.disabled = false;
            }else if (item.index >=5) {
                item.disabled = false;
            }else{
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
    };
    cleanNewValue();

    ysearchchar.value.blur();
    ysearchchar.value.focus();
};

const updateValueByVisible = (visible: boolean) => {
    if (!visible) {
        cleanNewValue();
    };
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
            else if (item.index >=5) {
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
    };


};


const cleanAllTagOption = () => {

    comparisons.value[1].options = comparisons.value[1].options.filter(option => option.value == option.value);

};

</script>

<style lang="scss">
.yhh-search-char-style {
    width: 100%;

    .el-select {
        width: 100%;
    }

    .el-tag {
        background-color: var(--el-color-primary) !important;

    }

    .el-select__tags-text {
        color: var(--next-color-white) ;

    }

    .el-tag__close {
        color: var(--el-fill-color);
    }
}
</style>

  