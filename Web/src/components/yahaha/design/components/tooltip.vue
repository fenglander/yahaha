<!-- Created by 337547038 on 2021/11/30. -->
<template>
  <el-tooltip placement="top" >
    <template #content>
      <span v-html="content"></span>
    </template>
    <el-icon><ele-InfoFilled /></el-icon>
  </el-tooltip>
</template>

<script lang="ts" setup>
  withDefaults(defineProps<{ content: string }>(), {})
</script>
