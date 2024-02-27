<template >
    <div class="yhh-search-oneToMany-style">
        <el-select ref="ysearchOneToMany" v-model="selectedValue" clearable multiple filterable default-first-option
            :popper-append-to-body="false" type="daterange" :placeholder="'请输入' + props.field?.Description"
            :filter-method="handleFilter" @change="updateValue" @remove-tag="cleanTagOption" @clear="cleanTagOption"
            @visible-change="updateValueByVisible">
            <el-option-group v-for="group in comparisons" :key="group.label">
                <el-option v-for="item in group.options" :key="item.key" :label="item.name" :value="item.value"
                    :disabled="item.disabled"></el-option>
            </el-option-group>
        </el-select>
        <!-- <el-button type="danger"  size="small" circle >且</el-button> -->
    </div>
</template>
  
<script setup lang="ts">
import { PropType, ref, computed } from 'vue';
import { fieldFilter } from '/@/api-services/models';
import { useSysModel } from '/@/stores/sysModel';
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
const ysearchOneToMany = ref();
const comparisons = ref([
    {
        label: 'new',
        options: [
            { index: 0, name: "包含", label: "包含", key: "Like", value: "", disabled: true },
            { index: 1, name: "等于", label: "等于", key: "Equal", value: "", disabled: true },
            { index: 2, name: "不等于", label: "不等于", key: "NoEqual", value: "", disabled: true },
            { index: 3, name: "起始于", label: "起始于", key: "LikeLeft", value: "", disabled: true },
            { index: 4, name: "结束于", label: "结束于", key: "LikeRight", value: "", disabled: true },
            { index: 5, name: "已设置", label: "已设置", key: "IsNullOrEmpty", value: JSON.stringify({ ConditionalType: "IsNullOrEmpty", value: null }), disabled: false },
            { index: 6, name: "未设置", label: "未设置", key: "IsNot", value: JSON.stringify({ ConditionalType: "IsNot", value: null }), disabled: false },
        ],
    },
    {
        label: 'selected',
        options: [

        ],
    },
]);

/**获取标题 */
const lableDescription = computed(() => {
    const lables = useSysModel().getSysModelLables(props.field.RelModel?.Name);
    const lastLable = lables[lables.length - 1];
    const lastItem = useSysModel().getSysFields(props.field.RelModel?.Name).find((item: any) => item.Name === lastLable);
    return lastItem.Description;
})


const handleFilter = (value: string) => {
    const newLabel = comparisons.value.find((item) => item.label === 'new');

    if (newLabel) {
        newLabel.options.forEach((item) => {
            //查看是否已被选择
            let isSelected = comparisons.value[1].options.find((option) => option.value === JSON.stringify({ ConditionalType: item.key, value: value }));

            if (item.index < 5 && value && !isSelected) {
                item.name = lableDescription.value + item.label + ":" + value;

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
    const index = comparisons.value[1].options.findIndex((option) => option.value === curr);
    // 匹配到值且已选择不存在
    if (valueToMove && index === -1) {
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
    cleanTagOption(newValue);
    cleanNewValue();

    ysearchOneToMany.value.blur();
    ysearchOneToMany.value.focus();
};

const updateValueByVisible = (visible: boolean) => {
    if (!visible) {
        cleanNewValue();
    }
    //console.log(comparisons);
};

const cleanNewValue = () => {

    const newLabel = comparisons.value.find((item) => item.label === 'new');

    // 清理临时数据
    newLabel!.options.forEach((item) => {
        if (item.index < 5) {
            item.name = item.label;
            item.value = "";
            item.disabled = true;
        }
        else if (item.index >= 5) {
            item.disabled = false;
        }
    });
};

const cleanTagOption = (newValue: any) => {
    //const selectedLabel = comparisons.value.find((item) => item.label === 'selected');
    newValue = newValue ?? selectedValue.value;
    console.log(newValue);
    if (Array.isArray(newValue)) {
        comparisons.value[1].options = comparisons.value[1].options.filter(option => newValue.includes(option.value));
    }

    console.log(comparisons.value);
};



</script>

<style lang="scss" scoped>
.yhh-search-oneToMany-style {
    width: 100%;

    :deep(.el-select) {
        width: 100%;
    }

    :deep(.el-tag) {
        background-color: var(--el-color-primary-light-9) !important;

    }

    :deep(.el-select__tags-text) {
        color: var(--el-color-primary);

    }

    :deep(.el-tag__close) {
        color: var(--el-color-primary);
    }

}
</style>

  