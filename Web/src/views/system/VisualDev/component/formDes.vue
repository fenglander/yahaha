<!-- Created by 337547038 表单设计. -->
<template>
	<el-drawer v-model="showDrawer" :append-to-body="true" :destroy-on-close="true" size="100%" :show-close="false"
		@closed="closeDialog">
		<template #header="{ titleId, titleClass }">
			<h4 :id="titleId" :class="titleClass">表单设计</h4>
			<headTools @click="headToolClick" />
		</template>
		<div class="design-container">
			<form-control :formId="curForm" :modelId="state.formData.form.modelId" @fieldSel="searchCheckField"
				@templateSel="selectTemplate" />
			<div class="main-body">
				<el-scrollbar>
					<div class="main-form" v-loading="state.loading">
						<div class="empty-tips" v-if="state.formData.list.length === 0">从左侧拖拽来添加字段</div>
						<form-renderer :type="5" :formData="state.formData" :dict="state.formDict" />
					</div>
				</el-scrollbar>
			</div>
			<form-control-attr ref="formControlAttrEl" :formData="state.formData.form" :formConfig="state.formData.config"
				v-model:formOtherData="state.formOtherData" @open-dialog="openAceEditDrawer" />
			<ace-drawer v-model="drawer.visible" :title="drawer.title" :direction="drawer.direction"
				:content="drawer.content" :code-type="drawer.codeType" @before-close="drawerBeforeClose"
				@confirm="dialogConfirm" />
			<el-dialog v-model="state.previewVisible" title="预览" :fullscreen="true">
				<form-renderer :form-data="state.formDataPreview" :dict="state.formDict" :type="1" ref="previewForm"
					v-if="state.previewVisible" />
				<template #footer>
					<div class="dialog-footer">
						<el-button size="small" type="primary" @click="previewSubmit">提交</el-button>
						<el-button size="small" @click="state.previewVisible = false">取消</el-button>
					</div>
				</template>
			</el-dialog>
			<el-dialog v-model="showSelectModel" title="选择模型" :close-on-click-modal="false" :show-close=false>
				<select-model v-model="state.formData.form.sysModel" @clost="props.close"></select-model>
				<template #footer>
					<span>
						<el-button @click="showDrawer = false">取消</el-button>
						<el-button type="primary" @click="confirmationModel">确定</el-button>
					</span>
				</template>
			</el-dialog>
		</div>
	</el-drawer>
</template>
<script setup lang="ts">
import headTools from '/@/components/yahaha/design/components/headTools.vue';
import formControl from '/@/components/yahaha/design/form/components/formControl.vue'
import FormRenderer from '/@/components/yahaha/design/form/components/formRenderer.vue'
import FormControlAttr from '/@/components/yahaha/design/form/components/formControlAttr.vue'
import AceDrawer from '/@/components/yahaha/design/components/aceDrawer.vue'
import { useVisualDev } from '/@/stores/visualDev';
import selectModel from './selectModel.vue'
import { ref, reactive, provide, onMounted } from 'vue'
import { useDesignFormStore } from '/@/stores/designForm'
import { saveVisualDev, delVisualDev } from '/@/api/visualDev';
import { ElMessage } from 'element-plus'
import { useRoute } from 'vue-router'
import { afterResponse, beforeRequest, onChange } from '/@/components/yahaha/design/utils/'
import {
	json2string,
	objToStringify,
	string2json,
	stringToObj
} from '/@/components/yahaha/design/utils/form'
import { useLayoutStore } from '/@/components/yahaha/design/store/layout'
import { FormData, FormList } from '/@/components/yahaha/design/types'
const props = defineProps({
	desId: {
		required: false,
	},
	close: {
		type: Function,
		default: () => { },
	},

});
const showDrawer = ref(false);
const layoutStore = useLayoutStore()
layoutStore.changeBreadcrumb([{ label: '系统工具' }, { label: '表单设计' }])
const store = useDesignFormStore()

const route: any = useRoute().query || {}
const state = reactive({
	formData: {
		list: [] as any[],
		form: {
			name: "",
			openMethod: 1,
			modelId: route.source as any,
			modelName: '',
			fullName: '未命名表单',
			resId: undefined as any,
			sysModel: undefined as any,
		},
		config: {
			style: null as any,
		},
		events: {} as any,
		name: "",
	},
	editor: {},
	loading: false,
	formDataPreview: {
		form: {
			name: '',
		}
	} as FormData,
	previewVisible: false, // 预览窗口
	designType: route.type, // 当前页面设计类型，有效值search
	formDict: {},//表单字典
	formOtherData: {

	},
});
const showSelectModel = ref(false);
const curForm = ref<number>();
const drawer = reactive({
	visible: false,
	type: '',
	title: '',
	codeType: '',
	direction: undefined, //弹出方向rtl / ltr
	callback: '',
	content: ''
})
const formControlAttrEl = ref()
// 当前表单设计类型，供各子组件调用以展示不同页面，统一方式不需要每个组件都从路由中取
provide('formDesignType', state.designType)

const headToolClick = (type: string) => {
	switch (type) {
		case 'Refresh':
			state.formData.list = []
			store.setActiveKey('')
			store.setControlAttr({})
			break
		case 'View':
			// 打开预览窗口
			store.setActiveKey('')
			store.setControlAttr({})
			state.previewVisible = true
			// eslint-disable-next-line no-case-declarations
			let stringPreview = objToStringify(state.formData) // 防止预览窗口数据修改影响
			// eslint-disable-next-line no-case-declarations
			const formName = state.formData.form.name
			// eslint-disable-next-line no-case-declarations
			const reg = new RegExp(`get${formName}ControlByName`, 'g')
			stringPreview = stringPreview.replace(
				reg,
				`getPreview${formName}ControlByName`
			)
			state.formDataPreview = stringToObj(stringPreview)
			state.formDataPreview.form.name = `Preview${formName}` // 修改下表单名
			break
		case 'json':
			// 生成脚本预览
			openAceEditDrawer({
				direction: 'rtl',
				content: state.formData,
				title: '可编辑修改或将已生成的脚本粘贴进来'
			})
			break
		case 'Collection':
			saveData()
			break
		case 'TopLeft':
			showDrawer.value = false;
			break
		case 'Close':
			delData();
			break
	}
}
// 弹窗确认
const dialogConfirm = (editVal: string) => {
	// 生成脚本预览和导入json，都是将编辑器内容更新至state.formData
	try {
		if (typeof drawer.callback === 'function') {
			// callback
			const newObj =
				drawer.codeType === 'json'
					? string2json(editVal)
					: stringToObj(editVal)
			drawer.callback(newObj)
		} else {
			switch (drawer.type) {
				case 'css':
					// 表单属性－编辑表单样式
					if (!state.formData.config) {
						state.formData.config = {
							style: ""
						}
					}
					state.formData.config.style = editVal
					break
				case 'dict':
					state.formDict = string2json(editVal)
					break
				case 'beforeRequest':
				case 'beforeSubmit':
				case 'afterResponse':
				case 'afterSubmit':
				case 'change':
					if (!state.formData.events) {
						state.formData.events = {}
					}
					state.formData.events[drawer.type] = stringToObj(editVal)
					break
				default:
					state.formData = stringToObj(editVal)
			}
		}
		dialogCancel()
	} catch (res) {
		// console.log(res.message)
		//ElMessage.error(res.message)
	}
}
// 将数据保存在服务端
const saveData = async () => {
	state.formData = store.formDesginData;
	let params: any = {
		FormData: objToStringify(state.formData),
		fullName: state.formData.form.fullName, // 表单名称，用于在显示所有已创建的表单列表里显示
		ModelId: state.formData.form.modelId, // 数据源允许在表单属性设置里修改的
		modelName: state.formData.form.modelName, // 数据源允许在表单属性设置里修改的
		sysModel: state.formData.form.sysModel,
		type: 4, // 1表单 2列表
		formDict: json2string(state.formDict),
		id: state.formData.form.resId,
	}
	state.loading = true;
	await saveVisualDev(params).then((res: any) => {
		state.formData.form.resId = res.data.result ?? 0;
	}).catch(() => {
	})
	// 初始化表单设计缓存
	await useVisualDev().setVisualDevList();
	state.loading = false;
	showDrawer.value = false;
};

const delData = async () => {
	state.loading = true;
	let params: any = {
		id: state.formData.form.resId,
	}
	await delVisualDev(params)
	state.loading = false;
	showDrawer.value = false;
}

const getInitData = () => {
	showDrawer.value = true;
	if (props.desId) {
		const id = props.desId;// 当前记录保存的id
		// 获取初始表单数据
		state.loading = true;
		const result = useVisualDev().getVisualDev(id);
		if (result) {
			const formData = stringToObj(result.formData);
			state.formData = formData;
			state.formDict = string2json(result.formDict);
			state.formData.form.resId = id;
			store.setFormDesginData(formData);
		}
		state.loading = false;
	} else {
		showSelectModel.value = true;
	}
};

const openAceEditDrawer = (params: any) => {
	const { type, direction, codeType, title, callback, content } = params
	drawer.direction = direction // 窗口位置ltr/rtl
	drawer.type = type // 作为窗口唯一标识，在窗口关闭时可根据type作不同处理
	drawer.codeType = codeType || '' // 显示代码类型
	drawer.title = title ? `提示：${title}` : ''
	drawer.visible = true
	drawer.callback = callback
	let editData =
		codeType === 'json'
			? json2string(content, true)
			: objToStringify(content, true)
	switch (type) {
		case 'css':
			editData = state.formData.config?.style || ''
			break
		case 'dict':
			// 格式化一下
			editData = json2string(state.formDict, true)
			break
		case 'beforeRequest':
		case 'beforeSubmit':
		case 'afterResponse':
		case 'afterSubmit':
		case 'change':
			// eslint-disable-next-line no-case-declarations
			const beforeData: any = state.formData.events || {}
			if (beforeData[type]) {
				editData = objToStringify(beforeData[type], true)
			} else {
				if (['afterResponse', 'afterSubmit'].includes(type)) {
					editData = afterResponse
				} else if (type === 'change') {
					editData = onChange
				} else {
					editData = beforeRequest
				}
			}
			break
		// case 'afterResponse':
		// case 'afterSubmit':
		//   const newData = state.formData.events || {}
		//   if (newData[type]) {
		//     editData = objToStringify(newData[type], true)
		//   } else {
		//     editData = afterResponse
		//   }
		//   break

		case 'optionsParams':
			if (!content) {
				editData = beforeRequest
			}
			break
		case 'optionsResult':
			if (!content) {
				editData = afterResponse
			}
			break
	}
	drawer.content = editData
}
const drawerBeforeClose = () => {
	dialogCancel()
}
const dialogCancel = () => {
	drawer.visible = false
	drawer.type = ''
	drawer.title = ''
	drawer.codeType = ''
	drawer.callback = ''
	drawer.content = ''
}
// 预览窗口提交测试
const previewForm = ref()
const previewSubmit = () => {
	previewForm.value.validate((valid: boolean, fields: any) => {
		if (valid) {
			// alert('校验通过')
			ElMessage.success('校验通过')
			console.log(fields)
		} else {
			// alert('校验不通过')
			// console.log('error submit!', fields)
			ElMessage.error('校验不通过')
			return false
		}
	})
};
/**
 * 调用父级退出
 */
const closeDialog = () => {
	props.close();
};
const confirmationModel = () => {
	state.formData.form.modelId = state.formData.form.sysModel.Id;
	state.formData.form.modelName = state.formData.form.sysModel.Name;
	state.formData.form.fullName = state.formData.form.sysModel.Description;
	showSelectModel.value = false;
};
// 选择模板
const selectTemplate = (data: FormData) => {
	state.formData = stringToObj(objToStringify(data))
}
// 搜索设计时左侧快速添加字段
const searchCheckField = (data: FormList) => {
	state.formData.list.push(data)
}
getInitData()
// 从数据源点创建表单过来时，带有参数source
onMounted(() => {
	if (route.source) {
		formControlAttrEl.value.getFormFieldBySource(route.source)
	}
})
</script>
  
<style lang="scss" scoped>
body {
	margin: 0; // 去除页面垂直滚动条
}

.dialog-select-model {
	.el-dialog__header {
		padding: 0;
	}
}
</style>
