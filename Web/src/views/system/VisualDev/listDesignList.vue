<template>
  <div style="width: 100%;height: 100%;">
    <!-- 使用 MyTable 组件 -->
    <yList modelName="ListDesign" :formComp="listDes">
    </yList>
  </div>
</template>
  
<script setup lang="ts" name="visualDev">
import { onMounted } from 'vue';
import yList from '/@/components/yahaha/yList.vue';
import listDes from './component/listDes.vue';
onMounted(() => {
  //console.log(123);
});

</script>