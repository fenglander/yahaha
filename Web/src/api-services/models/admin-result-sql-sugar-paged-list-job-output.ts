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
import { SqlSugarPagedListJobOutput } from './sql-sugar-paged-list-job-output';
/**
 * 全局返回结果
 * @export
 * @interface AdminResultSqlSugarPagedListJobOutput
 */
export interface AdminResultSqlSugarPagedListJobOutput {
    /**
     * 状态码
     * @type {number}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    code?: number;
    /**
     * 类型success、warning、error
     * @type {string}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    type?: string | null;
    /**
     * 错误信息
     * @type {string}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    message?: string | null;
    /**
     * 
     * @type {SqlSugarPagedListJobOutput}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    result?: SqlSugarPagedListJobOutput;
    /**
     * 附加数据
     * @type {any}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    extras?: any | null;
    /**
     * 时间
     * @type {Date}
     * @memberof AdminResultSqlSugarPagedListJobOutput
     */
    time?: Date;
}
