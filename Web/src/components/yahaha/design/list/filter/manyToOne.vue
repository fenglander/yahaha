<template >
    <el-select class="yhh-search-manyToOne-style" ref="ysearchchar" v-model="selectedValue" clearable multiple filterable
        default-first-option :popper-append-to-body="false" type="daterange" :placeholder="'请输入' + props.field?.Description"
        :loading="loading" :filter-method="handleFilter" @change="updateValue" @remove-tag="cleanTagOption"
        @clear="cleanAllTagOption" @visible-change="updateValueByVisible">
        <el-option class="yhh-search-manyToOne-opations" v-for="item in comparisons" :key="item.id" :label="item.ModelTitle"
            :value="item" :disabled="item.disabled">
            <el-row align="middle">
                <el-button v-if="!item.isDefault" style="margin-right: 10px;" type="danger"
                    @click="notEqualBut(item)">不等于</el-button>
                <el-text>{{ item.name }}</el-text>
            </el-row>
        </el-option>
        <template #tag>
            <el-tag v-for="item in selectedValue" :key="item.id" :type="getTagType(item)">{{ item.name }}</el-tag>
        </template>
    </el-select>
</template>
  
<script setup lang="ts">
import { PropType, ref } from 'vue';
import { fieldFilter } from '/@/api-services/models';
//import { useSysModel } from '/@/stores/sysModel';
import { selRelObjectQuery } from '/@/api/widget';
import { debounce } from 'lodash-es';

const emit = defineEmits(['update:modelValue'])
const loading = ref(false);
const props = defineProps({
    modelValue: {
        required: false,
    },
    field: {
        type: Object as PropType<fieldFilter>,
        required: true,
    }
});
const selectedValue = ref([] as any[]);
const ysearchchar = ref();
const comparisons = ref<any[]>([]);
const notEqualItem = ref<any>();

const basic = ref([
    { id: 5, name: "已设置", label: "已设置", key: "IsNot", value: JSON.stringify({ ConditionalType: "IsNot", value: null }), isDefault: true },
    { id: 6, name: "未设置", label: "未设置", key: "EqualNull", value: JSON.stringify({ ConditionalType: "EqualNull", value: null }), isDefault: true },
])


const selRelObjectQueryDebounce = debounce(
    async function (query: string) {
        const params = {
            keywords: query,
            relModelName: props.field.RelModel?.Name,
            pageSize: 5,
        };
        comparisons.value = [];
        const res = await selRelObjectQuery(params);
        res.data.result.forEach((it: any) => {
            comparisons.value.push({
                id: it.Id,
                value: it.Id,
                name: it.ModelTitle,
                label: it.ModelTitle,
                key: it.Id,
                isDefault: false,
                notEqual: false,
            });
        });
        comparisons.value = [...comparisons.value, ...basic.value];
        loading.value = false;
    },
    300
);


/**自定义筛选方法 */
const handleFilter = async (value: string) => {
    loading.value = true
    await selRelObjectQueryDebounce(value);
    return comparisons.value;
};

const notEqualBut = (item: any) => {
    notEqualItem.value = item;
}

/**选中值发生变化时触发 */
const updateValue = (newValue: any) => {
    const newItem = newValue.length > 0 ? newValue[newValue.length - 1] : undefined;
    // 默认否
    if (!newItem) {
        comparisons.value.forEach(it => it.notEqual = false); notEqualItem.value = undefined;
    } else {
        newItem.notEqual = false;
    }
    if (notEqualItem.value) {
        const curr = selectedValue.value.find((item) => item.id === notEqualItem.value.id);
        if (curr) curr.notEqual = true;
        notEqualItem.value = undefined;
    }
    let valForUpdate: any[] = [];
    newValue.forEach((item: any) => {
        if ([5, 6].includes(item.id)) {
            valForUpdate.push({ id: item.id, filterExp: item.value });
        } else if (item.notEqual) {
            valForUpdate.push({ id: item.id, filterExp: JSON.stringify({ ConditionalType: "NoEqual", value: item.id }) });
        } else {
            valForUpdate.push({ id: item.id, filterExp: JSON.stringify({ ConditionalType: "Equal", value: item.id }) });
        }
    })
    emit('update:modelValue', valForUpdate);

    // throw new Error("")
    cleanNewValue();
};
/**下拉框出现/隐藏时触发 */
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
        newLabel.options.forEach((item: any) => {
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
/**多选模式下移除tag时触发 */
const cleanTagOption = (tagValue: any) => {
    const selectedLabel = comparisons.value.find((item) => item.label === 'selected');
    if (selectedLabel) {

        const indexToRemove = selectedLabel.options.findIndex((option: any) => option.value === tagValue);
        if (indexToRemove !== -1) {
            // 如果找到了，就从数组中删除该对象
            selectedLabel.options.splice(indexToRemove, 1);
        }
    }
};

/**可清空的单选模式下用户点击清空按钮时触发 */
const cleanAllTagOption = () => {

    comparisons.value[1].options = comparisons.value[1].options.filter((option: any) => option.value == option.value);
};

const getTagType = (item: any) => {
    return item.notEqual ? "danger" : "primary";
}



</script>

<style lang="scss" scoped>
.yhh-search-manyToOne-style {
    width: 100%;

    :deep(.el-select) {
        width: 100%;
    }

    :deep(.el-select__tags-text) {
        color: var(--el-fill-color) !important;

    }

    :deep(.el-tag__close) {
        color: var(--el-fill-color);
    }

}

.yhh-search-manyToOne-opations {

    :global(.is-selected::after) {
        top: 0 !important;
        right: 0px !important;
        position: unset !important;
    }
}

.danger-tag {
    background-color: var(--el-color-danger);
}
</style>

  