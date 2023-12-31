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
import { SysOrg } from './sys-org';
/**
 * 
 * @export
 * @interface UpdateOrgInput
 */
export interface UpdateOrgInput {
    /**
     * 雪花Id
     * @type {number}
     * @memberof UpdateOrgInput
     */
    id?: number;
    /**
     * 创建时间
     * @type {Date}
     * @memberof UpdateOrgInput
     */
    createTime?: Date | null;
    /**
     * 更新时间
     * @type {Date}
     * @memberof UpdateOrgInput
     */
    updateTime?: Date | null;
    /**
     * 创建者Id
     * @type {number}
     * @memberof UpdateOrgInput
     */
    createUserId?: number | null;
    /**
     * 修改者Id
     * @type {number}
     * @memberof UpdateOrgInput
     */
    updateUserId?: number | null;
    /**
     * 软删除
     * @type {boolean}
     * @memberof UpdateOrgInput
     */
    isDelete?: boolean;
    /**
     * 租户Id
     * @type {number}
     * @memberof UpdateOrgInput
     */
    tenantId?: number | null;
    /**
     * 父Id
     * @type {number}
     * @memberof UpdateOrgInput
     */
    pid?: number;
    /**
     * 编码
     * @type {string}
     * @memberof UpdateOrgInput
     */
    code?: string | null;
    /**
     * 级别
     * @type {number}
     * @memberof UpdateOrgInput
     */
    level?: number | null;
    /**
     * 机构类型
     * @type {string}
     * @memberof UpdateOrgInput
     */
    orgType?: string | null;
    /**
     * 排序
     * @type {number}
     * @memberof UpdateOrgInput
     */
    orderNo?: number;
    /**
     * 备注
     * @type {string}
     * @memberof UpdateOrgInput
     */
    remark?: string | null;
    /**
     * 
     * @type {StatusEnum}
     * @memberof UpdateOrgInput
     */
    status?: StatusEnum;
    /**
     * 机构子项
     * @type {Array<SysOrg>}
     * @memberof UpdateOrgInput
     */
    children?: Array<SysOrg> | null;
    /**
     * 名称
     * @type {string}
     * @memberof UpdateOrgInput
     */
    name: string;
}
