<template>
  <el-card v-loading="paramsLoading" shadow="hover">
    <el-form ref="queryForm" :inline="true">
      <div v-if="showParamsComponent" class="yhh-search-collapse-style">
        <el-form-item v-for="field in primaryFilterParams" :key="field.name" :label="field.description">
          <template v-if="!field.default">
            <y-search-item :field="field" v-model="field.filters" />
          </template>
        </el-form-item>
        <el-collapse v-model="collapseParam.activeName">
          <el-collapse-item name="1">
            <el-form-item v-for="field in filterParams" :key="field.name" :label="field.description">
              <template v-if="!field.default">
                <y-search-item :field="field" v-model="field.filters" />
              </template>
            </el-form-item>

            <el-form-item class="select-primary-filter" label="设置默认条件">
              <el-select multiple collapse-tags @visible-change="setFilterParams" placeholder="请选择"
                v-model="primaryFields">
                <el-option v-for="item in fields" :key="item.name" :label="item.description" :value="item.name" />
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
      </div>

      <div class="yahaha-action-bar">
        <div class="left">
          <el-button-group>
            <el-button type="primary" @click="openAdd"> 新增 </el-button>
            <el-button type="primary" @click="deleteRow()" v-auth="'inventory:add'"> 删除 </el-button>
          </el-button-group>
          <div v-if="listSelectCount > 0" class="selected-count">
            <el-text>{{ listSelectCount }}已选</el-text>
          </div>
        </div>

        <el-button-group>
          <el-button type="primary" icon="ele-Search" @click="handleQuery" > 查询 </el-button>
          <el-button icon="ele-ZoomIn" @click="changeCollapseState" > {{ collapseParam.butName
          }} </el-button>
          <el-button icon="ele-Refresh" @click="cleanQueryValue"> 重置 </el-button>
        </el-button-group>
      </div>
    </el-form>
  </el-card>
  <el-card v-loading="loading" ref="y-list" class="yahaha-list full-table" shadow="hover" style="margin-top: 8px">

    <el-table ref="tableRef" :data="tableData" :key="mainListKey" tooltip-effect="light" row-key="id"
      @select-all="selectAllAction" @select="selectAction" style="width : 100%; height: 100%;">
      <el-table-column align="center" type="selection" />
      <!-- 根据字段配置渲染表格列 -->
      <el-table-column v-for="field in fields" :key="field.name" :prop="field.name" :label="field.description">
        <template v-slot="scope">
          <!-- <el-input v-model.lazy="scope.row.id" /> -->
          <yColumn :field="field" v-model="scope.row" />
        </template>
      </el-table-column>


      <!-- 创建操作列 -->
      <el-table-column label="操作" fixed="right" align="center" v-if="showActions()">
        <template v-slot="scope">
          <el-button type="primary" @click="Browse(scope.row)" text>查看</el-button>
          <el-button type="primary" @click="deleteRow(scope.row)" text>删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
      :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background="" @size-change="handleSizeChange"
      @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />

    <!-- 个性化组件 -->
    <component v-if="custComp && formDrawer.visible" :is="getComponent()" :desId="curRow.Id" :close="closeDrawer" />
    <!-- 需要弹窗 -->
    <el-dialog v-if="!custComp" v-model="formDrawer.visible" :fullscreen="false" title="1" width="80%" draggable>
      <formRenderer :form-data="designData.formData" :type="formType" />
    </el-dialog>

  </el-card>
</template>

<script setup lang="ts">
import { onMounted, PropType, ref, nextTick, computed, toRaw } from 'vue';
import formRenderer from './design/form/components/formRenderer.vue'
import yColumn from './yColumn.vue';
import ySearchItem from './search/ySearchItem.vue';
import * as api from '/@/api/model/';
import { useSysModel } from '/@/stores/sysModel';
import { useVisualDev } from '/@/stores/visualDev';
import { ElNotification } from 'element-plus'
import { SysFields, fieldFilter, userFilterSchemes } from '/@/api-services/models';
import { debounce } from 'lodash-es';
import { stringToObj } from '/@/components/yahaha/design/utils/form'
import router from '/@/router';

const props = defineProps({
  model: {
    type: String,
    required: false,
  },
  design: {
    type: Number,
    required: false,
  },
  create: Boolean as PropType<boolean>,
  edit: Boolean as PropType<boolean>,
  del: Boolean as PropType<boolean>,
  formComp: {
    type: Object,
    required: false,
  },
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
const tableData = ref<any>([]);
const fields = ref<SysFields[]>([]);
const filterSchemes = ref<userFilterSchemes[]>([]);
const primaryFilterParams = ref<fieldFilter[]>([]);
const filterParams = ref<fieldFilter[]>([]);
const listSelected = ref<any>([]);
const loading = ref(false);
const showParamsComponent = ref(true);
const paramsLoading = ref(false);
const mainListKey = ref(0);
const custComp = ref(false);
const collapseParam = ref({
  activeName: "",
  butName: "更多",
});
const formDrawer = ref({
  visible: false,
  editId: "",
});
const primaryFields = ref([] as string[]);

const queryParams = ref<any>
  ({
    model: 0,
    filters: ref<any>
  });
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
});

const getComponent = () => {
  return props.formComp as any;
};


const listSelectedId = computed(() => {
  return listSelected.value.map((item: any) => item.Id)
});
const listSelectCount = computed(() => {
  return listSelected.value.length;
});

/**
 * 读取表单设计数据
 */
const getModel = async () => {
  if (props.design) {
    // 根据设计获取
    // console.log('visualDevList',visualDevList)
    // stores.setVisualDevList;
    const res = useVisualDev().getVisualDev(props.design)
    designData.value.id = props.design;
    designData.value.formData = stringToObj(res.formData);
    queryParams.value.model = res.modelId;
  } else {
    const res = useSysModel().getSysModels(props.model);
    console.log(res)
    queryParams.value.model = res.Id;
  }
};

/**
 * 关闭表单组件
 */
const closeDrawer = () => {
  fetchData();
  formDrawer.value.visible = false;
};


const selectAction = async (selection: any, row?: any) => {
  //console.log('selection', selection, 'row', row, 'selected', selected)

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
    tableData.value.forEach((it: any) => {
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
      const rowToSelect = tableData.value.find((row: any) => row.Id === id);
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
  curRow.value.Id = undefined;
  curRow.value.id = undefined;
  if (custComp.value) {
    formType.value = 1;
    formDrawer.value.visible = true;
  } else {
    navRoute();
  }
};

const navRoute = () => {
  formType.value = 1;
  router.push({
    name: 'form',
    query: { visualDev: props.design, id: curRow.value.Id },
    state: { title: "xxx" }
  })
}

const changeCollapseState = () => {
  if (collapseParam.value.activeName === "") {
    collapseParam.value.activeName = "1";
    collapseParam.value.butName = "收起";
  } else {
    collapseParam.value.activeName = "";
    collapseParam.value.butName = "更多";
  }
};

const createfilterParams = () => {
  // 创建筛选字段信息

  primaryFilterParams.value = fields.value.filter((item) => primaryFields.value.includes(item.name)).map((item: any) => ({
    id: item.id,
    description: item.description,
    name: item.name,
    tType: item.tType,
  })) as fieldFilter[];
  filterParams.value = fields.value.filter((item) => !primaryFields.value.includes(item.name)).map((item: any) => ({
    id: item.id,
    description: item.description,
    name: item.name,
    tType: item.tType,
  })) as fieldFilter[];
};

const setFilterParams = async (visible: any) => {
  // 上传查询方案
  if (visible) { return; }
  createfilterParams();
  await createUserFilterSchemesDebounce();
};

const cleanQueryValue = () => {
  paramsLoading.value = true;
  loading.value = true;
  createfilterParams();

  showParamsComponent.value = false;
  setTimeout(async () => {
    await fetchData();
    showParamsComponent.value = true;
    paramsLoading.value = false;
  }, 100); // 使用setTimeout来触发重新加载，确保Vue能够正确处理更新
};

const createUserFilterSchemesDebounce = debounce(
  async function () {
    var id = 0;
    if (filterSchemes.value.length > 0) {
      id = filterSchemes.value[0].Id;
    }
    var params = {
      id: id,
      name: "默认",
      tableName: props.model,
      modelId: queryParams.value.model,
      defaultFields: JSON.stringify(primaryFields.value) as String,
    }
    var res = await api.createUserFilterSchemes(params);
    if (res.status === 200) {
      ElNotification({
        title: '成功',
        message: ('查询方案已保存'),
        type: 'success',
      })
    }
  },
  1000
);

const showActions = () => {
  return props.create || props.edit || props.del;
};
//组合查询条件
const compParams = () => {
  const combinedFilters = [...primaryFilterParams.value, ...filterParams.value];
  combinedFilters.forEach((item: any) => {
    if (item.filters !== null && item.filters !== undefined) {
      let tempFilters = item.filters;
      if (typeof tempFilters === 'string') {
        tempFilters = [tempFilters];
      }
      item.filters = tempFilters.map((str: any) => {
        if (typeof str === 'string' && str.length > 0) {
          return JSON.parse(str);
        }
        return str; // 如果已经是对象，直接返回
      });
    }
  });
  queryParams.value.filters = combinedFilters;
};

const fetchData = async () => {
  loading.value = true;
  await getModel();
  compParams();

  var res = await api.generalListData(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  fields.value = res.data.result?.fields.filter((item: any) => item.description !== null && item.description.trim() !== "" && item.navigatType == null)
    .map((item: any) => ({
      ...item as SysFields,
    })) as SysFields[];
  // 读取默认查询字段
  filterSchemes.value = res.data.result?.userFilterSchemes;
  if (filterSchemes.value.length > 0) {
    var defaultuserFilterScheme = toRaw(filterSchemes.value[filterSchemes.value.length - 1]);
    primaryFields.value = JSON.parse(defaultuserFilterScheme.DefaultFields);
  }
  loading.value = false;
  mainListKey.value++;
  // 初始化查询条件
  if (filterParams.value.length === 0) {
    createfilterParams();
  }
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

onMounted(async () => {
  if (props.formComp) {
    custComp.value = true;
  }
  fetchData();

});

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

  .el-form-item--small {
    margin-bottom: 9px;

  }

  .el-form-item--default {
    margin-bottom: 9px;
  }

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

