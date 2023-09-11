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
 * 
 * @export
 * @interface AddDictTypeInput
 */
export interface AddDictTypeInput {
    /**
     * 雪花Id
     * @type {number}
     * @memberof AddDictTypeInput
     */
    id?: number;
    /**
     * 创建时间
     * @type {Date}
     * @memberof AddDictTypeInput
     */
    createTime?: Date | null;
    /**
     * 更新时间
     * @type {Date}
     * @memberof AddDictTypeInput
     */
    updateTime?: Date | null;
    /**
     * 创建者Id
     * @type {number}
     * @memberof AddDictTypeInput
     */
    createUserId?: number | null;
    /**
     * 修改者Id
     * @type {number}
     * @memberof AddDictTypeInput
     */
    updateUserId?: number | null;
    /**
     * 软删除
     * @type {boolean}
     * @memberof AddDictTypeInput
     */
    isDelete?: boolean;
    /**
     * 名称
     * @type {string}
     * @memberof AddDictTypeInput
     */
    name: string;
    /**
     * 编码
     * @type {string}
     * @memberof AddDictTypeInput
     */
    code: string;
    /**
     * 排序
     * @type {number}
     * @memberof AddDictTypeInput
     */
    orderNo?: number;
    /**
     * 备注
     * @type {string}
     * @memberof AddDictTypeInput
     */
    remark?: string | null;
    /**
     * 
     * @type {StatusEnum}
     * @memberof AddDictTypeInput
     */
    status?: StatusEnum;
}
