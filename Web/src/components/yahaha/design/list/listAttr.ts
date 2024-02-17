export const listAttr = [

    {
        label: '列表名称',
        placeholder: '保存的数据列表名称',
        path: 'name'
    }, {
        label: '模型Id',
        placeholder: '请选择所属表单',
        path: 'modelId',
        readonly: true
    }, {
        label: '模型名称',
        placeholder: '请选择所属表单',
        path: 'modelName',
        readonly: true
    }, {
        label: '模型全称',
        placeholder: '请选择所属表单',
        path: 'modelFullName',
        readonly: true
    }, {
        label: '数据添加编辑打开方式',
        placeholder: '默认新页面打开',
        type: 'select',
        options: [
            { label: '弹窗', value: 'dialog' },
            { label: '新页面', value: 'page' }
        ],
        path: 'listConfig.config.openType',
        clearable: true,
        default: 'page',
        //hide: !state.formId
    }, {
        label: '可编辑',
        type: 'switch',
        path: 'listConfig.config.editable',
        default: false
    }, {
        label: '显示行号',
        type: 'switch',
        path: 'listConfig.config.displayLineNumbers',
        default: true,
    }, {
        label: '查询所有字段',
        type: 'switch',
        path: 'listConfig.config.expandSearch',
        default: true
    }, {
        label: '查询跳转页面',
        type: 'switch',
        path: 'listConfig.config.searchJump'
    }, {
        label: '开启侧栏树',
        path: 'listConfig.config.tree',
        type: 'switch'
    }
]
