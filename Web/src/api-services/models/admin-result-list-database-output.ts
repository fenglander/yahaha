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
import { DatabaseOutput } from './database-output';
/**
 * 全局返回结果
 * @export
 * @interface AdminResultListDatabaseOutput
 */
export interface AdminResultListDatabaseOutput {
    /**
     * 状态码
     * @type {number}
     * @memberof AdminResultListDatabaseOutput
     */
    code?: number;
    /**
     * 类型success、warning、error
     * @type {string}
     * @memberof AdminResultListDatabaseOutput
     */
    type?: string | null;
    /**
     * 错误信息
     * @type {string}
     * @memberof AdminResultListDatabaseOutput
     */
    message?: string | null;
    /**
     * 数据
     * @type {Array<DatabaseOutput>}
     * @memberof AdminResultListDatabaseOutput
     */
    result?: Array<DatabaseOutput> | null;
    /**
     * 附加数据
     * @type {any}
     * @memberof AdminResultListDatabaseOutput
     */
    extras?: any | null;
    /**
     * 时间
     * @type {Date}
     * @memberof AdminResultListDatabaseOutput
     */
    time?: Date;
}
