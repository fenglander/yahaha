import request from '/@/utils/request';
enum Api {
    visualDevById = '/api/visualDev/byId',
    visualDevList = '/api/visualDev/list',
    fieldsByModel = '/api/visualDev/fieldsByModel',
    modelInfo = '/api/visualDev/modelInfo',
    saveVisualDev = '/api/visualDev/saveVisualDev',
    delVisualDev = '/api/visualDev/delVisualDev',
}
export const visualDevById = (params?: any) =>
    request({
        url: `${Api.visualDevById}/${params}`,
        method: 'get'
    });

export const getVisualDevList = (params?: any) =>
    request({
        url: `${Api.visualDevList}`,
        method: 'get',
        data: params,
    });

export const fieldsByModel = (params?: any) =>
    request({
        url: `${Api.fieldsByModel}`,
        method: 'get',
        data: params,
    });

export const modelInfo = (params?: any) =>
    request({
        url: `${Api.modelInfo}/${params}`,
        method: 'get'
    });

export const saveVisualDev = (params?: any) =>
    request({
        url: Api.saveVisualDev,
        method: 'post',
        data: params,
    });

export const delVisualDev = (params?: any) =>
    request({
        url: Api.delVisualDev,
        method: 'post',
        data: params,
    });