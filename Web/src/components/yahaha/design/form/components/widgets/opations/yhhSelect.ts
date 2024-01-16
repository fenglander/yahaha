export const name = 'yhhSelect'
export const description = '单选'
export const fieldType = ['Select']
export const options = [{
  key: 'placeholder',
  label: '占位字符串',
  type: 'string', 
},{
  key: 'showPassword',
  label: '密码',
  type: 'bool', 
},{
  key: 'textarea',
  label: '长本文',
  type: 'bool', 
},{
  key: 'minRows',
  label: '最小行数',
  type: 'number', 
  show: [{textarea:true}],
},{
  key: 'maxRows',
  label: '最大行数',
  type: 'number', 
  show: [{textarea:true}],
},{
  key: 'prepend',
  label: '前置',
  type: 'string', 
  show: [{textarea:false}],
},{
  key: 'append',
  label: '后置',
  type: 'string', 
  show: [{textarea:false}],
},{
  key: 'showWordLimit',
  label: '统计字数',
  type: 'bool', 
},

]