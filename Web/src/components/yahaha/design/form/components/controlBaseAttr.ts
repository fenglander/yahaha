export const baseAttrItem = [
    {
        label: '字段名称',
        type: 'string',
        //value: controlData.value.fieldName,
        placeholder: '字段名称',
        readonly: true,
        path: 'fieldName'
    },
    {
        label: '标题',
        placeholder: '组件标题',
        path: 'label',
        type: 'string',
    },
    {
        label: '隐藏标题',
        path: 'hideLable',
        type: 'switch',
    },
    {
        label: '组件类型',
        //value: controlData.value.type,
        placeholder: '组件类型',
        path: 'widget',
        type: 'select',
        //dict: widgetList.value
    },
    {
        label: '只读',
        //value: controlData.value.readonly,
        type: 'string',
        placeholder: '控制字段是否只读,支持json表达式,最大支持关联字段3层,如[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'readonlyExp',
        format:'json',
        show: ['and',
            ['isLayout', '==', false],
        ]
    },
    {
        label: '必填',
        //value: controlData.value.required,
        type: 'string',
        placeholder: '控制字段是否必填,支持json表达式,最大支持关联字段3层,如[1=1],[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'requiredExp',
        format:'json',
        show: ['and',
            ['isLayout', '==', false],
        ]
    },
    {
        label: '隐藏',
        //value: controlData.value.hide,
        type: 'string',
        placeholder: '控制字段是否隐藏,支持json表达式,最大支持关联字段3层,如[[\'Code\', \'==\', false],[\'qty\', \'>\', 0]]',
        path: 'invisibleExp',
        format:'json',
        show: ['and',
            ['isLayout', '==', false],
        ]
    },
    {
        label: '控件标识',
        //value: controlData.value.name,
        type: 'string',
        placeholder: '字段唯一标识，区分不同组件',
        path: 'key',
        format: 'filedNameKey',
        readonly: true,
    },
    {
        label: '是否容器',
        path: 'isLayout',
        type: 'select',
        show: [false]
    },
]

export const otherAttrItem = [
    {
        label: '自定义Class',
        //value: controlData.value.config.className,
        placeholder: '样式类名',
        path: 'config.className'
    }]

export const formAttrItem = [
    {
        label: '表单名称',
        placeholder: '用于保存的表单名称',
        //value: props.formOtherData.fullName,
        path: 'fullName',
    },
    {
        label: '实体类',
        placeholder: '实体类',
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