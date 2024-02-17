export const name = 'yhhSelRelObject'
export const description = '关联记录'
export const fieldType = ['ManyToOne']
export const options = [{
  key: 'componentType',
  label: '类型',
  type: 'select',
  dict: [{
    name: '复选',
    value: 'checkbox'
  },{
    name: '开关',
    value: 'switch'
  }],
  default:'checkbox',
},

]