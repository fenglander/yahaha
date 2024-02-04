import request from '/@/utils/request';
enum Api {
	generalListData = '/api/models/generalListData',
	generalFormData = '/api/models/generalFormData',
	generalDelete = '/api/models/generalDelete',
	generalSave = '/api/models/generalSave',
	generalExecFunc = '/api/models/generalExecFunc',
	getFieldList = '/api/models/FieldList',
	getModelList = '/api/models/modelList',
	getActionList = '/api/models/actionList',
	sourceListByFormDesign = '/api/models/sourceListByFormDesign',
	createUserFilterSchemes = '/api/models/createUserFilterSchemes',
}

// 通用列表数据
export const generalListData = (params?: any) =>
	request({
		url: Api.generalListData,
		method: 'post',
		data: params,
	});
// 通用删除记录
export const generalDelete = (params?: any) =>
	request({
		url: Api.generalDelete,
		method: 'post',
		data: params,
	});
// 通用新增记录
export const generalSave = (params?: any) =>
	request({
		url: Api.generalSave,
		method: 'post',
		data: params,
	});
/// 获取字段信息
export const getFieldList = () =>
	request({
		url: `${Api.getFieldList}`,
		method: 'get'
	});
/// 获取模型信息
export const getModelList = (params?: any) =>
	request({
		url: Api.getModelList,
		method: 'get',
		data: params,
	});
/// 设置用户查询方案
export const createUserFilterSchemes = (params?: any) =>
	request({
		url: Api.createUserFilterSchemes,
		method: 'post',
		data: params,
	});

// 通用表单数据
export const generalFormData = (params?: any) =>
	request({
		url: Api.generalFormData,
		method: 'post',
		data: params,
	});

// 通用执行函数
export const generalExecFunc = (params?: any) =>
	request({
		url: Api.generalExecFunc,
		method: 'post',
		data: params,
	});

/**获取表单动作 */
export const getActionList = (params?: any) =>
	request({
		url: params === null ? `${Api.getActionList}/${params}` : `${Api.getActionList}`,
		method: 'get',
	});

export const uploadUrl = (params?: any) =>
	request({
		url: Api.generalFormData,
		method: 'get',
		data: params,
	});
export const getRequest = (params?: any) =>
	request({
		url: Api.generalFormData,
		method: 'get',
		data: params,
	});