<template>
  <!-- 筛选 -->
  <el-card v-loading="paramsLoading" shadow="hover">
    <el-form ref="queryForm" :inline="true">
      <div v-if="showParamsComponent" class="yhh-search-collapse-style">
        <el-form-item v-for="field in primaryFilterParams" :key="field.Name" :label="field.Description">
          <filter-middleware :field="field" v-model="field.filters" />
        </el-form-item>

        <el-collapse v-if="expandSearch" v-model="collapseParam.activeName" @change="changeCollapseFun">
          <el-collapse-item name="1">
            <el-form-item v-for="field in filterParams" :key="field.Name" :label="field.Description">
              <template v-if="!field.default">
                <filter-middleware :field="field" v-model="field.filters" />
              </template>
            </el-form-item>

            <el-form-item class="select-primary-filter" label="设置默认条件">
              <el-select multiple collapse-tags @visible-change="setFilterParams" placeholder="请选择"
                v-model="primaryFields">
                <el-option v-for="item in fields" :key="item.Name" :label="item.Description" :value="item.Name" />
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
      </div>

      <div class="yahaha-action-bar">
        <div class="left">
          <el-button-group>
            <el-button type="primary" @click="openAdd"> 新增 </el-button>
            <el-button type="primary" @click="deleteRow()"> 删除 </el-button>
          </el-button-group>
          <div v-if="listSelectCount > 0" class="selected-count">
            <el-text>{{ listSelectCount }}已选</el-text>
          </div>
        </div>

        <el-button-group>
          <el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
          <el-button v-if="expandSearch" icon="ele-ZoomIn" @click="changeCollapseState"> {{ collapseParam.butName }}
          </el-button>
          <el-button icon="ele-Refresh" @click="cleanQueryValue"> 重置 </el-button>
        </el-button-group>
      </div>
    </el-form>
  </el-card>
  <!-- 列表 -->
  <el-card v-loading="loading" ref="mianList" class="yahaha-list full-table" shadow="hover" style="margin-top: 8px">

    <el-table ref="tableRef" :data="listValue" :key="mainListKey" tooltip-effect="light" row-key="id" :border="type === 5"
      @header-dragend="changeColWidth" @select-all="selectAllAction" @select="selectAction"
      style="width : 100%; height: 100%;">
      <el-table-column v-if="displayLineNumbers" align="center" type="index" fixed="left" />
      <el-table-column align="center" type="selection" fixed="left" />
      <!-- 根据字段配置渲染表格列 -->
      <el-table-column v-for="field in selectedFields" :key="field.Name" :prop="field.Name" :label="field.Description"
        :width="field.width" :fixed="field.fixed" align="center">
        <template #header>
          <el-tooltip :disabled="isEmptyRoNull(field.Help)" :content="field.Help" placement="top">
            <el-text>
              <el-icon v-if="!field.fixed && type === 5" class="move-icon cursor-pointer"><ele-Switch /></el-icon>
              {{ field.Description }}
            </el-text>
          </el-tooltip>
        </template>
        <template v-slot="scope">
          <component :is="curWidget(field.curWidget)" :widgetConfig="setCurrStatus(field, scope)"
            v-model="scope.row[field.Name]" />
        </template>
      </el-table-column>


      <!-- 创建操作列 -->
      <el-table-column label="操作" fixed="right" align="center" :resizable="false">
        <template v-slot="scope">
          <el-button type="primary" @click="Browse(scope.row)" text :disabled="type === 5">查看</el-button>
          <el-button type="primary" @click="deleteRow(scope.row)" text :disabled="type === 5">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-row>
      <el-col :span="12">
        <div style="margin: 10px 0 -10px 0 !important;">
          <el-button v-if="type !== 5" @click="enterDesign" type="primary" icon="ele-Operation" circle />
          <selectFields v-if="type === 5" v-model="selectedFields" :fields="fields"></selectFields>
          <el-button v-if="type === 5" @click="refreshCurrentTagsView" type="danger" icon="ele-CloseBold" circle />
          <el-button v-if="type === 5" @click="saveDesign" type="success" icon="ele-Select" circle />
        </div>
      </el-col>
      <!-- 分页栏 -->
      <el-col :span="12">
        <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
          :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background=""
          @size-change="handleSizeChange" @current-change="handleCurrentChange" :layout="paginationLayout" />
      </el-col>
    </el-row>

    <!-- 个性化组件 -->
    <component v-if="custComp && formDrawer.visible" :is="getComponent()" :desId="curRow.Id" :close="closeDrawer" />
    <!-- 需要弹窗 -->
    <el-dialog v-if="!custComp" v-model="formDrawer.visible" :fullscreen="false" title="1" width="80%" draggable>
      <formRenderer :form-data="designData.formData" :type="formType" />
    </el-dialog>

  </el-card>
</template>

<script setup lang="ts">
import { ref, nextTick, computed, watch } from 'vue';
import formRenderer from '/@/components/yahaha/design/form/components/formRenderer.vue'
import getWidget from '../widgets/getWidget'
import selectFields from './components/selectFields.vue';
import filterMiddleware from './filter/middleware.vue';
import * as api from '/@/api/model/';
import { userFilterSchemes } from '/@/api/visualDev';
import { useSysModel } from '/@/stores/sysModel';
import { useUserInfo } from '/@/stores/userInfo';
import type { FormList } from '/@/components/yahaha/design/types'
import { ElNotification } from 'element-plus'
import { fieldFilter } from '/@/api-services/models';
import { debounce } from 'lodash-es';
import Sortable from 'sortablejs';
import { isEmptyRoNull, jsonParseStringify, deepClone, readWidgetOptions, objToStringify } from '/@/components/yahaha/design/utils'
import { evaluateExpression, getFieldData } from '/@/components/yahaha/design/utils/applyFilter'
import mittBus from "/@/utils/mitt";
import router from '/@/router';
import { useRoute } from "vue-router";
const route = useRoute();
const props = defineProps({
  modelId: Number,//模型Id
  statusType: { //状态:1.查看,5.设计
    type: Number,
    default: 1,
  },
  formComp: Object,//自定义组件组件
  listConfig: Object,//列表配置
  formId: Number,//关联表单设计ID
  desId: Number,//列表设计ID
  userDesId: Number,//用户自定义配置ID
});
const formType = ref(0);
const designData = ref<any>({
  id: 0,
  formData: {}
});
const curRow = ref<any>({
  id: null,
});
const tableRef = ref();
const filterSchemes = ref<any>();
const primaryFilterParams = ref<fieldFilter[]>([]);
const filterParams = ref<fieldFilter[]>([]);
const listSelected = ref<any>([]);
const mianList = ref();
const loading = ref(false);
const showParamsComponent = ref(true);
const paramsLoading = ref(false);
const mainListKey = ref(0);
const collapseParam = ref({
  activeName: "",
  butName: "展开",
});
const formDrawer = ref({
  visible: false,
  editId: "",
});

const userinfo = ref(useUserInfo().userInfos);
const model = ref(useSysModel().getSysModelsById(props.modelId));
/**1：查看，5：设计 */
const type = ref(props.statusType)

const config = ref(props.listConfig?.config ?? {})

const custComp = computed(() => {
  return props.formComp ? true : false
});

const displayLineNumbers = computed(() => {
  return config.value?.displayLineNumbers ?? true;
});

const expandSearch = computed(() => {
  return config.value?.expandSearch ?? true;
});

const editable = computed(() => {
  return config.value?.editable ?? false;
});

const primaryFields = ref([] as string[]);


const queryParams = ref<any>
  ({
    model: 0,
    filters: ref<any>
  });

const tableParams = ref({
  page: 1,
  pageSize: config.value?.pageSize ?? 20,
  total: 0,
});

const getComponent = () => {
  return props.formComp as any;
};

const enterDesign = () => {
  listValue.value = emptyData();
  nextTick(() => {
    type.value = 5;
  })
}

const saveDesign = async () => {
  type.value = 1;
  const designData = getListConfig();
  const params = {
    model: useSysModel().getSysModels('UserListDesignScheme').Id,
    data: {
      UserName: userinfo.value.account,
      RelUser: { Id: userinfo.value.id, },
      ModelFull: model.value.FullName,
      RelModel: model.value,
      ListDesign: props.desId ? { Id: props.desId } : undefined,
      DesignData: objToStringify(designData),
      Default: true,
      Id: props.userDesId,
    }
  }
  await api.generalSave(params)
  fetchData();
}

const paginationLayout = computed(() => {
  if (type.value === 5) {
    return "sizes"
  } else {
    return "total, sizes, prev, pager, next, jumper"
  }
});

const editIndex = ref(0);

const curWidget = (name: string) => {
  //写的时候，组件的起名一定要与dragList中的element名字一模一样，不然会映射不上
  return getWidget[name]
}

const setCurrStatus = (item: any, scope: any) => {
  let temp = item;
  const isCur = scope.$index === editIndex.value;
  if (type.value !== 5) {
    temp = jsonParseStringify(item);
  }
  if (type.value === 1 || !editable.value) {
    temp.readonly = true; // 查看模式，为不可编辑状态
  } else if (isCur || type.value === 5) {// 是否当前行
    temp.readonly = false;
  } else {
    temp.readonly = true;
  }
  const value = scope.row[temp.Name];
  temp.validateReq = item.origRequired && !value;
  return temp
}

const setFieldStatus = (data: FormList[]) => {
  if (type.value === 5 || type.value === 3) { return; }
  data.forEach((it: FormList) => {
    if (it.Relate || it.Name === 'Id' || !editable.value) {
      it.origReadonly = true;
    }
    else if (it.readonlyExp) {
      it.origReadonly = evaluateExpression(listValue.value, it.requiredExp)
    }
    if (it.invisibleExp) {
      it.origInvisible = evaluateExpression(listValue.value, it.invisibleExp)
    }
    if (it.NotNull && it.Name !== 'Id') {
      it.origRequired = true
    }
    else if (it.requiredExp) {
      it.origRequired = evaluateExpression(listValue.value, it.requiredExp)
    }

    if (it.child && it.child.length > 0) {
      setFieldStatus(it.child)
    }
    if (it.list) {
      setFieldStatus(it.list)
    }
  })
}


const getDefaultWidget = (info: FormList) => {
  if ([null, undefined, 0, ''].includes(info.widget)) {
    const widget = readWidgetOptions()
    const filteredItem: any = widget.find(item => {
      return (!item.isLayout) &&
        (
          (item.fieldType.includes('*')) ||
          (item.fieldType.includes(info.tType as string))
        );
    });
    info.curWidget = filteredItem.name;
    if (!info.config) {
      info.config = {};
    }
    filteredItem?.options.forEach((item: any) => {
      if (!(item.key in info.config) && item.default) {
        item.value = item.default;
        info.config[item.key] = item.default;
      }
    })
  } else {
    info.curWidget = info.widget;
  }
}

const fields = computed<FormList[]>(() => {
  if (props.modelId) {
    const res: FormList[] = useSysModel().getSysFieldsByModelId(props.modelId).filter((item: any) => item.Description !== null && item.Description.trim() !== "");

    return res
  } else {
    return []
  }
})

const selectedFields = ref<any[]>(props.listConfig?.columns ?? deepClone(fields.value));

const listSelectedId = computed(() => {
  return listSelected.value.map((item: any) => item.Id)
});
const listSelectCount = computed(() => {
  return listSelected.value.length;
});

/**
 * 关闭表单组件
 */
const closeDrawer = () => {
  fetchData();
  formDrawer.value.visible = false;
};


const changeColWidth = (newWidth: number, oldWidth: number, column: any) => {
  let temp = selectedFields.value;
  temp.map((it: any) => {
    if (it.Name === column.property) {
      it.width = newWidth;
    }
    return it;
  })
  selectedFields.value = temp;
}

const selectAction = async (selection: any, row?: any) => {
  //是否单选
  let selected = selection.length && selection.indexOf(row) !== -1
  if (selected) {
    listSelected.value.push(row);
  } else {
    const indexToRemove: number = listSelectedId.value.indexOf(row.Id);
    if (indexToRemove !== -1) {
      // 使用 splice 删除元素
      listSelected.value.splice(indexToRemove, 1);
    }
  }
}

const selectAllAction = (selection: any) => {
  if (selection.length) {
    selection.forEach((it: any) => {
      listSelected.value.push(it);
    })
  } else {
    listValue.value.forEach((it: any) => {
      const indexToRemove: number = listSelectedId.value.indexOf(it.Id);
      if (indexToRemove !== -1) {
        // 使用 splice 删除元素
        listSelected.value.splice(indexToRemove, 1);
      }
    })
  }
}

const toggleSelection = () => {
  nextTick(() => {
    listSelectedId.value.forEach((id: any) => {
      const rowToSelect = listValue.value.find((row: any) => row.Id === id);
      if (rowToSelect) {
        tableRef.value!.toggleRowSelection(rowToSelect, true)
      }
    });
  });
}

const deleteRow = async (row?: any) => {
  loading.value = true;
  let ids = []
  if (row && row !== null && row !== undefined) {
    ids.push(row.Id)
  } else {
    ids = [...listSelectedId.value]
  }
  const params = {
    model: queryParams.value.model,
    ids: ids
  }
  await api.generalDelete(params);
  listSelected.value = []
  await fetchData();
};

const handleQuery = () => {
  fetchData();
};


//编辑行
const Browse = (row: any) => {
  curRow.value = row;
  if (custComp.value) {
    formType.value = 2;
    formDrawer.value.visible = true;
  } else {
    navRoute();
  }
};

const openAdd = () => {
  curRow.value.id = undefined;
  if (custComp.value) {
    formType.value = 1;
    formDrawer.value.visible = true;
  } else {
    navRoute();
  }
};

/**拖拽列 */
const columnDrop = () => {
  if (!tableRef.value) return
  const wrapperTr = tableRef.value!.$el.querySelector('.el-table__header-wrapper tr')
  Sortable.create(wrapperTr, {
    handle: '.move-icon',
    animation: 180,
    onEnd: (evt: any) => {
      let adjustIndex = displayLineNumbers.value ? 2 : 1;
      const oldIndex = evt.oldIndex;  // element's old index within old parent
      const newIndex = evt.newIndex - adjustIndex;  // element's new index within new parent
      if (oldIndex - adjustIndex === newIndex) return;
      let arr = selectedFields.value;
      const oldItem = arr[oldIndex - adjustIndex];
      let leftFixed = selectedFields.value.filter((it: any) => it.fixed === 'left').length;
      let rightFixed = selectedFields.value.filter((it: any) => it.fixed === 'right').length;
      const lastColumnIndex = selectedFields.value.length - rightFixed;
      if (leftFixed - 1 < newIndex && newIndex < lastColumnIndex) {
        arr.splice(oldIndex - adjustIndex, 1);
        arr.splice(newIndex!, 0, oldItem);
      } else {
        const index = oldIndex! + (oldIndex - adjustIndex > newIndex ? 1 : 0)
        wrapperTr!.insertBefore(evt.item, wrapperTr!.children[index]);
      }
      nextTick(() => {
        selectedFields.value = arr
      })
    }
  })
}



const navRoute = () => {
  formType.value = 1;
  router.push({
    name: 'form',
    query: { model: queryParams.value.model, form: props.formId, id: curRow.value.Id },
  })
}

const changeCollapseFun = (val: any) => {
  if (val.includes("1")) {
    collapseParam.value.butName = "收起";
  } else {
    collapseParam.value.butName = "展开";
  }
};

const changeCollapseState = () => {
  if (collapseParam.value.activeName === "") {
    collapseParam.value.activeName = "1";
    collapseParam.value.butName = "收起";
  } else {
    collapseParam.value.activeName = "";
    collapseParam.value.butName = "展开";
  }
};

const setFilterParams = async (visible: any) => {
  // 上传查询方案
  if (visible || type.value === 5) { return; }
  await saveUserFilterSchemesDebounce();
};

const cleanQueryValue = () => {
  refreshCurrentTagsView();
};
/**初始化查询字段 */
const createfilterParams = async (force: boolean = false) => {
  const filterFields = (items: any[], included: boolean) => {
    return items
      .filter((item) => item.Name && !item.Relate)
      .filter((item) => primaryFields.value.includes(item.Name) === included)
      .map((item: any) => ({
        id: item.Id,
        Description: item.Description,
        Name: item.Name,
        tType: item.tType,
        EnumValue: item.EnumValue,
        RelModel: item.RelModel,
      })) as fieldFilter[];
  };
  if (primaryFields.value.length === 0 || force) {
    //查询
    await getUserFilterSchemes();
    //区分
    primaryFilterParams.value = filterFields(fields.value, true);
    filterParams.value = filterFields(fields.value, false);
  }
};

const getUserFilterSchemes = async () => {
  const params = {
    sysModel: props.modelId,
    listDesign: props.desId ?? 0,
  }
  const res = await userFilterSchemes(params);
  filterSchemes.value = res.data.result;
  primaryFields.value = JSON.parse(filterSchemes.value?.DefaultFields ?? "[]");
}

const saveUserFilterSchemesDebounce = debounce(
  async function () {
    const params = {
      model: useSysModel().getSysModels('UserFilterScheme').Id,
      data: {
        UserName: userinfo.value.account,
        RelUser: { Id: userinfo.value.id, },
        ModelFull: model.value.FullName,
        RelModel: model.value,
        ListDesign: props.desId ? { Id: props.desId } : undefined,
        DefaultFields: JSON.stringify(primaryFields.value) as String,
        Default: true,
        Id: filterSchemes.value?.Id ?? 0,
      }
    }
    var res = await api.generalSave(params)
    if (res.status === 200) {
      ElNotification({
        title: '成功',
        message: ('查询方案已保存'),
        type: 'success',
      })
      createfilterParams(true);
    }
  },
  1000
);

const refreshCurrentTagsView = () => {
  mittBus.emit(
    "onCurrentContextmenuClick",
    Object.assign({}, { contextMenuClickId: 0, ...route })
  );
};

/**组合查询条件 */
const compFilterParams = () => {
  console.log([...primaryFilterParams.value, ...filterParams.value]);
  queryParams.value.filters = [...primaryFilterParams.value, ...filterParams.value]
    .map(item => {

      if (Array.isArray(item.filters)) {
        item.filters = item.filters.map(filter => {
          if (typeof filter === 'object' && 'filterExp' in filter) {
            return typeof filter.filterExp === 'string' ? JSON.parse(filter.filterExp) : filter.filterExp;
          } else if (typeof filter === 'string' && filter.trim() !== '') {
            return JSON.parse(filter);
          }
          return filter;
        });
      }

      return item;
    })
    .filter(item => item.filters !== undefined && item.filters !== null);
};

const emptyData = () => {
  let data: any[] = [];
  const result: { [key: string]: null } = {};
  if (fields.value.length > 0) {
    fields.value.forEach((item: any) => {
      result[item.Name] = null;
    });
  }
  data.push(result);
  return data;
}

const listValue = ref<any[]>([]);

const fetchData = async () => {
  // 设置状态
  setFieldStatus(selectedFields.value);
  selectedFields.value.forEach(it => {
    getDefaultWidget(it)
  });
  // 当设计模式下获取空值
  if (type.value === 5) {
    listValue.value = emptyData();
    return;
  }
  // 开始查询相关
  loading.value = true;
  queryParams.value.model = props.modelId;
  await createfilterParams();
  compFilterParams();
  console.log(queryParams.value);
  var res = await api.generalListData(Object.assign(queryParams.value, tableParams.value));
  listValue.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;

  loading.value = false;
  mainListKey.value++;
  toggleSelection()
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
  tableParams.value.page = val;
  fetchData();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
  tableParams.value.pageSize = val;
  fetchData();
};

fetchData();

/**只保留有效的设置属性 */
const filterObjectByPaths = (obj: any) => {
  let pathList: any[] = ['Name', 'width', 'fixed'];//需要保存的值key
  const widgetOptions = readWidgetOptions();
  const curWidgetOptions = widgetOptions.find((item: { name: any; }) => item.name === obj.curWidget);
  if (curWidgetOptions) {
    curWidgetOptions.options.forEach((item: any) => {
      if (item.path) {
        pathList.push(item.path);
      }
      if (item.key) {
        pathList.push(item.key);
      }
    })
  } else {
    console.log(obj);
    throw new Error('未匹配到有效组件');
  }

  const filteredObject: any = {};

  pathList.forEach(path => {
    const value = getFieldData(obj, path);
    if (value) {
      filteredObject[path] = value;
    }
  });
  // 如果存在 child 属性，则递归调用 filterObjectByPaths 函数进行额外过滤
  if (obj.child) {
    filteredObject.child = obj.child.map((item: any) => filterObjectByPaths(item));
  }

  return filteredObject;
};

const getListConfig = () => {
  const tempSelcted = selectedFields.value.map((it: FormList) => {
    return filterObjectByPaths(it);
  })
  let temp = {
    config: { ...config.value, pageSize: tableParams.value.pageSize },

    columns: [...tempSelcted]
  }
  return temp
}

watch(
  () => selectedFields.value,
  (nval, oval) => {
    if (nval.length !== oval?.length) {
      mainListKey.value++;
      nextTick(() => {
        columnDrop();
      })
    }
  },
  {
    deep: true, immediate: true
  }
);

defineExpose({
  getListConfig,
})

</script>

<style lang="scss">
.yahaha-list {
  width: 100%;
  /* 使用视窗宽度的80%作为组件的宽度 */
  height: calc(100% - 85px);
  /* 使用视窗高度的80%作为组件的高度 */

}

.yahaha-action-bar {
  margin: 10px 0px 0px 0px;
  display: flex;
  justify-content: space-between;

  .left {
    display: flex;
  }

  .selected-count {
    margin: 0px 0px 0px 10px;
    border: 1px solid var(--el-color-primary);
    background-color: var(--el-color-primary-light-9);
    border-radius: 3px;
    display: flex;
    padding: 3px;
  }
}

.yhh-search-collapse-style {

  .el-collapse-item__content {
    padding: 10px 0px;
  }

  .el-form-item {
    min-width: 29%;
    margin-bottom: 9px;
  }

  // .el-form-item--default {
  //   width: 30%;
  //   margin-bottom: 9px;
  // }

  // .el-form-item--default {
  //   width: 30%;
  //   margin-bottom: 9px;
  // }

  .el-collapse-item__header {
    height: 0px;
  }

  .el-collapse-item__arrow {
    display: none
  }

  .select-primary-filter {
    .el-form-item__label {
      color: var(--el-color-primary);
    }
  }
}
</style>

