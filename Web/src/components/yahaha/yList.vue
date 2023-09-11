<template>
  <el-card v-loading="paramsLoading" shadow="hover" :body-style="{ paddingBottom: '0' }">
    <el-form ref="queryForm" :inline="true">
      <el-form-item v-if="showParamsComponent" v-for="field in filterParams" :key="field.name" :label="field.description">
        <y-search-item :field="field" v-model="field.filters" />
      </el-form-item>

      <el-form-item>
        <el-button-group>
          <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'inventory:page'"> 查询 </el-button>
          <el-button icon="ele-Refresh" @click="cleanQueryValue"> 重置 </el-button>
        </el-button-group>
      </el-form-item>
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

    <el-drawer v-model="dialog.visible" size="50%">

      <yForm ref="formEl" :formData="dialog.formData" :type="dialog.type" addUrl="dictSave" editUrl="dictEdit"
        :beforeSubmit="beforeSubmit" :afterSubmit="afterSubmit" @btn-click="cancelClick" />
    </el-drawer>
  </el-card>
</template>

<script setup lang="ts">
import { onMounted, PropType, ref, watch, reactive } from 'vue';
import yForm from './yForm.vue';
import yColumn from './yColumn.vue';
import ySearchItem from './search/ySearchItem.vue';
import { generalListData } from '/@/api/model/table';
import { SysFields, fieldFilter } from '/@/api-services/models';
const props = defineProps({
  model: String as PropType<string>,
  create: Boolean as PropType<boolean>,
  edit: Boolean as PropType<boolean>,
  del: Boolean as PropType<boolean>,
});

const tableData = ref<any>([]);
const fields = ref<SysFields[]>([]);
const loading = ref(false);
const showParamsComponent = ref(true);
const paramsLoading = ref(false);

const filterParams = ref<fieldFilter[]>([]);
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

const dialog = reactive({
  visible: false,
  type: 1,
  editId: '',
  formData: {
    list: [
      {
        type: 'input',
        control: {
          modelValue: '',
          placeholder: '请输入字典名称'
        },
        config: {},
        name: 'name',
        item: {
          label: '字典名称'
        },
        customRules: [
          {
            type: 'required',
            message: '请输入字典名称',
            trigger: 'blur'
          }
        ]
      },
      {
        type: 'input',
        control: {
          modelValue: '',
          placeholder: '请输入字典标识'
        },
        config: {
          editDisabled: true
        },
        name: 'type',
        item: {
          label: '字典标识'
        },
        customRules: [
          {
            type: 'required',
            message: '请输入字典标识',
            trigger: 'blur'
          }
        ]
      },
      {
        type: 'radio',
        control: {
          modelValue: 1
        },
        options: [
          {
            label: '正常',
            value: 1
          },
          {
            label: '停用',
            value: 0
          }
        ],
        config: {
          optionsType: 0
        },
        name: 'status',
        item: {
          label: '状态'
        }
      },
      {
        type: 'textarea',
        control: {
          modelValue: ''
        },
        config: {},
        name: 'remark',
        item: {
          label: '说明描述'
        }
      },
      {
        type: 'button',
        control: {
          label: '保存',
          type: 'primary',
          key: 'submit'
        },
        config: {
          span: 0
        }
      },
      {
        type: 'button',
        control: {
          label: '取消',
          key: 'reset'
        },
        config: {
          span: 0
        }
      }
    ],
    form: {
      labelWidth: '',
      size: 'default'
    },
    config: {}
  }
});

const beforeSubmit = (params: any) => {
  params.id = dialog.editId // 添加编辑id
  return params
};
const afterSubmit = () => {
  dialog.visible = false;
  fetchData(); // 重新拉数据
};

const cancelClick = (type: string) => {
  if (type === 'reset') {
    dialog.visible = false
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
  dialog.visible = true;
};

const createfilterParams = () => {
  // 清理临时数据
  filterParams.value = fields.value.map((item: any) => ({
    id: item.id,
    description: item.description,
    name: item.name,
    tType: item.tType,
  })) as fieldFilter[];
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

const showActions = () => {
  return props.create || props.edit || props.del;
};

const compParams = () => {

  filterParams.value.forEach((item: any) => {

    if (item.filters !== null && item.filters !== undefined) {

      let tempFilters = item.filters;
      if (typeof tempFilters === 'string') {
        tempFilters = [tempFilters];
      }
      console.log("#21", tempFilters);
      item.filters = tempFilters.map((str: any) => {
        if (typeof str === 'string' && str.length > 0) {
          return JSON.parse(str);
        }
        return str; // 如果已经是对象，直接返回
      });
    }

  })
  queryParams.value.filters = filterParams.value;
};

const fetchData = async () => {
  loading.value = true;
  compParams();

  var res = await generalListData(Object.assign(queryParams.value, tableParams.value));
  tableData.value = res.data.result?.items ?? [];
  tableParams.value.total = res.data.result?.total;
  fields.value = res.data.result?.fields.filter((item: any) => item.description !== null && item.description.trim() !== "")
    .map((item: any) => ({
      name: item.name,
      description: item.description,
      tType: item.tType,
      navigatType: item.navigatType,
    })) as SysFields[];

  if (filterParams.value.length === 0) {
    createfilterParams();
  };

  loading.value = false;
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
</style>

