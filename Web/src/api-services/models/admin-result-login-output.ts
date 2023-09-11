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
import { LoginOutput } from './login-output';
/**
 * 全局返回结果
 * @export
 * @interface AdminResultLoginOutput
 */
export interface AdminResultLoginOutput {
    /**
     * 状态码
     * @type {number}
     * @memberof AdminResultLoginOutput
     */
    code?: number;
    /**
     * 类型success、warning、error
     * @type {string}
     * @memberof AdminResultLoginOutput
     */
    type?: string | null;
    /**
     * 错误信息
     * @type {string}
     * @memberof AdminResultLoginOutput
     */
    message?: string | null;
    /**
     * 
     * @type {LoginOutput}
     * @memberof AdminResultLoginOutput
     */
    result?: LoginOutput;
    /**
     * 附加数据
     * @type {any}
     * @memberof AdminResultLoginOutput
     */
    extras?: any | null;
    /**
     * 时间
     * @type {Date}
     * @memberof AdminResultLoginOutput
     */
    time?: Date;
}
