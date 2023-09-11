<template>
    <el-popover placement="top-end" :width="200" trigger="hover">
        <template #reference>
            <el-text truncated>查看</el-text>
        </template>
        <el-table :data="modelValue">
        </el-table>
    </el-popover>
</template>
  
<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { SysFields } from '/@/api-services/models';

export default defineComponent({
    props: {
        modelValue: {
            type: Object as any,
            required: false,
        },
        field: {
            type: Object as PropType<SysFields>,
            required: true,
        },
    },
});
</script>
  