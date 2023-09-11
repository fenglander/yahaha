<template>
    <component :field="props.field" :is="getComponentType(props.field.tType)" />
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, watch } from 'vue';
import ySearchChar from './ySearchChar.vue';
import ySearchNum from './ySearchNum.vue';
import ySearchBool from './ySearchBool.vue';
import ySearchDatetime from './ySearchDatetime.vue';
import { fieldFilter } from '/@/api-services/models';


const props = defineProps({
    field: {
        type: Object as PropType<fieldFilter>,
        required: true,
    },
})


const getComponentType = (tType: string) => {
    if (tType === "Int64" || tType === "Int32") {
        return ySearchNum as any;
    }
    else if (tType === 'String') {
        return ySearchChar as any;
    }
    else if (tType === 'Boolean') {
        return ySearchBool as any;
    }
    else if (tType === 'DateTime') {
        return ySearchDatetime as any;
    }
    // 添加其他tType的处理逻辑
};

</script>
  