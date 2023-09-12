import request from '/@/utils/request';
enum Api {
	sourceListByFormDesign = '/api/models/sourceListByFormDesign',
	DeleteInventory = '/api/inventory/delete',
	UpdateInventory = '/api/inventory/update',
	PageInventory = '/api/inventory/page',
}

// 通用列表数据
export const getRequest = (params?: any) =>
	request({
		url: Api.PageInventory,
		method: 'post',
		data: params,
	});
	
	export const uploadUrl = (params?: any) =>
	request({
		url: Api.PageInventory,
		method: 'post',
		data: params,
	});
export const sourceListByFormDesign = (params?: any) =>
	request({
		url: Api.sourceListByFormDesign,
		method: 'get',
	});