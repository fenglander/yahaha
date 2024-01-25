export interface Options {
  label: string
  value: string
  disabled?: boolean
}

export interface FormItem {
  label?: string
  labelWidth?: string
  required?: boolean
  rules?: any
  error?: string
  showMessage?: boolean
  inlineMessage?: boolean
  size?: string
  showLabel?: boolean
}

export interface FormList {
  key: string
  fieldName: string
  Name: string
  type: string
  isLayout?: boolean
  control: any // 当前type表单控件所有props
  item?: FormItem // formItem组件所有props
  config: any // 其他配置信息
  customRules?: any // 自定义检验规则,这个规则会合并进item.rules
  columns?: any // 布局字段
  tableData?: any // 子表时
  options?: Options[] // radio,checkbox,select选项
  origRequired?: boolean //原始必填状态，避免在动态设置时混淆
  required?: boolean //必填
  origInvisible?: boolean //原始隐藏状态，避免在动态设置时混淆
  invisible?: boolean //隐藏
  ForceRequired?: boolean //后端强制必填校验
  validateReq?: boolean //必填校验
  readonly?: boolean //只读，会在formitem.vue进一步处理
  origReadonly?: boolean //原始只读状态，避免在动态设置时混淆
  readonlyExp?: any
  requiredExp?: any
  invisibleExp?: any
  Relate?: any // 是否为关联字段
  list?: any // 布局字段
  child?: any // 子表字段
}

export interface widgetConfig {
  name: string,
  description: string,
  fieldType: string[],
  isLayout?: boolean,
  options: any[]
}

export interface FormData {
  list: FormList[]
  form: any // form所有props
  modelId?: number
  sysModel?: any
  config?: {
    style?: string // 表单css样式，相当于scope
    hideField?: string[] // 使用v-if隐藏的字段，用于交互
    requestUrl?: string // 表单数据请求url
    addUrl?: string // 提交表单
    editUrl?: string // 编辑保存
    addLoad?: boolean
    expand?: boolean // 用于设置筛选条件默认展开/收起状态
  }
  events?: {
    beforeRequest?: Function
    afterResponse?: Function | string
    beforeSubmit?: Function
    afterSubmit?: Function
    change?: Function
  }
}

export interface TableData {
  tableProps?: any
  columns: any
  controlBtn?: any
  events?: any
  treeData?: {
    show: boolean
    beforeRequest?: Function
    afterResponse?: Function | string
    method: string
    requestUrl: string
    name: string
  }
  operateBtn?: any
  config?: any
}

export interface OpenDrawer {
  //visible: boolean
  title?: string
  direction?: string
  content?: any
  codeType?: string
  callback?: Function | string
  type?: string
}
