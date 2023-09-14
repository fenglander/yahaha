/* tslint:disable */
/* eslint-disable */
/**
 * Yahaha
 * 只想开心的上班<br/>
 *
 * OpenAPI spec version: 1.0.0
 *
 */
/**
 * 全局返回结果
 * @export
 * @interface SysFields
 */
export interface SysFields {
    /**
     * id
     * @type {number}
     * @memberof SysFields
     */
    id?: number;
    /**
     * 名称
     * @type {string}
     * @memberof SysFields
     */
    name: string;
    /**
     * 描述
     * @type {string}
     * @memberof SysFields
     */
    description?: string | null;
    /**
     * 帮助
     * @type {string}
     * @memberof SysFields
     */
    help?: string | null;
    /**
     * 模型ID
     * @type {number}
     * @memberof SysFields
     */
    modelid?: number | null;
    /**
     * 数据类型
     * @type {string}
     * @memberof SysFields
     */
    tType: string;
    /**
     * 关系类型
     * @type {string}
     * @memberof SysFields
     */
    navigattype?: string | null;
    /**
     * 关系字段
     * @type {string}
     * @memberof SysFields
     */
    relfieldname?: string | null;
    /**
     * 关系字段2
     * @type {string}
     * @memberof SysFields
     */
    relfieldname2?: string | null;
    /**
     * 映射类型
     * @type {string}
     * @memberof SysFields
     */
    mappingtype?: string | null;
    /**
     * 映射对象ID1
     * @type {string}
     * @memberof SysFields
     */
    mappingaid?: string | null;
    /**
     * 映射对象ID2
     * @type {string}
     * @memberof SysFields
     */
    mappingbid?: string | null;
    /**
     * 过滤条件
     * @type {string}
     * @memberof SysFields
     */
    wheresql?: string | null;
    /**
     * 创建时间
     * @type {Date}
     * @memberof SysFields
     */
    createtime?: Date | null;
    /**
     * 更新时间
     * @type {Date}
     * @memberof SysFields
     */
    updatetime?: Date | null;
    /**
     * 创建者Id
     * @type {number}
     * @memberof SysFields
     */
    createuserid?: number | null;
    /**
     * 修改者Id
     * @type {number}
     * @memberof SysFields
     */
    updateuserid?: number | null;
    /**
     * 软删除
     * @type {boolean}
     * @memberof SysFields
     */
    isdelete?: boolean | null;
}


export interface fieldFilter {
    id: number;
    name: string;
    description: string;
    tType: string;
    filters?: any;
    default: false;
};
export interface userFilterSchemes {
    name: string,
    tableName: string,
    modelId: number,
    sysModels: any,
    defaultFields: string,
    defaultFilter: string,
    default: boolean,
    createTime: any,
    updateTime: any,
    createUserId: any | null,
    updateUserId: any | null,
    isDelete: boolean,
    id: number
}