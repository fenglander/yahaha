import{d as p,u as v,r as o,p as _,a as h,b,o as r,c as u,F as k,e as y,n as D,f as S,g as B}from"./index-b7d7e192.js";import{g as R,a as q}from"./getData-fc38b1eb.js";const z=p({__name:"show",setup(w){const i=v(),l=o(!0),c=o({});_("globalScreen",c);const s=o({list:[],config:{}}),d=h(()=>{const{width:n,height:e,background:a,primary:t}=s.value.config;return{width:n,height:e,background:a,color:t,position:"relative"}}),f=()=>{R(i.params.id).then(n=>{l.value=!1,s.value=n;const{requestUrl:e,afterResponse:a,beforeRequest:t,method:g}=s.value.config;e&&q(e,a,t,g).then(m=>{c.value=m})}).catch(()=>{l.value=!1})};return b(()=>{f()}),(n,e)=>(r(),u("div",{style:D(d.value),class:"design-canvas"},[(r(!0),u(k,null,y(s.value.list,(a,t)=>(r(),S(B,{key:t,data:a},null,8,["data"]))),128))],4))}});export{z as default};
