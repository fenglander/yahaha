<template>
    <div style="width: 100%;height: 100%;">
        <listRenderer  :model-id="modelId" />
    </div>
</template>
    
<script setup lang="ts" name="generalView">
import { ref, reactive, onMounted } from 'vue';
import listRenderer from '/@/components/yahaha/design/list/listRenderer.vue'
import { formatNumber } from '/@/components/yahaha/design/utils/'
import { useRouter } from 'vue-router';
const router = useRouter();
const designId = ref<number>();
const modelId = ref<number>();
const isKeepAlive = ref<any>();


console.log('页面参数：', router.currentRoute.value)
const query = reactive(router.currentRoute.value.query);
isKeepAlive.value = router.currentRoute.value.meta.isKeepAlive;
designId.value = formatNumber(query.formDesignId);
modelId.value = formatNumber(query.modelId);
onMounted(async () => {
	//console.log('onMounted')
});

</script>