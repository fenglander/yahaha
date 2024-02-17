<template>
	<div class="sys-menu-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit />
					</el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="上级菜单">
							<el-cascader :options="props.menuData"
								:props="{ checkStrictly: true, emitPath: false, value: 'id', label: 'title' }"
								placeholder="请选择上级菜单" clearable class="w100" v-model="state.ruleForm.pid">
								<template #default="{ node, data }">
									<span>{{ data.title }}</span>
									<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
								</template>
							</el-cascader>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="菜单类型" prop="type"
							:rules="[{ required: true, message: '菜单类型不能为空', trigger: 'blur' }]">
							<el-radio-group v-model="state.ruleForm.type">
								<el-radio :label="1">目录</el-radio>
								<el-radio :label="2">菜单</el-radio>
								<el-radio :label="3">按钮</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单名称" prop="title"
							:rules="[{ required: true, message: '菜单名称不能为空', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.title" placeholder="菜单名称" clearable />
						</el-form-item>
					</el-col>
					<template v-if="requiredName">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="路由名称" prop="name"
								:rules="[{ required: requiredName, message: '路由名称不能为空', trigger: 'blur' }]">
								<el-input v-model="state.ruleForm.name" placeholder="路由名称" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="系统模型">
								<el-select v-model="state.ruleForm.modelId" clearable filterable remote reserve-keyword
									placeholder="输入名称" remote-show-suffix :remote-method="getModelOptions"
									@focus="getModelOptions" @change="clearVisualDev" :loading="loading">
									<el-option v-for="item in modelOptions" :key="item.Id" :label="item.Name"
										:value="item.Id" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="表单应用">
								<el-select v-model="state.ruleForm.formDesignId" clearable filterable remote reserve-keyword
									placeholder="输入名称" remote-show-suffix :remote-method="getVisualDevOptions"
									@focus="getVisualDevOptions" @change="changeModelIdByVisualDev" :loading="loading">
									<el-option v-for="item in visualDevOptions" :key="item.Id" :label="item.FullName"
										:value="item.Id" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="路由路径">
								<el-input v-model="state.ruleForm.path" :disabled="true" placeholder="路由路径" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="组件路径">
								<el-input :disabled="![null, undefined, 0, ''].includes(state.ruleForm.modelId)"
									v-model="state.ruleForm.component" placeholder="组件路径" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单图标">
								<IconSelector v-model="state.ruleForm.icon" :size="getGlobalComponentSize"
									placeholder="菜单图标" type="all" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="重定向">
								<el-input v-model="state.ruleForm.redirect" placeholder="重定向地址" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="链接地址">
								<el-input v-model="state.ruleForm.outLink" placeholder="外链/内嵌时链接地址" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单排序">
								<el-input-number v-model="state.ruleForm.orderNo" placeholder="排序" class="w100" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否隐藏">
								<el-radio-group v-model="state.ruleForm.isHide">
									<el-radio :label="true">隐藏</el-radio>
									<el-radio :label="false">不隐藏</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否缓存">
								<el-radio-group v-model="state.ruleForm.isKeepAlive">
									<el-radio :label="true">缓存</el-radio>
									<el-radio :label="false">不缓存</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否固定">
								<el-radio-group v-model="state.ruleForm.isAffix">
									<el-radio :label="true">固定</el-radio>
									<el-radio :label="false">不固定</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否内嵌">
								<el-radio-group v-model="state.ruleForm.isIframe">
									<el-radio :label="true">内嵌</el-radio>
									<el-radio :label="false">不内嵌</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
					</template>
					<template v-if="state.ruleForm.type === 3">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="权限标识">
								<el-input v-model="state.ruleForm.permission" placeholder="权限标识" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单排序">
								<el-input-number v-model="state.ruleForm.orderNo" placeholder="排序" class="w100" />
							</el-form-item>
						</el-col>
					</template>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="是否启用">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :label="1">启用</el-radio>
								<el-radio :label="2">不启用</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="备注">
							<el-input v-model="state.ruleForm.remark" placeholder="请输入备注内容" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditMenu">
import { computed, reactive, ref } from 'vue';
import IconSelector from '/@/components/iconSelector/index.vue';

import { getAPI } from '/@/utils/axios-utils';
import other from '/@/utils/other';
import { SysMenuApi } from '/@/api-services/api';
import { useVisualDev } from '/@/stores/visualDev';
import { useSysModel } from '/@/stores/sysModel';

import { SysMenu, UpdateMenuInput } from '/@/api-services/models';
import { formatNumber } from '/@/components/yahaha/design/utils/'
const props = defineProps({
	title: String,
	menuData: Array<SysMenu>,
});
const emits = defineEmits(['handleQuery']);
const loading = ref(false);
const visualDevOptions = ref<any[]>();
const modelOptions = ref<any[]>();
const visualDevComponent = ref("/system/generalView/index");
const requiredName = computed(() => {
	return state.ruleForm.type === 1 || state.ruleForm.type === 2
})
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateMenuInput,
});

// 获取全局组件大小
const getGlobalComponentSize = computed(() => {
	return other.globalComponentSize();
});

// 打开弹窗
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

const getVisualDevOptions = (query?: string) => {
	loading.value = true;
	if (state.ruleForm.modelId) {
		visualDevOptions.value = useVisualDev().formDesginList.filter((it: any) => it.ModelId === state.ruleForm.modelId)
	} else {
		visualDevOptions.value = useVisualDev().formDesginList;
	}
	if (query && query !== '' && query.constructor !== FocusEvent) {
		visualDevOptions.value = visualDevOptions.value.filter((item: any) => item.FullName !== null && item.FullName.trim() !== "" && item.FullName.trim().indexOf(query) > -1)
	}
	// const res = await setFormDesginList(query);
	// visualDevOptions.value = res.data?.result ?? [];
	loading.value = false;
}
getVisualDevOptions();

const clearVisualDev = () => {
	const visualDev = useVisualDev().getFormDesgin(state.ruleForm.formDesignId);
	if (visualDev && visualDev.ModelId !== state.ruleForm.modelId) {
		state.ruleForm.formDesignId = null
	}
}

const changeModelIdByVisualDev = () => {
	const visualDev = useVisualDev().getFormDesgin(state.ruleForm.formDesignId);
	if (visualDev) {
		state.ruleForm.modelId = formatNumber(visualDev.ModelId);
	}
}

const getModelOptions = (query?: string) => {
	loading.value = true;
	if (query && query !== '' && query.constructor !== FocusEvent) {
		console.log(query.constructor)
		const res = useSysModel().sysModelList;
		modelOptions.value = res.filter((item: any) => item.Name !== null && item.Name.trim() !== "" && item.Name.trim().indexOf(query) > -1)
	} else {
		modelOptions.value = useSysModel().sysModelList;
	}
	loading.value = false;
}
getModelOptions();

// 提交
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		afterSubmit();
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysMenuApi).apiSysMenuUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysMenuApi).apiSysMenuAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

const findNodeById = (treeNode: any[] | undefined, targetId: number) => {
	console.log(treeNode)
	if (treeNode === undefined) {
		return undefined
	}
	for (const item of treeNode) {
		if (item.id === targetId) {
			return item
		}
		if (item.children) {
			const result: any = findNodeById(item.children, targetId);
			if (result) {
				return result;
			}
		}
	}
	return undefined;
};

const afterSubmit = () => {
	if (state.ruleForm.modelId) {
		state.ruleForm.component = visualDevComponent.value;
	}
	let path = ""
	if (state.ruleForm.pid) {
		const p = findNodeById(props.menuData, state.ruleForm.pid)
		if (p) { path = path + p.path }
	}
	if (state.ruleForm.name) { path = path + "/" + state.ruleForm.name; }
	state.ruleForm.path = path
};

// 导出对象
defineExpose({ openDialog });
</script>
