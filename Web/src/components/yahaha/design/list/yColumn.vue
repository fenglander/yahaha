<template>
    <component :field="props.field" :is="getComponentType(props.field)" />
</template>
  
<script setup lang="ts">
import {  PropType,  } from 'vue';
import yBool from '/@/components/yahaha/formComp/yBool.vue';
import yText from '/@/components/yahaha/formComp/yText.vue';
import yTable from '/@/components/yahaha/formComp/yTable.vue';
import { SysField } from '/@/api-services/models';


const props = defineProps({
    field: {
        type: Object as PropType<SysField>,
        required: true,
    },
})


const getComponentType = (field: any) => {
    if (field.tType === 'Boolean') {
        return yBool as any;
    } else if (field.navigatType === 'OneToMany') {
        return yTable as any;
    }
    else {
        return yText as any;
    }

    // if (field.navigatType === 'OneToMany') {
    //     return yTable as any;
    // } else {
    //     return yText as any;
    // }

    return yText as any;
    // 添加其他tType的处理逻辑
};

</script>
  