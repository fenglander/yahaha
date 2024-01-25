export const name = 'yhhTable'
export const description = '子表'
export const isLayout = true
export const fieldType = ['OneToMany']
export const options = [{
  path: 'child',
  label: '字段',
  type: 'selectField',
  dict: [],
  dictPath: 'SubFields',
}
]
