import request from '/@/utils/request';
enum Api {
    visualDevById = '/api/visualDev/byId',
    formDesginList = '/api/visualDev/formDesginList',
    listDesginList = '/api/visualDev/listDesginList',
    fieldsByModel = '/api/visualDev/fieldsByModel',
    modelInfo = '/api/visualDev/modelInfo',
    saveFormDesgin = '/api/visualDev/saveFormDesgin',
    saveListDesgin = '/api/visualDev/saveListDesgin',
    delFormDesgin = '/api/visualDev/delFormDesgin',
    userListDesignScheme = '/api/visualDev/userListDesignScheme',
    userFilterSchemes = '/api/visualDev/userFilterSchemes',
}
export const visualDevById = (params?: any) =>
    request({
        url: `${Api.visualDevById}/${params}`,
        method: 'get'
    });

export const formDesginList = (params?: any) =>
    request({
        url: `${Api.formDesginList}`,
        method: 'get',
        data: params,
    });

export const listDesginList = (params?: any) =>
    request({
        url: `${Api.listDesginList}`,
        method: 'get',
        data: params,
    });

export const fieldsByModel = (params?: any) =>
    request({
        url: `${Api.fieldsByModel}`,
        method: 'get',
        data: params,
    });

export const getRequest = (params?: any) =>
    request({
        url: `${Api.modelInfo}/${params}`,
        method: 'get'
    });

export const saveFormDesgin = (params?: any) =>
    request({
        url: Api.saveFormDesgin,
        method: 'post',
        data: params,
    });

export const saveListDesgin = (params?: any) =>
    request({
        url: Api.saveListDesgin,
        method: 'post',
        data: params,
    });

export const delFormDesgin = (params?: any) =>
    request({
        url: Api.delFormDesgin,
        method: 'post',
        data: params,
    });

export const userListDesignScheme = (params?: any) =>
    request({
        url: Api.userListDesignScheme,
        method: 'get',
        data: params,
    });

export const userFilterSchemes = (params?: any) =>
    request({
        url: Api.userFilterSchemes,
        method: 'get',
        data: params,
    });