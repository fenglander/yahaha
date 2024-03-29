<template>
  <el-card v-loading="loading" class="yahaha-form" shadow="hover">
    <template #header>
      <div class="card-header">
        <span class="title">{{ title }}</span>
        <el-space>
          <el-button-group>
            <el-button v-for="fun in functionList" @click="executeFunc(fun)" :key="fun.Id"> {{ fun.Name }}
            </el-button>
          </el-button-group>

          <el-button-group>
            <!-- <el-button type="danger" @click="validate()"> 校验 </el-button> -->
            <el-button type="primary" v-if="actionType === 3" @click="editFun"> 编辑 </el-button>
            <el-button type="primary" v-if="actionType === 1 || actionType === 2" @click="saveFun"> 保存 </el-button>
            <el-button v-if="actionType === 3" @click="createFun"> 新建 </el-button>
            <el-button v-if="actionType === 2" @click="cancelFun"> 取消 </el-button>
            <el-button v-if="id" @click="deleteFun"> 删除 </el-button>
          </el-button-group>
        </el-space>
      </div>
    </template>
    <div>
      <formRenderer ref="rendererRef" v-if="loadingComplete" :form-data="designData.formData" :type="actionType"
        :value="tempDataRecs" @change="setDataRecs($event)" />
    </div>
  </el-card>
</template>

<script setup lang="ts" name="generalFormView">
import { ref, reactive, computed, watch } from 'vue';
import { ElMessage } from 'element-plus'
import { useRouter, useRoute } from 'vue-router';
import { stringToObj } from '/@/components/yahaha/design/utils/form';
import { deepClone, key, formatNumber } from '/@/components/yahaha/design/utils'
import pinia from '/@/stores/index';
import mittBus from "/@/utils/mitt";
import { debounce } from 'lodash-es';
import formRenderer from '/@/components/yahaha/design/form/components/formRenderer.vue'
import { useVisualDev } from '/@/stores/visualDev';
import { useSysModel } from '/@/stores/sysModel';
import * as api from '/@/api/model/';
import type { FormData, FormList } from '/@/components/yahaha/design/types';
const visualDevStores = useVisualDev(pinia);

const router = useRouter();
const route = useRoute();
const query = reactive(router.currentRoute.value.query);
const currentRoute = reactive(router.currentRoute.value);
const rendererRef = ref();
const id = ref<any>();
const tempDataRecs = ref<any>();
const dataRecs = ref<any>();
const loadingComplete = ref(false);
const loading = ref(false);
// actionType:1新增；2修改；3查看；5设计
const actionType = ref<number>(3);

const visualDev = computed(() => {
  if (query) {
    return visualDevStores.getFormDesgin(query.visualDev);
  } else {
    return null
  }
})

const fields = computed(() => {
  const res = useSysModel().getSysFieldsByModelId(sysModel.value.Id);
  return res;
})

const sysModel = computed(() => {
  if (query) {
    return useSysModel().getSysModelsById(formatNumber(query.model));
  } else {
    return null
  }
})

const title = ref<any>();

const getFormTitle = async (modelId: any, newVal: any) => {
  const params = {
    model: modelId,
    data: newVal
  }
  const res = await api.getModelTitle(params);
  return res.data.result;
}

const functionList = computed(() => {
  if (visualDev.value) {
    return useSysModel().getSysActionById(sysModel.value.Id).filter((item: any) => item.Function);
  } else {
    return [];
  }
})

const designData = computed(() => {
  //const methods: any = item.methods
  let res: { formData: FormData } = {
    formData: {
      list: [],
      form: undefined,
    },
  }
  if (visualDev.value) {
    res.formData = stringToObj(visualDev.value.FormData);
    res.formData.modelId = visualDev.value.ModelId;
  } else {
    const tempForm = {
      openMethod: 1,
      modelId: sysModel.value.Id,
      modelName: sysModel.value.Name,
      fullName: sysModel.value.Description,
      sysModel: {
        Id: sysModel.value.Id
      }
    }
    let tempList: FormList[] = []
    fields.value.forEach((it: any) => {
      tempList.push({
        fieldName: it.Name,
        label: it.Description,
        key: key(),
        control: undefined,
        config: undefined,
        child: it.tType === 'OneToMany' ? it.SubFields.filter((item: any) => item.Name !== it.Related) : undefined
      })
    })
    res.formData.form = tempForm;
    res.formData.list = tempList;
    console.log('自动生成表单设计', res.formData)
  }
  return res;
})

const editFun = async () => {
  actionType.value = 2;
}

const validate = () => {
  const res = rendererRef.value.validate()
  console.log(res);
  if (res && res.length > 0) {
    const detailStr = res.map((it: any) => {
      return '栏目:' + it.Lable + (it.Indexes && it.Indexes.length > 0 ? ',行号:' + it.Indexes.join(',') : ';');
    });
    const msgStr: string = '<p>' + detailStr.join('</p><p>') + '</p>';
    ElMessage({
      showClose: true,
      dangerouslyUseHTMLString: true,
      message: '<strong>尚有必填项未处理</strong><p>请检查：</p>' + msgStr,
      type: 'error',
    })
    return false
  } else {
    return true
  }
}

const saveFun = async () => {
  loading.value = true;
  if (!validate()) {
    loading.value = false;
    return;
  }
  const params = {
    model: sysModel.value.Id,
    data: dataRecs.value
  }
  const res = await api.generalSave(params);
  id.value = res.data.result;
  ElMessage({
    message: '保存成功',
    type: 'success',
  })
  Refresh();
}

console

const cancelFun = async () => {
  getDataRecs();
}

const createFun = () => {
  router.replace({
    path: currentRoute.path,
    query: { model: query.model, visualDev: query.visualDev },
  });
  mittBus.emit('onCurrentContextmenuClick', Object.assign({}, { contextMenuClickId: 1, ...route }));
}

const executeFunc = async (fun: any) => {
  const params = {
    moduleName: fun.ActionModuleName,
    className: fun.ActionClassName,
    methodName: fun.ActionName,
    model: fun.BindingModel,
    data: dataRecs.value,
  }
  const res = await api.generalExecFunc(params);
  if (res.status === 200) {
    const rec = res.data.result.Data[0]  // 这是表单，所以只会有一行
    tempDataRecs.value = deepClone(rec);
    dataRecs.value = tempDataRecs.value;
  }
  // 获取处理后的数据
  // 获取处理后的结果
}

const deleteFun = async () => {
  if (dataRecs.value.Id) {
    let ids = [dataRecs.value.Id]
    const params = {
      model: sysModel.value.Id,
      ids: ids
    }
    const res = await api.generalDelete(params)
    if (res.status === 200) {
      ElMessage({
        message: ('删除成功'),
        type: 'success',
      })
      mittBus.emit('onCurrentContextmenuClick', Object.assign({}, { contextMenuClickId: 1, ...route }));
    }
  }
}

const Refresh = () => {
  loading.value = false;
  if (!query.id && id.value) {
    mittBus.emit('onCurrentContextmenuClick', Object.assign({}, { contextMenuClickId: 1, ...route }));
    router.replace({
      path: currentRoute.path,
      query: { model: query.model, visualDev: query.visualDev, id: id.value },
    });
  } else {
    getDataRecs();
  }
}

const setDataRecs = (vals: any) => {
  dataRecs.value = vals.model;
}

const getDataRecs = async () => {
  id.value = query.id;
  loading.value = true;
  if (id.value) {
    actionType.value = 3;
    let res = await api.generalFormData({ 'model': sysModel.value.Id, 'id': id.value ?? 0 });
    if (res && res.status === 200 && res.data.result) {
      tempDataRecs.value = deepClone(res.data.result)
    }
  } else {

    let temp: any = {}
    fields.value.forEach((it: any) => {
      temp[it.Name] = null;
    });
    tempDataRecs.value = deepClone(temp);
    actionType.value = 1;
  }
  dataRecs.value = { ...tempDataRecs.value }
  loading.value = false;
  loadingComplete.value = true;
}
getDataRecs()

watch(
  () => dataRecs.value,
  async (newVal) => {
    if (id.value) {
      title.value = await getFormTitle(sysModel.value.Id, newVal);
    } else {
      title.value = '新建';
    }
  },
  {
    deep: true
  }
);

</script>

<style lang="scss" scoped>
.yahaha-form {

  width: 100%;
  height: 100%;

  :deep(.el-card__header) {
    padding: 10px 20px;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
  }

  :deep(.el-card__body) {
    overflow-y: auto;
  }

  .title {
    color: var(--el-color-primary);
    font-weight: bold;
    font-size: 18px;
    /* margin-bottom: 10px; */
  }

  .body {
    padding: 8px;
    background-color: var(--el-fill-color-blank);
    border-bottom-left-radius: 4px;
    border-bottom-right-radius: 4px;
  }
}
</style>: any: any(: any)