import request from '/@/utils/request';
enum Api {
	generalListData = '/api/models/generalListData',
  DeleteInventory = '/api/inventory/delete',
  UpdateInventory = '/api/inventory/update',
  PageInventory = '/api/inventory/page',
}

// 通用列表数据
export const getRequest = (params?: any) =>
	request({
		url: Api.generalListData,
		method: 'post',
		data: params,
	});


    export const uploadUrl = (params?: any) =>
	request({
		url: Api.generalListData,
		method: 'post',
		data: params,
	});