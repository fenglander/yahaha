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
 * 数据库表列
 * @export
 * @interface ColumnOuput
 */
export interface ColumnOuput {
    /**
     * 字段名
     * @type {string}
     * @memberof ColumnOuput
     */
    columnName?: string | null;
    /**
     * 数据库中类型
     * @type {string}
     * @memberof ColumnOuput
     */
    dataType?: string | null;
    /**
     * .NET字段类型
     * @type {string}
     * @memberof ColumnOuput
     */
    netType?: string | null;
    /**
     * 字段描述
     * @type {string}
     * @memberof ColumnOuput
     */
    columnComment?: string | null;
    /**
     * 主外键
     * @type {string}
     * @memberof ColumnOuput
     */
    columnKey?: string | null;
}
