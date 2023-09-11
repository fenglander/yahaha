<template>
    <el-popover placement="top-end" popper-class="el-popover-self" :width="popoverWidth" :visible="isPopover" :content="internalValue" :fallback-placements="['bottom', 'top', 'right', 'left']">
        <template #reference>
            <div ref="ytext"  class="member-label member-span text-hidden" @mouseenter="visibilityChange($event)" @mouseleave="() => isPopover = false">
                <span>{{ internalValue }}</span>
            </div>

        </template>
    </el-popover>
</template>
  
<script setup lang="ts">
import { defineComponent, PropType, ref, toRefs } from 'vue';
import { SysFields } from '/@/api-services/models';
const isPopover = ref(false);
const popoverWidth = ref(200);
const ytext = ref();
const props = defineProps({
    modelValue: {
        type: Object as any,
        required: false,
    },
    field: {
        type: Object as PropType<SysFields>,
        required: true,
    },
});
const row  = toRefs(props.modelValue);
const fieldInfo = toRefs(props.field);
//console.log(props.modelValue[props.field.name]);
const internalValue = ref(props.modelValue[props.field.name]);

const visibilityChange = (event: any) => {
    const ev = event.target;
    const ev_weight = ev.scrollWidth; // 文本的实际宽度   scrollWidth：对象的实际内容的宽度，不包边线宽度，会随对象中内容超过可视区后而变大。
    const content_weight = ev.clientWidth;// 文本的可视宽度 clientWidth：对象内容的可视区的宽度，不包滚动条等边线，会随对象显示大小的变化而改变。
    //const content_weight = ytext.value.clientWidth; // 文本容器宽度(父节点)
    //console.log('ev_weight',ev_weight);
    //console.log('ev_weight',content_weight);
    if (ev_weight > content_weight) {
        // 实际宽度 > 可视宽度  文字溢出
        popoverWidth.value = ev_weight <= 250 ? ev_weight + 50 : 300;

        isPopover.value = true;
    } else {
        // 否则为不溢出
        isPopover.value = false;
    }
};

</script>
<style>
.text-hidden {
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.el-popover-self {
  min-width: 30px!important;
  padding: 0;
}
</style>