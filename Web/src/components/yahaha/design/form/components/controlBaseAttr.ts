export const baseAttrItem = [
    {
        label: '字段名称',
        type: 'string',
        //value: controlData.value.fieldName,
        placeholder: '字段名称',
        path: 'fieldName'
    },
    {
        label: '组件类型',
        //value: controlData.value.type,
        placeholder: '组件类型',
        path: 'type',
        type: 'select',
        //dict: widgetList.value
    },
    {
        label: '控件标识',
        //value: controlData.value.name,
        type: 'string',
        placeholder: '字段唯一标识，区分不同组件',
        path: 'key',
        eventName: 'filedNameKey',
        readonly: true,
    },
    {
        label: '只读',
        //value: controlData.value.readonly,
        type: 'string',
        placeholder: '控制字段是否只读,支持表达式,最大支持关联字段3层,如[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'readonlyExp',
        show: ['and',
            ['readonly', '==', false],
            ['isLayout', '==', false],
        ]
    },
    {
        label: '必填',
        //value: controlData.value.required,
        type: 'string',
        placeholder: '控制字段是否必填,支持表达式,最大支持关联字段3层,如[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'requiredExp',
        show: ['and',
            ['required', '==', false],
            ['isLayout', '==', false],
        ]
    },
    {
        label: '隐藏',
        //value: controlData.value.hide,
        type: 'string',
        placeholder: '控制字段是否隐藏,支持表达式,最大支持关联字段3层,如[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'invisibleExp',
        show: ['and',
            ['invisible', '==', false],
            ['isLayout', '==', false],
        ]
    }
]

export const otherAttrItem = [
    {
        label: '自定义Class',
        //value: controlData.value.config.className,
        placeholder: '样式类名',
        path: 'config.className'
    },
    {
        label: '联动',
        //value: controlData.value.config.linkKey,
        type: 'switch',
        path: 'config.linkKey'
    },
    {
        label: '联动条件',
        //value: controlData.value.config.linkResult,
        type: 'string',
        placeholder: '表达式如: $.input>1 $表示为当前表单数据，input为字段标识',
        path: 'config.linkResult',
        show: ['config.linkKey = true'],
    }]

export const formAttrItem = [
    {
        label: '表单名称',
        placeholder: '用于保存的表单名称',
        //value: props.formOtherData.fullName,
        path: 'fullName',
    },
    {
        label: '数据源',
        placeholder: '数据源',
        //value: props.formOtherData.modelName,
        path: 'modelName',
        readonly: true,
    },
    {
        label: '数据源id',
        //value: props.formOtherData.modelId,
        path: 'modelId',
        readonly: true,
        hide: true,
    },
    {
        label: '跳转方式',
        placeholder: '请选择跳转方式',
        path: 'select',
        options: [
            { label: '弹窗', value: 1 },
            { label: '路由', value: 2 },
        ],
        key: 'openMethod',
        hide: true,
    },
    {
        label: '表单标识',
        placeholder: '表单唯一标识',
        path: 'resId',
        readonly: true,
    },
    {
        label: '表单标签宽度',
        //value: formData.value.labelWidth,
        placeholder: '表单label宽;如180px',
        path: 'labelWidth'
    },
    {
        label: '表单样式名称',
        //value: formData.value.class,
        placeholder: '额外添加的表单class类名',
        path: 'class',
        type: 'select',
        options: [
            { label: '无样式', value: '' },
            { label: '每行两列', value: 'form-row-2' },
            { label: '每行三列', value: 'form-row-3' },
            { label: '每行四列', value: 'form-row-4' }
        ],
        clearable: true
    },
    {
        label: '表单动作',
        //value: formData.value.Actions,
        path: 'Actions',
        type: 'switch'
    },
    {
        label: '字段名后添加冒号',
        //value: formData.value.showColon,
        path: 'showColon',
        type: 'switch'
    },
]