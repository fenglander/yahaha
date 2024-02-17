<template>
    <!-- 筛选 -->
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
                                <el-option v-for="item in fields" :key="item.name" :label="item.description"
                                    :value="item.name" />
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
                    <el-button icon="ele-ZoomIn" @click="changeCollapseState"> {{ collapseParam.butName }} </el-button>
                    <el-button icon="ele-Refresh" @click="cleanQueryValue"> 重置 </el-button>
                </el-button-group>
            </div>
        </el-form>
    </el-card>
</template>

<script setup lang="ts">
import { onMounted, ref, nextTick, computed, toRaw } from 'vue';
import ySearchItem from './search/ySearchItem.vue';



const paramsLoading = ref(false);
const showParamsComponent = ref(true);
const primaryFilterParams = ref<fieldFilter[]>([]);
const filterParams = ref<fieldFilter[]>([]);
const collapseParam = ref({
    activeName: "",
    butName: "展开",
});



const setFilterParams = async (visible: any) => {
    // 上传查询方案
    if (visible) { return; }
    createfilterParams();
    await createUserFilterSchemesDebounce();
};

const createfilterParams = () => {
    // 创建筛选字段信息

    // primaryFilterParams.value = fields.value.filter((item) => primaryFields.value.includes(item.name)).map((item: any) => ({
    //   id: item.id,
    //   description: item.description,
    //   name: item.name,
    //   tType: item.tType,
    // })) as fieldFilter[];
    //   filterParams.value = fields.value.filter((item) => !primaryFields.value.includes(item.name)).map((item: any) => ({
    //     id: item.id,
    //     description: item.description,
    //     name: item.name,
    //     tType: item.tType,
    //   })) as fieldFilter[];
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


</script>