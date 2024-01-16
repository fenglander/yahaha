import request from '/@/utils/request';
enum Api {
    selRelObjectQuery = '/api/widgetSerivce/selRelObjectQuery',
}

export const selRelObjectQuery = (params?: any) =>
    request({
        url: `${Api.selRelObjectQuery}`,
        method: 'post',
        data: params,
    });