<template>
  <div  style="width: 100%;height: 100%;">
    <!-- 使用 MyTable 组件 -->
    <yList model="SysModels" :create="true" :edit="true" :del="true" >
    </yList>
  </div>
</template>

<script setup lang="ts" name="inventory">
  import { ref, onMounted } from 'vue';
  import yList from '/@/components/yahaha/yList.vue';
  const internalValue = ref("");
  onMounted(() => {
    //console.log(123);
  });

</script>