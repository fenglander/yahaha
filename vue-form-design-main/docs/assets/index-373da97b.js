import{d as h,C as k,r as v,a as C,b as w,o,c as n,F as p,e as d,l,t as m,k as D,n as S,j as q,q as x}from"./index-b7d7e192.js";const B={class:"task-apply"},E={class:"list"},F=["onClick"],N=h({__name:"index",setup(I){const g=k(),i=v({}),c=(e,s)=>e?e.split(",")[s]:"",_=C(()=>{const e=window.localStorage.getItem("akFormDict");let s={};return e&&(s=JSON.parse(e)),s.flow||{}}),f=()=>{x("designList",{type:3}).then(s=>{const r=s.data.list,a=[];r.forEach(t=>{a.includes(t.category)||a.push(t.category)}),a.forEach(t=>{i.value[t]=r.filter(u=>u.category===t)})})},y=e=>{g.push({path:"/task/apply/start",query:{flowId:e.id}})};return w(()=>{f()}),(e,s)=>(o(),n("div",B,[(o(!0),n(p,null,d(i.value,(r,a)=>(o(),n("div",{class:"item",key:a},[l("h3",null,m(_.value[a]||"未分组"),1),l("div",E,[(o(!0),n(p,null,d(r,t=>(o(),n("div",{key:t.id,onClick:u=>y(t)},[c(t.icon,0)?(o(),n("i",{key:0,class:D(["icon",c(t.icon,0)]),style:S({background:c(t.icon,1)})},null,6)):q("",!0),l("span",null,m(t.name),1)],8,F))),128))])]))),128))]))}});export{N as default};
