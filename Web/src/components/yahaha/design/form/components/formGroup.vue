<!-- Created by 337547038 on 2021/9/8. -->
<template>
  <draggable itemKey="id" :list="dataList" name="fade" class="drag" v-bind="{
    group: 'form', ghostClass: 'ghost', animation: 300, handle: '.drag-move', disabled: type !== 5
  }" @add="draggableAdd">
    <template #item="{ element, index }">
        <div :class="getGroupClass(element)" :style="getFormItemStyle(element)" @click.stop="groupClick(element)"
          >
          <template v-if="element.type === 'tabs'">
            <div class="form-tabs">
              <el-tabs v-bind="element.control" :class="[element.config?.className]">
                <el-tab-pane v-for="(item, tIndex) in element.columns" :label="item.label" :key="tIndex">
                  <form-group :data="item.list" :all-data="dataList" />
                </el-tab-pane>
              </el-tabs>
            </div>
          </template>
          <template v-else-if="element.type === 'title'">
            <div class="title" :class="[element.config.className]" v-bind="element.control">
              <span v-html="element.control.modelValue"></span>
              <Tooltips :content="element.config.help" v-if="element.config.help" />
            </div>
          </template>
          <!-- <template v-else-if="element.isLayout">
            <div class="form-table" v-if="type === 5">
              <form-group :data="element.list" :all-data="dataList" data-type="not-nested" />
            </div>
          </template> -->
          <template v-else-if="element.type === 'grid'">
            <el-row class="form-row" :class="[element.className]">
              <el-col class="form-col" :class="{
                'active-col': activeKey === getGroupName(col),
                [col.className]: col.className
              }" v-bind="col.attr" v-for="(col, i) in element.columns" :key="i"
                @click.stop="groupClick(col, 'gridChild')">
                <form-group :data="col.list" :all-data="dataList" data-type="not-nested" />
                <div class="drag-control" v-if="type === 5">
                  <div class="item-control">

                    <el-tooltip class="item" content="删除" placement="bottom-end">
                      <el-button @click="click('del', i as number, element.columns)" icon="ele-CloseBold"
                        circle />
                    </el-tooltip>
                  </div>
                </div>
              </el-col>
            </el-row>
          </template>
          <template v-else-if="element.type === 'card'">
            <el-collapse model-value="1">
              <el-collapse-item :title="element.item.label" name="1">
                <template #title v-if="element.help">
                  {{ element.item.label }}
                  <Tooltips :content="element.help" />
                </template>
                <form-group :data="element.list" :all-data="dataList" data-type="not-nested" />
              </el-collapse-item>
            </el-collapse>
          </template>
          <template v-else-if="element.type === 'divider'">
            <el-divider v-bind="element.control">{{ element.item && element.item.label }}
            </el-divider>
          </template>
          <template v-else-if="element.type === 'div'">
            <div class="div-layout" v-bind="element.control" :class="{
              [element.className]: element.className,
              inline: element.config?.inline,
              [element.config?.textAlign]: element.config?.textAlign
            }">
              <form-group :data="element.list" :all-data="dataList" data-type="not-nested" />
            </div>
          </template>
          <template v-else-if="element.type === 'flex'">
            <form-group :data="element.list" :all-data="dataList" data-type="not-nested" v-if="type === 5" />
            <flex-box :data="element" v-else />
            <el-button style="position: relative; top: -28px; left: 10px" v-if="element.config.addBtnText && type === 5"
              size="small">{{ element.config.addBtnText }}</el-button>
          </template>
          <template v-else-if="element.type === 'button'">
            <div :class="[element.config?.className]" :style="{ 'text-align': element.config?.textAlign }">
              <el-button v-bind="element.control" @click="clickBtn(element.control)">{{ element.control?.label
              }}</el-button>
            </div>
          </template>
          <template v-else-if="element.type === 'inputSlot' && type !== 5">
            <!--  除设计外其他无需处理-->
          </template>

          <form-item v-else :data="element" />

          <template v-if="type === 5">
            <div class="drag-control">
              <div class="item-control">
                <template v-if="state.gridAdd">
                  <el-tooltip class="item" content="添加列" placement="bottom-end">
                    <el-button @click="click('gridAdd', index, element)" icon="ele-CirclePlusFilled" circle />
                  </el-tooltip>
                </template>
                <!-- <el-tooltip class="item" content="复制" placement="bottom-end">
                <el-button  v-if="state.clone" @click="click('clone', index, element)" icon="ele-Picture" circle />
              </el-tooltip> -->
                <el-tooltip class="item" content="删除" placement="bottom-end">
                  <el-button @click="click('del', index)" icon="ele-CloseBold" circle />
                </el-tooltip>
              </div>
              <div class="drag-move icon-move"></div>
            </div>
            <div class="tooltip">{{ element.key }}</div>
          </template>
        </div>
      </template>
  </draggable>
</template>

<script lang="ts" setup>
import { reactive, computed, ref, watch, inject, onUnmounted } from 'vue'
import Draggable from 'vuedraggable-es'
import FormItem from './formItem.vue'
import Tooltips from '../../components/tooltip.vue'
import FlexBox from './flexBox.vue'
import { useDesignFormStore } from '/@/stores/designForm'
import type { FormList } from '../../types'
import md5 from 'md5'
import { deepClone,constFormBtnEvent, constFormProps, key } from '../../utils'
const props = withDefaults(
  defineProps<{
    data: FormList[]
    allData?: FormList[]
    parent?: any
  }>(),
  {
    data: () => {
      return []
    }
  }
)
/*  const emits = defineEmits<{
  (e: 'update:data', value: FormList[]): void
}>()*/
const store = useDesignFormStore() as any
const formProps = inject(constFormProps, {}) as any


const type = computed(() => {
  return formProps.value.type
})
const state = reactive({
  clone: true, // 允许clone
  gridAdd: false
})
const dataList = ref(props.data)
watch(
  () => props.data,
  (v: FormList[]) => {
    dataList.value = v
  }
)
const activeKey = computed(() => {
  return store.activeKey
})

// 不能嵌套
const notNested = (type: string) => {
  const controlType = ['grid', 'table', 'tabs', 'div', 'flex', 'card']
  return controlType.includes(type)
}
// 删除或复制
const click = (action: string, index: number, item?: any) => {
  if (type.value !== 5) {
    return // 非设计模式
  }
  if (action === 'clone') {
    if (checkDuplicateField(item.fieldName, 0)) {
      return
    }
    const newItem = deepClone(item)
    dataList.value.splice(index, 0, Object.assign(newItem, { key: key() }))
  } else if (action === 'del') {
    dataList.value.splice(index, 1)
    // 清空右侧栏信息
    store.setActiveKey('')
    store.setControlAttr({})
  } else if (action === 'gridAdd') {
    item.columns.push({
      list: [],
      attr: { span: 12 },
      key: key()
    })
  } else if (action === 'delGridChild') {
    item.splice(index, 1)
  }
}


/**
 * 检查是否有字段重复
 * 新增与复制有些不同
 */
function checkDuplicateField(targetFieldName: string, allow: number): boolean {
  let count = 0;
  if (targetFieldName === undefined || targetFieldName.trim() === '') {
    return false;
  }
  let data = props.allData;
  if (data !== undefined){
    for (const item of data) {
    if (item.fieldName === targetFieldName) {
      count++;
    }

    // if (item.list) {
    //   // 检查子项
    //   item.list.forEach((columnItem: any) => {
    //     if (columnItem.fieldName === targetFieldName) {
    //       count++;
    //     }
    //   });
    // }

    if (item.columns) {
      // 检查子项
      item.columns.forEach((column: any) => {
        column.list.forEach((columnItem: any) => {
          if (columnItem.fieldName === targetFieldName) {
            count++;
          }
        });
      });
    }
  }
  }
  
  return count > allow ? true : false;
}


const draggableAdd = (evt: any) => {
  //console.log('dataList.value', dataList.value);
  if (type.value !== 5) {
    return // 非设计模式
  }
  const newIndex = evt.newIndex;
  const obj: any = dataList.value[newIndex]
  const parentItem = props.parent // 父节点
  // 不允许嵌套
  if (parentItem?.isLayout && obj?.SubFields) {
    dataList.value.splice(newIndex, 1)
    return
  }
  // 子表字段只允许在对应子表
  if (obj.parent && obj.parent.id != parentItem?.id) {
    dataList.value.splice(newIndex, 1)
    return
  }
  // 重复字段
  if (checkDuplicateField(obj.fieldName, 1)) {
    dataList.value.splice(newIndex, 1)
    return
  }
  if (!obj) {
    return
  }

  let objectItem = {}
  // 不需要添加item的项div'
  if (!obj.isLayout) {
    objectItem = {
      item: {
        label: obj.label || obj.item.label
      }
    }
  }
  // 不需要name的组件
  let nameObj = {}
  if (!obj.key) {
    nameObj = {
      key: key()
    }
  }
  // 需要对columns增加name属性的组件
  const needColumnsName: string | any[] = [
    'grid',
  ]

  if (needColumnsName.includes(obj.type)) {
    obj.columns.forEach((column: any) => {
      if (!column.key) {
        column.key = key()
      }
    });
  }
  Object.assign(obj, nameObj, objectItem)
  groupClick(obj)
}

const getGroupName = (item: any) => {
  if (item.key) {
    return item.key
  } else {
    return md5(JSON.stringify(item))
  }
}

const getGroupClass = (item: any) => {
  let res = {
    'mb5': type.value !== 5,
    'group': true,
    ['group-' + item.type]: true,
    active: activeKey.value === getGroupName(item),
    'group-layout': item.isLayout,
  };
  return res;
}

// 点击激活当前
const groupClick = (item: any, ele?: string) => {
  // 设计模式下才执行
  if (type.value !== 5) {
    return
  }
  if (ele) {
    item.type = ele
  }
  // 更新字段

  store.setActiveKey(getGroupName(item))
  store.setControlAttr(item)
  // grid时显示添加列按钮
  state.gridAdd = item.type === 'grid'
  state.clone = !notNested(item.type)
}
// 返回栅格宽度
const getFormItemStyle = (ele: FormList) => {
  let res = {};
  if (ele.config?.span === 0) {
    Object.assign(res, { width: 'auto', margin: '0 5px' })
  }
  if (ele.config && ele.config.span) {
    Object.assign(res, { width: (ele.config.span / 24) * 100 + '%' })
  }
  return res
}



//按钮点击事件
const injectBtnEvent = inject(constFormBtnEvent)
const clickBtn = (control: any) => {
  // 0: '提交表单',
  // 1: '重置表单',
  // 2: '取消返回',
  // 3: '无动作(自定义)'
  if (type.value !== 5) {
    // 非设计模式才触发事件
    if (injectBtnEvent && typeof injectBtnEvent === 'function') {
      injectBtnEvent(control);
    }
  }
}
onUnmounted(() => {
  // console.log('formGroup onUnmounted')
  // dataList.value = {}
  store.setActiveKey('')
  store.setControlAttr({})
})
</script>
