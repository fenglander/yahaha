<template>
    <component :field="props.field" :is="getComponentType(props.field.tType)" />
</template>
  
<script setup lang="ts">
import { PropType } from 'vue';
import char from './char.vue';
import num from './num.vue';
import bool from './bool.vue';
import datetime from './datetime.vue';
import select from './select.vue';
import manyToOne from './manyToOne.vue';
import oneToMany from './oneToMany.vue';
import { fieldFilter } from '/@/api-services/models';


const props = defineProps({
    field: {
        type: Object as PropType<fieldFilter>,
        required: true,
    },
})


const getComponentType = (tType: string) => {
    if (["Int64", "Int32", "Single", "Double"].includes(tType)) {
        return num as any;
    }
    else if (["String", "BigString"].includes(tType)) {
        return char as any;
    }
    else if (tType === 'Boolean') {
        return bool as any;
    }
    else if (tType === 'DateTime') {
        return datetime as any;
    } else if (tType === 'Select') {
        return select as any;
    }else if (tType === 'ManyToOne') {
        return manyToOne as any;
    }else if (tType === 'OneToMany') {
        return oneToMany as any;
    }
    // 添加其他tType的处理逻辑
};

</script>
  