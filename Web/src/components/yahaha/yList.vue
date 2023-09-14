<template>
  <el-card v-loading="paramsLoading" shadow="hover" :body-style="{ padding: '10px 10px' }">
    <el-form ref="queryForm" :inline="true">
      <div class="yhh-search-collapse-style">
        <el-form-item v-if="showParamsComponent" v-for="field in primaryFilterParams" :key="field.name"
          :label="field.description">
          <template v-if="!field.default">
            <y-search-item :field="field" v-model="field.filters" />
          </template>
        </el-form-item>
        <el-collapse v-model="collapseParam.activeName">
          <el-collapse-item name="1">
            <el-form-item v-if="showParamsComponent" v-for="field in filterParams" :key="field.name"
              :label="field.description">
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


      <div style="margin: 10px 0px 0px 0px;">
        <el-form-item>
          <el-button-group>
            <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'inventory:page'"> 查询 </el-button>
            <el-button icon="ele-ZoomIn" @click="changeCollapseState" v-auth="'inventory:page'"> {{ collapseParam.butName
            }} </el-button>
            <el-button icon="ele-Refresh" @click="cleanQueryValue"> 重置 </el-button>
          </el-button-group>
        </el-form-item>
      </div>
    </el-form>
  </el-card>
  <el-card v-loading="loading" ref="y-list" class="yahaha-list full-table" shadow="hover" style="margin-top: 8px">

    <div style="margin: 0px 0px 10px 0px;">
      <el-button type="primary" icon="ele-Plus" @click="openAdd" v-auth="'inventory:add'"> 新增 </el-button>
    </div>
    <el-table :data="tableData" tooltip-effect="light" row-key="id" border="" style="width: 100%; height: 100%;">
      <el-table-column align="center" type="selection" />
      <!-- 根据字段配置渲染表格列 -->
      <el-table-column v-for="field in fields" :key="field.name" :prop="field.name" :label="field.description">
        <template v-slot="scope">
          <!-- <el-input v-model.lazy="scope.row.id" /> -->
          <yColumn :field="field" v-model="scope.row" />
        </template>
      </el-table-column>


      <!-- 创建操作列 -->
      <el-table-column label="操作" v-if="showActions()">
        <template v-slot="scope">
          <el-button type="primary" @click="editRow(scope.row)" text>编辑</el-button>
          <el-button type="primary" @click="deleteRow(scope.row)" text>删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
      :total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small="" background="" @size-change="handleSizeChange"
      @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />

    <!-- <el-drawer v-model="dialog.visible" size="100%">
      <div style="margin: 10px;">

        <yForm ref="formEl" :formData="dialog.formData" :type="dialog.type" addUrl="dictSave" editUrl="dictEdit"
          :beforeSubmit="beforeSubmit" :afterSubmit="afterSubmit" @btn-click="cancelClick" />
      </div>
    </el-drawer> -->
  </el-card>
</template>

<script setup lang="ts">
import { onMounted, PropType, ref, watch, reactive } from 'vue';
import yForm from './yForm.vue';
import yColumn from './yColumn.vue';
import ySearchItem from './search/ySearchItem.vue';
import * as api from '/@/api/model/list';
import { ElNotification, collapseProps } from 'element-plus'
import { SysFields, fieldFilter, userFilterSchemes } from '/@/api-services/models';
import { debounce } from 'lodash-es';

const props = defineProps({
  model: String as PropType<string>,
  create: Boolean as PropType<boolean>,
  edit: Boolean as PropType<boolean>,
  del: Boolean as PropType<boolean>,
  dialog: Object as PropType<any>,
});

const tableData = ref<any>([]);
const fields = ref<SysFields[]>([]);
const filterSchemes = ref<userFilterSchemes[]>([]);
const primaryFilterParams = ref<fieldFilter[]>([]);
const filterParams = ref<fieldFilter[]>([]);
const loading = ref(false);
const showParamsComponent = ref(true);
const paramsLoading = ref(false);
const collapseParam = ref({
  activeName: "",
  butName: "更多",
});
const primaryFields = ref([] as string[]);
const queryParams = ref<any>
  ({
    model: props.model,
    filters: ref<any>
  });
const tableParams = ref({
  page: 1,
  pageSize: 10,
  total: 0,
});

const beforeSubmit = (params: any) => {
  params.id = props.dialog.editId // 添加编辑id
  return params
};
const afterSubmit = () => {
  props.dialog.visible = false;
  fetchData(); // 重新拉数据
};

const cancelClick = (type: string) => {
  if (type === 'reset') {
    props.dialog.visible = false
  }
}

// 动态表单相关结束

const editRow = (row: any) => {
  console.log('编辑行', row);
};

const deleteRow = (row: any) => {
  console.log('删除行', row);
};

const handleQuery = () => {
  fetchData();
};

const openAdd = () => {
  props.dialog.visible = true;
};

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
    if(filterSchemes.value.length>0){
      id = filterSchemes.value[0].id;
    }
    var params = {
      id: id,
      name: "默认",
      tableName: props.model,
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
    var defaultuserFilterScheme = filterSchemes.value[filterSchemes.value.length - 1];
    primaryFields.value = JSON.parse(defaultuserFilterScheme.defaultFields);
  };
  loading.value = false;
  // 初始化查询条件
  if (filterParams.value.length === 0) {
    createfilterParams();
  };
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

onMounted(() => {
  fetchData();
})


</script>

<style lang="scss">
.yahaha-list {
  width: 100%;
  /* 使用视窗宽度的80%作为组件的宽度 */
  height: calc(100% - 85px);
  /* 使用视窗高度的80%作为组件的高度 */

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

  .select-primary-filter{
    .el-form-item__label{
      color: var(--el-color-primary);
    }
  }
}
</style>

