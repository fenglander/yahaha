<template >
    <div class="yhh-search-datetime-style">

        <el-date-picker v-model="selectedValue" @change="updateValue" :type="pickerType" :shortcuts="shortcuts"
            range-separator="至" start-placeholder="起始" end-placeholder="结束" />
    </div>
</template>
  
<script setup lang="ts">
import { PropType, ref } from 'vue';
import { fieldFilter } from '/@/api-services/models';
const props = defineProps({
    modelValue: {
        required: false,
    },
    field: {
        type: Object as PropType<fieldFilter>,
        required: true,
    }
});
const emit = defineEmits(['update:modelValue'])
const selectedValue = ref([] as string[]);
const pickerType = ref("datetimerange");

const updateValue = (newValue: any) => {
    let value: any[] = [];
    if (newValue && Array.isArray(newValue) && newValue.length > 1) {
        value = [
            JSON.stringify({ ConditionalType: "GreaterThanOrEqual", value: newValue[0] }),
            JSON.stringify({ ConditionalType: "LessThanOrEqual", value: newValue[1] })
        ]
    }
    console.log(value);
    emit('update:modelValue', value);
};
const shortcuts = [
    {
        text: '今天',
        value: () => {
            const today = new Date();
            const startOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0, 0);
            const endOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
            return [startOfDay, endOfDay];
        },
    },
    {
        text: '昨天',
        value: () => {
            const today = new Date();
            const startOfYesterday = new Date(today);
            startOfYesterday.setDate(today.getDate() - 1);
            startOfYesterday.setHours(0, 0, 0, 0);

            const endOfYesterday = new Date(today);
            endOfYesterday.setDate(today.getDate() - 1);
            endOfYesterday.setHours(23, 59, 59, 999);

            return [startOfYesterday, endOfYesterday];
        },
    },
    {
        text: '本周',
        value: () => {
            const today = new Date();
            const currentDayOfWeek = today.getDay(); // 0 表示星期日，1 表示星期一，以此类推

            // 计算距离本周一的天数差
            const daysToMonday = currentDayOfWeek === 0 ? 6 : currentDayOfWeek - 1;

            const startOfWeek = new Date(today);
            startOfWeek.setDate(today.getDate() - daysToMonday);
            startOfWeek.setHours(0, 0, 0, 0);

            const endOfWeek = new Date(today);
            endOfWeek.setDate(today.getDate() + (6 - daysToMonday));
            endOfWeek.setHours(23, 59, 59, 999);

            return [startOfWeek, endOfWeek];
        },
    },
    {
        text: '近1周',
        value: () => {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
            return [start, end]
        },
    },
    {
        text: '近30天',
        value: () => {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30)
            return [start, end]
        },
    },
    {
        text: '近90天',
        value: () => {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90)
            return [start, end]
        },
    },
    {
        text: '近365天',
        value: () => {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 365)
            return [start, end]
        },
    },
]

</script>

<style lang="scss">
.yhh-datetime-bool-style {
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

  