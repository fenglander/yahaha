<template>
  <el-drawer v-model="showDrawer" :append-to-body="true" :destroy-on-close="true" size="100%" :show-close="false"
    @closed="closeDialog">
    <template #header="{ titleId, titleClass }">
      <h4 :id="titleId" :class="titleClass">列表设计</h4>
      <head-tools :showKey="['Close', 'TopLeft', 'Collection']" @click="headToolClick" />
    </template>
    <div class="design-container design-table" v-loading="state.loading" ref="container">
      <div class="main-body">

        <div class="main-form main-table">
          <list-renderer ref="rendererRef" :statusType="5" :model-id="state.modelId"
            :listConfig="state.listConfig"></list-renderer>
        </div>
      </div>
      <div class="sidebar-tools">
        <el-form size="small" class="form">
          <el-tabs v-model="state.tabsName">
            <el-tab-pane label="字段属性" name="first">
              <div>
                <div class="h3">
                  个性化设置
                </div>
              </div>
            </el-tab-pane>
            <el-tab-pane label="数据列表配置" name="second">
              <el-form-item v-for="(item, index) in tableListAttr" :label="item.label" :key="index">
                <el-select v-if="item.type === 'select'" v-model="item.value" :placeholder="item.placeholder"
                  :clearable="true" @change="tableListAttrChange(item, $event)">
                  <el-option :label="opt.label || opt.name" v-for="opt in item.options" :key="opt.label || opt.name"
                    :value="formatNumber(opt.value ?? opt.id)" />
                </el-select>
                <el-switch v-else-if="item.type === 'switch'" v-model="item.value"
                  @input="tableListAttrChange(item, $event)" />
                <el-input v-else v-model="item.value" :placeholder="item.placeholder"
                  @input="tableListAttrChange(item, $event)" :readonly="item.readonly" />
              </el-form-item>
              <el-form-item class="event-btn">
                <el-button @click="editOpenDrawer('FilterCriteria')">筛选条件
                </el-button>
                <el-button @click="editOpenDrawer('afterResponse')">afterResponse
                </el-button>
                <el-button @click="editOpenDrawer('beforeDelete')">beforeDelete
                </el-button>
              </el-form-item>
            </el-tab-pane>
          </el-tabs>
        </el-form>
      </div>
    </div>
    <ace-drawer v-model="drawer.visible" :title="drawer.title" direction="rtl" :content="drawer.content"
      :code-type="drawer.codeType" @before-close="drawerBeforeClose" />
    <!-- <vue-file ref="vueFileEl" /> -->
    <el-dialog v-model="showSelectModel" :close-on-click-modal="false" title="选择模型" :show-close=false>
      <select-model v-model="state.sysModel" @clost="props.close"></select-model>
      <template #footer>
        <span>
          <el-button @click="showDrawer = false">取消</el-button>
          <el-button type="primary" @click="confirmationModel">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </el-drawer>
</template>
<script setup lang="ts">
import { reactive, ref, computed } from 'vue'
import HeadTools from '/@/components/yahaha/design/components/headTools.vue'
import aceDrawer from '/@/components/yahaha/design/components/aceDrawer.vue'
import listRenderer from '/@/components/yahaha/design/list/listRenderer.vue'
import { listAttr } from '/@/components/yahaha/design/list/listAttr'
import { json2string, objToStringify, stringToObj } from '/@/components/yahaha/design/utils/'
import selectModel from './selectModel.vue'
import { getFieldData, applyFilter } from '/@/components/yahaha/design/utils/applyFilter'
import { beforeRequest, afterResponse, formatNumber, keepOnlyId, hasKey } from '/@/components/yahaha/design/utils/'
import { saveListDesgin } from '/@/api/visualDev';
// import type { FormList } from '/@/components/yahaha/design/types'
// import { useRouter, useRoute } from 'vue-router'
import { useLayoutStore } from '/@/stores/layout'
import { useVisualDev } from '/@/stores/visualDev';
const layoutStore = useLayoutStore()

const props = defineProps({
  desId: {
    type: Number,
    required: false,
  },
  close: {
    type: Function,
    default: () => { },
  },

});

layoutStore.changeBreadcrumb([{ label: '设计管理' }, { label: '列表页设计' }])
const vueFileEl = ref()
const container = ref()
const rendererRef = ref()
const state = reactive({
  listConfig: {
    // tableProps: {}, //表格所有参数
    columns: [],
    config: {} as any
  },
  searchData: {},
  loading: false,
  modelId: 0 as number,//模型ID
  modelName: '' as string,//模型名称
  modelFullName: '' as string, //类型全称
  sysModel: undefined as any,
  desId: 0 as number, //标识
  name: '',
  treeData: {}, // 左侧树相关
  tabsName: 'second',
  refreshTable: true,
  CreateUser: undefined as any,
  CreateTime: undefined as any,
})
const drawer = reactive({
  visible: false,
  title: '',
  direction: '',
  content: '',
  codeType: ''
})

const showDrawer = ref(true);
const showSelectModel = ref(false);
const closeDialog = () => {
  props.close();
};
const tableListAttr = computed(() => {
  let temp: any[] = [];
  listAttr.forEach((it: any) => {
    if (!hasKey(state, it.path) && it.default) {
      getPropByPath(state, it.path, it.default)
    }
    let isShow = true;
    if (it.show) {
      isShow = applyFilter(state, it.show)
    }
    if (isShow) {
      temp.push({
        ...it,
        value: getFieldData(state, it.path) || null, // 设定值
      })
    }
  })
  return temp;
})

const tableListAttrChange = (obj: any, val?: any) => {
  if (obj.path) {
    const newVal = obj.isNum ? formatNumber(val) : val // 类型为数字时转整数
    obj.path && getPropByPath(state, obj.path, newVal)
  }
}
const confirmationModel = () => {
  state.modelId = state.sysModel.Id;
  state.modelName = state.sysModel.Name;
  state.modelFullName = state.sysModel.FullName;
  state.name = state.sysModel.Description;
  state.sysModel = keepOnlyId(state.sysModel)
  showSelectModel.value = false;
};


// 修改指定路径下的值
const getPropByPath = (obj: any, path: string, val: any) => {
  let tempObj = obj
  const keyArr = path.split('.')
  let i = 0
  for (i; i < keyArr.length - 1; i++) {
    const key = keyArr[i]
    if (key in tempObj) {
      tempObj = tempObj[key]
    } else {
      throw new Error(`${key} is undefined`)
      // break
    }
  }
  const key = keyArr[i]
  // 检查最后一级是否存在
  /*if (!(key in tempObj)) {
  throw new Error(`${key} is undefined`)
}*/
  tempObj[key] = val
}

const headToolClick = (type: string) => {
  switch (type) {
    case 'del':
      // 清空
      state.listConfig.columns = []
      break
    case 'json':
      // 生成脚本
      dialogOpen(state.listConfig, { direction: 'rtl', type: 'json' })
      break
    case 'vue':
      // 导出vue文件
      vueFileEl.value.openTable(state.listConfig)
      break
    case 'Collection':
      // 保存
      saveData()
      break
  }
}

const editOpenDrawer = (type: string) => {
  switch (type) {
    case 'FilterCriteria':
      // eslint-disable-next-line no-case-declarations
      const newData = state.listConfig.config || {}
      dialogOpen(newData[type], { type: type, title: '设置列表的默认筛选条件,处理相同对象存在不同业务界面的情况。格式：[[\'Code\', \'==\', false],\'or\',[\'qty\', \'>\', 0],[CreateUser, \'=\',userId]]  生成的SQL Code == false and (qty>0 or CreateUser = 13000000001) ' })
      break
  }
}

const dialogOpen = (obj: any, params: any = {}) => {
  drawer.visible = true
  Object.assign(drawer, { direction: 'ltr' }, params)
  let editData = objToStringify(obj, true)
  switch (params.type) {
    case 'FilterCriteria':
      editData = json2string(obj, true)
      break
    case 'beforeRequest':
    case 'beforeDelete':
    case 'treeBeforeRequest':
      if (!obj) {
        editData = beforeRequest
      }
      break
    case 'afterResponse':
    case 'treeAfterResponse':
      if (!obj) {
        editData = afterResponse
      }
      break
  }
  drawer.content = editData
}

const drawerBeforeClose = () => {
  drawer.visible = false
  drawer.content = ''
  drawer.codeType = ''
  drawer.title = ''
}

const saveData = async () => {
  state.listConfig = rendererRef.value.getListConfig();
  let params: any = {
    ColumnData: objToStringify(state.listConfig),
    FullName: state.name, // 表单名称，用于在显示所有已创建的表单列表里显示
    ModelId: state.modelId,
    modelName: state.modelName, // 数据源允许在表单属性设置里修改的
    modelFullName: state.modelFullName, // 数据源允许在表单属性设置里修改的
    SysModel: state.sysModel,
    Id: state.desId,
  }
  state.loading = true;
  await saveListDesgin(params).then((res: any) => {
    state.desId = res.data.result ?? 0;
  }).catch(() => {
  })
  // 初始化设计缓存
  await useVisualDev().setListDesginList();
  state.loading = false;
  showDrawer.value = false;
}

// 修改时获取初始数据
const getInitData = () => {
  showDrawer.value = true;
  if (props.desId) {
    const id: number = props.desId;// 当前记录保存的id
    // 获取初始表单数据
    state.loading = true;
    const result = useVisualDev().getlistDesgin(id);
    if (result) {
      state.listConfig = stringToObj(result.ColumnData);
      state.name = result.FullName;
      state.modelId = result.SysModel.Id;
      state.sysModel = result.SysModel;
      state.modelName = result.SysModel.Name;
      state.modelFullName = result.SysModel.FullName;
      state.desId = id;
      state.CreateUser = result.CreateUser;
      state.CreateTime = result.CreateTime;
    }
    state.loading = false;
  } else {
    showSelectModel.value = true;
  }
}
// 数据相关结束
getInitData()
</script>
