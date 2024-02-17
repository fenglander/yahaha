<template>
  <div style="width: 100%;height: 100%;">
    <!-- 使用 MyTable 组件 -->
    <yList modelName="FormDesign" :formComp="formDes">
    </yList>
  </div>
</template>
  
<script setup lang="ts" name="visualDev">
import { onMounted } from 'vue';
import yList from '/@/components/yahaha/yList.vue';
import formDes from './component/formDes.vue';
onMounted(() => {
  //console.log(123);
});

</script>