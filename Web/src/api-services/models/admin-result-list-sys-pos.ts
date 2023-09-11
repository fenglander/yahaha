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
import { SysPos } from './sys-pos';
/**
 * 全局返回结果
 * @export
 * @interface AdminResultListSysPos
 */
export interface AdminResultListSysPos {
    /**
     * 状态码
     * @type {number}
     * @memberof AdminResultListSysPos
     */
    code?: number;
    /**
     * 类型success、warning、error
     * @type {string}
     * @memberof AdminResultListSysPos
     */
    type?: string | null;
    /**
     * 错误信息
     * @type {string}
     * @memberof AdminResultListSysPos
     */
    message?: string | null;
    /**
     * 数据
     * @type {Array<SysPos>}
     * @memberof AdminResultListSysPos
     */
    result?: Array<SysPos> | null;
    /**
     * 附加数据
     * @type {any}
     * @memberof AdminResultListSysPos
     */
    extras?: any | null;
    /**
     * 时间
     * @type {Date}
     * @memberof AdminResultListSysPos
     */
    time?: Date;
}
