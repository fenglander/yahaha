export const name = 'yhhCheckbox'
export const description = '复选框'
export const fieldType = ['Boolean']
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