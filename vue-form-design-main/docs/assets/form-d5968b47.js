import{d as g,v as _,u as p,C as v,r as D,I as h,a as y,b,h as I,D as k,z as C,o as B,c as E,m as S,S as i,q,E as x,W as F,B as j}from"./index-b7d7e192.js";const R={style:{"min-height":"300px"}},L=g({__name:"form",setup(T){const d=_(),r=p().query,c=v(),n=D(),e=h({formData:{list:[],form:{},config:{}},dict:{},formId:r.form,id:r.id,loading:!0}),f=y(()=>r.id?2:1),m=()=>{if(!e.formId)return i.error("非法操作."),!1;const t={id:e.formId};q("designById",t).then(a=>{var s;const o=a.data;o&&Object.keys(o).length&&(e.formData=x(o.data),e.dict=F(o.dict),(r.id||(s=e.formData.config)!=null&&s.addLoad)&&n.value.getData({formId:e.formId,id:r.id}),d.changeBreadcrumb([{label:"内容管理"},{label:o.name}])),j(()=>{e.loading=!1})}).catch(a=>{e.loading=!1,i.error(a.message||"非法操作..")})},l=t=>(t.formId=e.formId,t.id=r.id,t),u=t=>{t==="success"&&c.go(-1)};return b(()=>{m()}),(t,a)=>{const o=I("ak-form"),s=k("loading");return C((B(),E("div",R,[S(o,{ref_key:"formEl",ref:n,formData:e.formData,type:f.value,dict:e.dict,requestUrl:"getFormContent",addUrl:"saveFormContent",editUrl:"editFormContent",beforeSubmit:l,afterSubmit:u},null,8,["formData","type","dict"])])),[[s,e.loading]])}}});export{L as default};
