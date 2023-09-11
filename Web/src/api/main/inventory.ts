import request from '/@/utils/request';
enum Api {
  AddInventory = '/api/inventory/add',
  DeleteInventory = '/api/inventory/delete',
  UpdateInventory = '/api/inventory/update',
  PageInventory = '/api/inventory/page',
}

// 增加存货档案
export const addInventory = (params?: any) =>
	request({
		url: Api.AddInventory,
		method: 'post',
		data: params,
	});

// 删除存货档案
export const deleteInventory = (params?: any) => 
	request({
			url: Api.DeleteInventory,
			method: 'post',
			data: params,
		});

// 编辑存货档案
export const updateInventory = (params?: any) => 
	request({
			url: Api.UpdateInventory,
			method: 'post',
			data: params,
		});

// 分页查询存货档案
export const pageInventory = (params?: any) => 
	request({
			url: Api.PageInventory,
			method: 'post',
			data: params,
		});


