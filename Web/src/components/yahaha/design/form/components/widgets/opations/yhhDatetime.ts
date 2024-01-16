export const name = 'yhhDatetime'
export const description = '日期时间'
export const fieldType = ['DateTime', 'DateTimeOffset']
export const options = [{
  key: 'componentType',
  label: '类型',
  type: 'select',
  dict: [{
    name: '日期',
    value: 'date'
  }, {
    name: '日期时间',
    value: 'datetime'
  }],
  default: 'datetime',
},{
  key: 'placeholder',
  label: '占位字符串',
  type: 'string', 
},

]