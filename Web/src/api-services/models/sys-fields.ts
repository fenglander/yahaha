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
 * @interface SysField
 */
export interface SysField {
    /**
     * id
     * @type {number}
     * @memberof SysField
     */
    Id?: number;
    /**
     * 名称
     * @type {string}
     * @memberof SysField
     */
    Name: string;
    /**
     * 描述
     * @type {string}
     * @memberof SysField
     */
    Description?: string | null;
    /**
     * 帮助
     * @type {string}
     * @memberof SysField
     */
    Help?: string | null;
    /**
     * 模型ID
     * @type {number}
     * @memberof SysField
     */
    Modelid?: number | null;
    /**
     * 数据类型
     * @type {string}
     * @memberof SysField
     */
    tType: string;
    /**
     * 关系类型
     * @type {string}
     * @memberof SysField
     */
    Navigattype?: string | null;
    /**
     * 关系字段
     * @type {string}
     * @memberof SysField
     */
    relfieldname?: string | null;
    /**
     * 关系字段2
     * @type {string}
     * @memberof SysField
     */
    relfieldname2?: string | null;
    /**
     * 映射类型
     * @type {string}
     * @memberof SysField
     */
    mappingtype?: string | null;
    /**
     * 映射对象ID1
     * @type {string}
     * @memberof SysField
     */
    mappingaid?: string | null;
    /**
     * 映射对象ID2
     * @type {string}
     * @memberof SysField
     */
    mappingbid?: string | null;
    /**
     * 过滤条件
     * @type {string}
     * @memberof SysField
     */
    wheresql?: string | null;
    /**
     * 创建时间
     * @type {Date}
     * @memberof SysField
     */
    createtime?: Date | null;
    /**
     * 更新时间
     * @type {Date}
     * @memberof SysField
     */
    updatetime?: Date | null;
    /**
     * 创建者Id
     * @type {number}
     * @memberof SysField
     */
    createuserid?: number | null;
    /**
     * 修改者Id
     * @type {number}
     * @memberof SysField
     */
    updateuserid?: number | null;
    /**
     * 软删除
     * @type {boolean}
     * @memberof SysField
     */
    isdelete?: boolean | null;
}


export interface fieldFilter {
    id: number;
    Name: string;
    Description: string;
    tType: string;
    filters?: any;
    default: false;
    EnumValue?: string;
    RelModel?: any;
};
export interface userFilterSchemes {
    Name: string,
    TableName: string,
    ModelId: number,
    SysModels: any,
    DefaultFields: string,
    DefaultFilter: string,
    Default: boolean,
    createTime: any,
    UpdateTime: any,
    CreateUserId: any | null,
    UpdateUserId: any | null,
    IsDelete: boolean,
    Id: number
};