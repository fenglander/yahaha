<template>
    <div style="width: 100%;height: 100%;">
        <!-- Pass designId as a prop to yList -->
        <yList :design="designId" :create="true" :edit="true" :del="true" />
    </div>
</template>
    
<script setup lang="ts" name="generalView">
import { ref, reactive, onMounted } from 'vue';
import yList from '/@/components/yahaha/yList.vue';
import { formatNumber } from '/@/components/yahaha/design/utils/'
import { useRouter } from 'vue-router';
const router = useRouter();
const designId = ref<number>();
const isKeepAlive = ref<any>();


console.log('页面参数：', router.currentRoute.value)
const query = reactive(router.currentRoute.value.query);
isKeepAlive.value = router.currentRoute.value.meta.isKeepAlive;
designId.value = formatNumber(query.visualDevId);
onMounted(async () => {
	//console.log('onMounted')
});

</script>