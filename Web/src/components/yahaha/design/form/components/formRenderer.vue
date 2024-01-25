<!-- Created by 337547038 on 2021/9/25. -->
<template>
  <div v-loading="loading">
    <el-form v-bind="formData.form" ref="ruleForm" :model="formValues" :disabled="disabled" class="add-form "
      :class="{ 'design-form': type === 5 }">
      <form-group :data="formDesginData?.list" :all-data="formDesginData?.list" />
      <slot></slot>
    </el-form>
  </div>
</template>
<script lang="ts" setup>
import FormGroup from './formGroup.vue'
import { computed, ref, watch, onUnmounted, onMounted, nextTick, provide } from 'vue'
import type { FormData, FormList, } from '../../types'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  constGetControlByName,
  constFormBtnEvent,
  constControlChange,
  constblurEvent,
  constFormProps,
  appendOrRemoveStyle, jsonParseStringify
} from '../../utils'
import { useSysModel } from '/@/stores/sysModel';
import * as api from '/@/api/model/';
import { applyFilter } from '../../utils/applyFilter'
import { useDesignFormStore } from '/@/stores/designForm'
const props = withDefaults(
  defineProps<{
    formData: FormData
    type?: number // 1新增；2修改；3查看（表单模式）；5设计
    disabled?: boolean // 禁用表单提交
    requestUrl?: string // 编辑数据请求url
    value?: { [key: string]: any } // 表单初始值，同setValue
  }>(),
  {
    type: 1, // 1新增；2修改；3查看（表单模式）； 5设计
    formData: () => {
      return {
        list: [],
        form: {},
        config: {}
      }
    },
    dict: () => {
      return {}
    },
    isSearch: false
  }
)

const formDesginData = ref<FormData>(props.formData);
const formValues = ref<any>({});

const emits = defineEmits<{
  (e: 'btnClick', type: string): void
  (e: 'change', val: any): void // 表单组件值发生变化时
}>()
const router = useRouter()

const triggerList = computed(() => {
  if (props.type !== 5) {
    const row = useSysModel().getSysActionById(formDesginData.value?.modelId).filter((item: any) => item.Trigger);
    const res = row.map((item: any) => {
      return {
        ...item, // 保留原始属性
        triggerKey: item.FieldName.split('.')[0], // 增加新属性
      };
    });
    return res;
  } else {
    return [];
  }
})

const relateFieldList = computed(() => {
  if (props.type !== 5) {
    const row = useSysModel().getSysFieldsByModelId(formDesginData.value?.modelId).filter((item: any) => item.Relate);
    const res = row.map((item: any) => {
      return {
        ...item, // 保留原始属性
        relatedKey: item.Related.split('.')[0], // 增加新属性
      };
    });
    return res;
  } else {
    return [];
  }
})

const loading = ref(false)
let timer = 0
let eventName = ''
let getValueEvent = ''
// 注册window事件
const setWindowEvent = (bool?: boolean) => {
  if (formDesginData.value?.list.length && formDesginData.value?.list.length > 0) {
    const formName = formDesginData.value?.form?.name
    if (!formName) {
      // 导出.vue时，name可以没有
      return
    }
    eventName = `get${formName}ControlByName`
    getValueEvent = `get${formName}ValueByName`
    if (formName && (!window[eventName as any] || !bool)) {
      // 根据name获取当前数据项
      // @ts-ignore
      window[eventName] = (name: string) => {
        return getNameForEach(formDesginData.value?.list, name)
      }
      // 根据name获取当前项的值
      // @ts-ignore
      window[getValueEvent] = (name: string) => {
        return formValues.value[name]
      }
    }
  }
}
watch(
  () => formDesginData.value?.config,
  () => {
    if (timer < 2) {
      setWindowEvent() // 简单判断下，这里不是每次都更新
    }
    timer++
    appendRemoveStyle(true) // 更新样式
  },
  { deep: true }
)
setWindowEvent()
// 设置全局事件结束
const resultDict = ref({})
// 处理表单值开始



// 表单组件值改变事件 tProp为子表格相关
provide(constControlChange, async ({ key, value, data, tProp }: any) => {
  if (key) {
    let fieldName = key; //默认是key
    if (tProp) {
      console.log(tProp);
      fieldName = tProp.split('.')[0];
      //setPropertyValue(tProp, value)
    } else {
      // 表格和弹性布局不是这里更新，只触change
      formValues.value[key] = value
    }
    // 校验必填状态
    console.log(key, data)

    //setValidateReqFailedStatus(fieldName, tProp, value)

    // 触发更新关联字段值
    TrigRelateFieldVals(fieldName);
    // 当表格和弹性内的字段和外面字段冲突时，可通过tProps区分
    emits('change', { key, value, model: formValues.value, data, tProp })
  }
})


// 表单组件值改变事件 tProp为子表格相关
provide(constblurEvent, async (key: any, item: any, tProp: any) => {
  // 判断是否有触发函数
  console.log('item', item, 'tProp', tProp);
  const fun = triggerList.value.find((item: any) => item.triggerKey === key);
  if (fun != null) {
    const params = {
      moduleName: fun.ActionModuleName,
      className: fun.ActionClassName,
      methodName: fun.ActionName,
      model: fun.BindingModel,
      data: formValues.value,
    }
    await executeFunc(params);
    // 返回父级组件
    emits('change', { key, model: formValues.value })
  }
})

const TrigRelateFieldVals = (key: string) => {
  const relate = relateFieldList.value.filter((item: any) => { return item.relatedKey === key });
  if (relate.length > 0) {
    relate.forEach((it: any) => {
      let relValue = formValues.value;
      for (const prop of it.Related.split('.')) {
        if (relValue && relValue.hasOwnProperty(prop)) {
          relValue = relValue[prop];
        } else {
          // 属性不存在时可以选择处理错误或提供默认值
          relValue = null;
          break;
        }
      }
      formValues.value[it.Name] = relValue;
    });
  }

}

const setFieldStatus = (data: FormList[]) => {
  if (props.type === 5 || props.type === 3) { return; }
  data.forEach((it: FormList) => {
    if (it.Relate){
      it.origReadonly = true;
    }
    else if (it.readonlyExp) {
      it.origReadonly = evaluateExpression(it.readonlyExp)
    }
    if (it.invisibleExp) {
      it.origInvisible = evaluateExpression(it.invisibleExp)
    }
    if (it.ForceRequired) {
      it.origRequired = true
    }
    else if (it.requiredExp) {
      it.origRequired = evaluateExpression(it.requiredExp)
    }
    
    if (it.child && it.child.length > 0) {
      setFieldStatus(it.child)
    }
    if (it.list) {
      setFieldStatus(it.list)
    }
  })
}

const evaluateExpression = (exp: any) => {
  try {
    if (/^[0-9]$/.test(exp)) {
      return exp.toString === '0' ? false : true
    }
    // 检查是否以 "1=1" 或 "true" 开头
    if (!exp.startsWith("[")) {
      if (/^\d+([=<>])\d+$/.test(exp) || /^(true|false)$/.test(exp)) {
        return eval(exp); // 使用eval进行简单的求值
      }
    }
    if (!exp.startsWith("[")) {
      if (/^\d+([=<>])\d+$/.test(exp) || /^(true|false)$/.test(exp)) {
        return eval(exp); // 使用eval进行简单的求值
      }
    }
    exp = exp.replace(/'/g, '"');
    if (!exp.startsWith("[[")) {
      exp = '[' + exp + ']';
    }
    return applyFilter(formValues.value, JSON.parse(exp));
  } catch (error) {
    console.error(`Expression Parsing failed:`, error);
    return false;
  }

}


const dictForm = computed(() => {
  const storage = window.localStorage.getItem('akFormDict')
  let storageDict = {}
  if (storage) {
    storageDict = JSON.parse(storage)
  }
  // 全局的、当前表单配置的以及接口返回的
  return Object.assign({}, storageDict, props.dict, resultDict.value)
})

// 调用函数
const executeFunc = async (params?: any) => {
  setLoading(true);
  await api.generalExecFunc(params).then((res) => {
    if (res.status === 200) {
      formValues.value = res.data.result.Data[0]  // 这是表单，所以只会有一行
    }
  })
  setLoading(false);
}

// 表单参数
const formProps = computed(() => {
  return {
    model: formValues.value,
    type: props.type,
    hideField: formDesginData.value?.config?.hideField as [],
    showColon: formDesginData.value?.form.showColon,
    dict: dictForm.value
  }
})
provide(constFormProps, formProps)

// 提供一个方法，用于根据name从formData.list里查找数据
const getNameForEach = (data: any, name: string) => {
  let temp = {}
  for (const key in data) {
    const dataKey = data[key]
    if (dataKey.name === name) {
      return dataKey
    }
    if (['grid', 'tabs'].includes(dataKey.type)) {
      dataKey.columns.forEach((co: any) => {
        temp = getNameForEach(co.list, name)
      })
    }
    if (['card', 'div'].includes(dataKey.type)) {
      temp = getNameForEach(dataKey.list, name)
    }
    if (dataKey.child) {
      temp = getNameForEach(dataKey.child, name)
    }
  }
  return temp
}
const getControlByName = (name: string) => {
  return getNameForEach(formDesginData.value?.list, name)
}
provide(constGetControlByName, getControlByName)
// 表单检验方法
const ruleForm = ref()

const validate = () => {
  let validateInfo = [] as any[];
  formDesginData.value?.list.forEach((it: any) => {
    if (it.origRequired && !formValues.value[it.Name]) {
      validateInfo.push({ 'Lable': it.label, 'Name': it.Name })
    }
    if (it.child && it.child.length > 0) {
      const childValue = formValues.value[it.Name];
      const reqPropertyNames = it.child.filter((t: any) => t.origRequired).map((t: any) => t.Name)
      let emptyIndexes: number[] = [];
      childValue.forEach((data: any, index: number) => {
        const isEmpty = reqPropertyNames.some((name: any) => data[name] === null || data[name] === undefined || data[name] === '');
        if (isEmpty) {
          emptyIndexes.push(index + 1);
        }
      });
      validateInfo.push({ 'Lable': it.label, 'Name': it.Name, 'Indexes': emptyIndexes })
    }
    if (it.list && it.list.length > 0) {
      it.list.forEach((t: any) => {
        if (t.origRequired && !formValues.value[t.Name]) {
          validateInfo.push({ 'Lable': it.label, 'Name': it.Name })
        }
      })
    }
  })

  return validateInfo
}
// 提供一个取值的方法
const getValue = (filter?: boolean) => {
  if (filter) {
    const obj: any = {}
    for (const key in formValues.value) {
      // eslint-disable-next-line no-prototype-builtins
      if (formValues.value.hasOwnProperty(key)) {
        const val = (formValues.value as any)[key]
        if (!/^\s*$/.test(val)) {
          obj[key] = val
        }
      }
    }
    return obj
  } else {
    return formValues.value
  }
}
// 对表单设置初始值
const setValue = (obj: { [key: string]: any }) => {
  // 分两种，false时将obj所有值合并到model，当obj有某些值不存于表单中，也会合并到model，当提交表单也会提交此值
  formValues.value = Object.assign({}, jsonParseStringify(obj)) // 防止列表直接使用set方法对弹窗表单编辑，当重置表单时当前行数据被清空
}
// 对表单设置初始值
const setDesginData = (obj: any) => {
  let temp = Object.assign({}, jsonParseStringify(obj))
  setFieldStatus(temp.list)
  formDesginData.value = temp;

}

// 对表单选择项快速设置

// 追加移除style样式
const appendRemoveStyle = (type?: boolean) => {
  try {
    const config = formDesginData.value?.config
    if (config && config.style !== undefined) {
      const { style } = config;
      appendOrRemoveStyle('formStyle', style, type);
    }
  } catch (e) {
    /* empty */
  }
}

// 按钮组件事件
provide(constFormBtnEvent, (obj: any) => {
  emits('btnClick', obj.key)
  if ([3, 4, 5].includes(props.type)) {
    return ElMessage.error('当前模式不能提交表单')
  }
  switch (obj.key) {
    case 'submit':
      break
    case 'reset':
      resetFields() // 重置
      break
    case 'cancel': // 取消返回，
      router.go(-1) //这个刷新后可能会失败
      break
  }
})

// 设置 loading 为 true，等待5秒
// 在5秒内如果接收到 setLoading(false)，则 loading 不会变为 true
let loadingTimer: NodeJS.Timeout | null = null;
const setLoading = (val: boolean) => {
  if (val === true) {
    loadingTimer = setTimeout(() => {
      if (loading.value === false) {
        loading.value = true; // 如果 5 秒内没有接收到 false，则再次将 loading 设为 true
      }
    }, 3000);
  } else {
    // 如果接收到 false，立即设置 loading 为 false，并清除定时器
    loading.value = false;
    if (loadingTimer !== null) {
      clearTimeout(loadingTimer);
      loadingTimer = null;
    }
  }
};

// 表单初始值
watch(
  () => props.value,
  (v: any) => {
    v && setValue(v)
  },
  {
    immediate: true
  }
)
// 表单设计初始值
watch(
  () => props.formData,
  (v: any) => {
    v && setDesginData(v)
  },
  {
    immediate: true
  }
)
// 表单设计初始值
watch(
  [() => formValues.value, () => props.type],
  () => {
    setFieldStatus(formDesginData.value?.list);
  },
  {
    deep: true,
    immediate: true
  }
)
watch(
  () => formDesginData.value?.list,
  (v: any) => {
    v && useDesignFormStore().setFormData(v);
  },
  {
    deep: true,
    immediate: true
  }
)

// ------------------------数据处理结束------------------------
// 重置表单方法
const resetFields = () => {
  ruleForm.value.resetFields()
  // setValue(Object.assign(model.value, obj || {})) // 这才能清空组件显示的值
}
onMounted(() => {
  //getInitModel()
  // props.value && setValue(props.value)
  nextTick(() => {
    appendRemoveStyle(true)
  })
})
onUnmounted(() => {
  if (eventName) {
    // @ts-ignore
    window[eventName] = ''
  }
  appendRemoveStyle()
})
defineExpose({
  setValue,
  getValue,
  validate,
  resetFields,
})
</script>
