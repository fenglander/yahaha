<!-- Created by 337547038 -->
<template>
  <el-drawer v-model="visible" size="60%" :title="title" :direction="direction" class="yhh-ace-dialog"
    :append-to-body="true" :before-close="drawerBeforeClose">
    <template #header>
      <el-text>{{ title }} </el-text>
    </template>
    <el-input v-model="textarea" :rows="2" type="textarea" placeholder="Please input" />
    <!-- <div v-if="visible" :id="id"></div> -->
    <div>
      <el-button type="primary" size="small" @click="dialogConfirm">
        确定
      </el-button>
    </div>
  </el-drawer>
</template>

<script setup lang="ts">
import { ref, nextTick, watch, onMounted, onUnmounted } from 'vue'
import { aceEdit } from '../utils'
const props = withDefaults(
  defineProps<{
    modelValue: boolean
    title?: string
    direction?: string //'rtl' | 'ltr'
    content: string
    id?: string
    codeType?: string
  }>(),
  {
    id: 'editJson',
    content: '',
    direction: 'ltr'
  }
)
const emits = defineEmits<{
  (e: 'beforeClose'): void
  (e: 'confirm', content: string): void
  (e: 'update:modelValue', val: boolean): void
}>()
const editor = ref({})
const visible = ref(false)
watch(
  () => props.modelValue,
  (val: boolean) => {
    visible.value = val
    if (val) {
      initEditor()
    }
  }
)
const initEditor = () => {
  nextTick(() => {
    editor.value = aceEdit(props.content, props.id, props.codeType)
  })
}
const dialogConfirm = () => {
  const editVal = editor.value.getValue()
  emits('confirm', editVal)
}
const drawerBeforeClose = () => {
  emits('update:modelValue', false)
  emits('beforeClose')
}
onMounted(() => { })
onUnmounted(() => {
  if (Object.keys(editor.value).length !== 0) {
    editor.value.destroy()
    editor.value.container.remove()
  }
})
</script>

<style lang="scss" scoped>
.yhh-ace-dialog {

  .el-drawer__body {
    padding: 0
  }

  .el-drawer__header {
    margin: 0;
    color: #e9e9e9;
    font-size: 12px;
    padding: 3px 10px
  }

  .dialog-footer {
    text-align: center;
    padding-top: 5px;
  }
}
</style>
