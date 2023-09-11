<template>
  <div class="inventory-container">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="Completed Quantity">
          <el-input v-model="queryParams.qty_operation_wip" clearable="" placeholder="请输入Completed Quantity"/>
          
        </el-form-item>
        <el-form-item label="Code">
          <el-input v-model="queryParams.code" clearable="" placeholder="请输入Code"/>
          
        </el-form-item>
        <el-form-item label="单位">
          <el-input-number v-model="queryParams.unit"  clearable="" placeholder="请输入单位"/>
          
        </el-form-item>
        <el-form-item label="创建时间">
          <el-date-picker placeholder="请选择创建时间" value-format="YYYY/MM/DD" type="daterange" v-model="queryParams.createtimeRange" />
          
        </el-form-item>
        <el-form-item label="更新时间">
          <el-date-picker placeholder="请选择更新时间" value-format="YYYY/MM/DD" type="daterange" v-model="queryParams.updatetimeRange" />
          
        </el-form-item>
        <el-form-item label="创建者Id">
          <el-input v-model="queryParams.createuserid" clearable="" placeholder="请输入创建者Id"/>
          
        </el-form-item>
        <el-form-item label="修改者Id">
          <el-input v-model="queryParams.updateuserid" clearable="" placeholder="请输入修改者Id"/>
          
        </el-form-item>
        <el-form-item>
          <el-button-group>
            <el-button type="primary"  icon="ele-Search" @click="handleQuery" v-auth="'inventory:page'"> 查询 </el-button>
            <el-button icon="ele-Refresh" @click="() => queryParams = {}"> 重置 </el-button>
            
          </el-button-group>
          
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="ele-Plus" @click="openAddInventory" v-auth="'inventory:add'"> 新增 </el-button>
          
        </el-form-item>
        
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 8px">
      <el-table
				:data="tableData"
				style="width: 100%"
				v-loading="loading"
				tooltip-effect="light"
				row-key="id"
				border="">
        <el-table-column type="index" label="序号" width="55" align="center"/>
         <el-table-column prop="qty_operation_wip" label="Completed Quantity" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="code" label="Code" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="unit" label="单位" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="createtime" label="创建时间" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="updatetime" label="更新时间" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="createuserid" label="创建者Id" fixed="" show-overflow-tooltip="" />
         <el-table-column prop="updateuserid" label="修改者Id" fixed="" show-overflow-tooltip="" />
        <el-table-column prop="isdelete" label="软删除" show-overflow-tooltip="">
          <template #default="scope">
            <el-tag v-if="scope.row.isdelete"> 是 </el-tag>
            <el-tag type="danger" v-else=""> 否 </el-tag>
            
          </template>
          
        </el-table-column>
        <el-table-column label="操作" width="140" align="center" fixed="right" show-overflow-tooltip="" v-if="auth('inventory:edit') || auth('inventory:delete')">
          <template #default="scope">
            <el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditInventory(scope.row)" v-auth="'inventory:edit'"> 编辑 </el-button>
            <el-button icon="ele-Delete" size="small" text="" type="primary" @click="delInventory(scope.row)" v-auth="'inventory:delete'"> 删除 </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination
				v-model:currentPage="tableParams.page"
				v-model:page-size="tableParams.pageSize"
				:total="tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				small=""
				background=""
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
	/>
      <editDialog
			    ref="editDialogRef"
			    :title="editInventoryTitle"
			    @reloadTable="handleQuery"
      />
    </el-card>
  </div>
</template>

<script lang="ts" setup="" name="inventory">
  import { ref } from "vue";
  import { ElMessageBox, ElMessage } from "element-plus";
  import { auth } from '/@/utils/authFunction';
  //import { formatDate } from '/@/utils/formatTime';

  import editDialog from '/@/views/main/inventory/component/editDialog.vue'
  import { pageInventory, deleteInventory } from '/@/api/main/inventory';


    const editDialogRef = ref();
    const loading = ref(false);
    const tableData = ref<any>
      ([]);
      const queryParams = ref<any>
        ({});
        const tableParams = ref({
        page: 1,
        pageSize: 10,
        total: 0,
        });
        const editInventoryTitle = ref("");


        // 查询操作
        const handleQuery = async () => {
        loading.value = true;
        var res = await pageInventory(Object.assign(queryParams.value, tableParams.value));
        tableData.value = res.data.result?.items ?? [];
        tableParams.value.total = res.data.result?.total;
        loading.value = false;
        };

        // 打开新增页面
        const openAddInventory = () => {
        editInventoryTitle.value = '添加存货档案';
        editDialogRef.value.openDialog({});
        };

        // 打开编辑页面
        const openEditInventory = (row: any) => {
        editInventoryTitle.value = '编辑存货档案';
        editDialogRef.value.openDialog(row);
        };

        // 删除
        const delInventory = (row: any) => {
        ElMessageBox.confirm(`确定要删除吗?`, "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
        })
        .then(async () => {
        await deleteInventory(row);
        handleQuery();
        ElMessage.success("删除成功");
        })
        .catch(() => {});
        };

        // 改变页面容量
        const handleSizeChange = (val: number) => {
        tableParams.value.pageSize = val;
        handleQuery();
        };

        // 改变页码序号
        const handleCurrentChange = (val: number) => {
        tableParams.value.page = val;
        handleQuery();
        };


handleQuery();
</script>


