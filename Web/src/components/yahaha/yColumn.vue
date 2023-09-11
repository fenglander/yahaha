<template>
    <component :field="field" :is="getComponentType(field)" />
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, watch } from 'vue';
import yInput from '/@/components/yahaha/formComp/yInput.vue';
import yBool from '/@/components/yahaha/formComp/yBool.vue';
import yText from '/@/components/yahaha/formComp/yText.vue';
import yTable from '/@/components/yahaha/formComp/yTable.vue';
import { SysFields } from '/@/api-services/models';


const props = defineProps({
    field: {
        type: Object as PropType<SysFields>,
        required: true,
    },
})


const getComponentType = (field: any) => {
    if (field.tType === 'Boolean') {
        return yBool as any;
    } else if (field.navigatType === 'OneToMany') {
        return yText as any;
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
  