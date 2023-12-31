/* tslint:disable */
/* eslint-disable */
/**
 * Yahaha
 * 让 .NET 开发更简单、更通用、更流行。前后端分离架构(.NET6/Vue3)，开箱即用紧随前沿技术。<br/>
 *
 * OpenAPI spec version: 1.0.0
 * Contact: 515096995@qq.com
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import { StatusEnum } from './status-enum';
/**
 * 系统动态插件表
 * @export
 * @interface SysPlugin
 */
export interface SysPlugin {
    /**
     * 雪花Id
     * @type {number}
     * @memberof SysPlugin
     */
    id?: number;
    /**
     * 创建时间
     * @type {Date}
     * @memberof SysPlugin
     */
    createTime?: Date | null;
    /**
     * 更新时间
     * @type {Date}
     * @memberof SysPlugin
     */
    updateTime?: Date | null;
    /**
     * 创建者Id
     * @type {number}
     * @memberof SysPlugin
     */
    createUserId?: number | null;
    /**
     * 修改者Id
     * @type {number}
     * @memberof SysPlugin
     */
    updateUserId?: number | null;
    /**
     * 软删除
     * @type {boolean}
     * @memberof SysPlugin
     */
    isDelete?: boolean;
    /**
     * 租户Id
     * @type {number}
     * @memberof SysPlugin
     */
    tenantId?: number | null;
    /**
     * 名称
     * @type {string}
     * @memberof SysPlugin
     */
    name: string;
    /**
     * C#代码
     * @type {string}
     * @memberof SysPlugin
     */
    csharpCode: string;
    /**
     * 程序集名称
     * @type {string}
     * @memberof SysPlugin
     */
    assemblyName?: string | null;
    /**
     * 排序
     * @type {number}
     * @memberof SysPlugin
     */
    orderNo?: number;
    /**
     * 
     * @type {StatusEnum}
     * @memberof SysPlugin
     */
    status?: StatusEnum;
    /**
     * 备注
     * @type {string}
     * @memberof SysPlugin
     */
    remark?: string | null;
}
