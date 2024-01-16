export const name = 'yhhNumInput'
export const description = '数字输入框'
export const fieldType = ['Int32', 'Int64', 'Double', 'Single']
export const options = [{
  key: 'placeholder',
  label: '占位字符串',
  type: 'string',
}, {
  key: 'precision',
  label: '数值精度',
  type: 'number',
  default: 0,

}
]