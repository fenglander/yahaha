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
import { SqlSugarPagedListTenantOutput } from './sql-sugar-paged-list-tenant-output';
/**
 * 全局返回结果
 * @export
 * @interface AdminResultSqlSugarPagedListTenantOutput
 */
export interface AdminResultSqlSugarPagedListTenantOutput {
    /**
     * 状态码
     * @type {number}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    code?: number;
    /**
     * 类型success、warning、error
     * @type {string}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    type?: string | null;
    /**
     * 错误信息
     * @type {string}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    message?: string | null;
    /**
     * 
     * @type {SqlSugarPagedListTenantOutput}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    result?: SqlSugarPagedListTenantOutput;
    /**
     * 附加数据
     * @type {any}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    extras?: any | null;
    /**
     * 时间
     * @type {Date}
     * @memberof AdminResultSqlSugarPagedListTenantOutput
     */
    time?: Date;
}
