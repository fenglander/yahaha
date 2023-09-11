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
/**
 * 
 * @export
 * @interface PageDictDataInput
 */
export interface PageDictDataInput {
    /**
     * 当前页码
     * @type {number}
     * @memberof PageDictDataInput
     */
    page?: number;
    /**
     * 页码容量
     * @type {number}
     * @memberof PageDictDataInput
     */
    pageSize?: number;
    /**
     * 排序字段
     * @type {string}
     * @memberof PageDictDataInput
     */
    field?: string | null;
    /**
     * 排序方向
     * @type {string}
     * @memberof PageDictDataInput
     */
    order?: string | null;
    /**
     * 降序排序
     * @type {string}
     * @memberof PageDictDataInput
     */
    descStr?: string | null;
    /**
     * 字典类型Id
     * @type {number}
     * @memberof PageDictDataInput
     */
    dictTypeId?: number;
    /**
     * 值
     * @type {string}
     * @memberof PageDictDataInput
     */
    value?: string | null;
    /**
     * 编码
     * @type {string}
     * @memberof PageDictDataInput
     */
    code?: string | null;
}
