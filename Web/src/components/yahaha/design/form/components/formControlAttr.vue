<!-- Created by 337547038 on 2021/6/1 0001. -->
<template>
  <div class="sidebar-tools">
    <el-tabs v-model="state.tabsName">
      <el-tab-pane label="字段配置" name="first">
        <el-form size="small" class="form">
          <template v-for="(list, index) in attrList" :key="index">
            <div class="mb5"><el-text tag="b">{{ list.title }}</el-text></div>
            <!-- <el-divider content-position="left">{{ list.title }}</el-divider> -->
            <template v-for="item in list.children" :key="item.id">
              <el-form-item :label="item.label">
                <el-select v-if="item.type === 'select' && item.dict" :placeholder="item.placeholder" v-model="item.value"
                  :filterable="true" :allow-create="item.allowCreate" :collapse-tags="true" :clearable="true"
                  @change="controlChange(item, $event)">
                  <el-option v-for="opt in item.dict" :key="opt.value" :value="opt.value" :label="opt.name" />
                </el-select>
                <el-switch v-else-if="item.type === 'switch' || item.type === 'bool'" v-model="item.value"
                  @change="controlChange(item, $event)" />
                <el-input v-else-if="item.type === 'string'" v-model="item.value" :placeholder="item.placeholder"
                  @input="controlChange(item, $event)" :readonly="item.readonly" />

                <el-input-number v-else-if="item.type === 'number'" v-model="item.value" :placeholder="item.placeholder"
                  @input="controlChange(item, $event)" :readonly="item.readonly" />
                <component v-else :is="curWidget(item.widget)" v-model="item.value" :dict="item.dict"
                  @change="controlChange(item, $event)"></component>
              </el-form-item>
            </template>
          </template>


          <template v-if="showHide(['tabs'], true)">
            <div class="h3">
              <h3>标签配置项</h3>
            </div>
            <el-form-item v-for="(item, index) in controlData.columns" :key="index">
              <el-col :span="12">
                <el-input placeholder="标签配置项" v-model="item.label" />
              </el-col>
              <el-col :span="2" :offset="1">
                <i class="icon-del" @click="delSelectOption(index as number, 'tabs')"></i>
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-button @click="addSelectOption('tabs')">增加标签</el-button>
            </el-form-item>
          </template>

          <div v-if="showHide(['grid', 'card', 'gridChild', 'divider', 'div'])">
            <div class="h3">
              <h3>其他属性</h3>
            </div>


            <el-tooltip :content="state.tooltip.props" placement="top">
              <el-button size="small" @click="openAttrDialog('', state.tooltip.props)">编辑属性 </el-button>
            </el-tooltip>
          </div>

        </el-form>
      </el-tab-pane>
      <el-tab-pane label="表单配置" name="second">
        <el-form size="small" class="form">
          <el-form-item v-for="(item, index) in formAttr.filter(item => !item.hide)" :label="item.label" :key="index">
            <el-select v-if="item.type === 'select'" v-model="item.value" :filterable="item.key === 'class'"
              :allow-create="item.key === 'class'" :placeholder="item.placeholder" :clearable="item.clearable"
              @change="formAttrChange(item)">
              <el-option :label="opt.label" v-for="opt in item.options" :key="opt.label"
                :value="formatNumber(opt.value)" />
            </el-select>
            <el-switch v-else-if="item.type === 'switch'" v-model="item.value" @input="formAttrChange(item)" />
            <el-input v-else v-model="item.value" :placeholder="item.placeholder" @input="formAttrChange(item)"
              :readonly="item.readonly" />

          </el-form-item>
          <el-form-item v-if="!state.isSearch">
            <template #label>添加时获取请求
              <el-tooltip content="新增表单数据时，从接口获取新增初始数据" placement="top">
                <el-icon>
                </el-icon>
              </el-tooltip>
            </template>
            <el-switch v-model="formConfig.addLoad" @change="
              formAttrChange({ key: 'addLoad', path: 'config' }, $event)
              " />
          </el-form-item>
          <template v-if="!state.isSearch">
            <div class="h3">
              <h3>接口数据事件</h3>
            </div>
            <el-form-item class="event-btn">
              <el-button @click="
                eventClick(
                  'beforeRequest',
                  '获取表单初始数据前事件，可修改请求参数'
                )
                ">beforeRequest
              </el-button>
              <el-button @click="
                eventClick(
                  'afterResponse',
                  '获取表单初始数据后事件，可对请求返回数据进行处理；也可为字符串，如opt=formatTest'
                )
                ">afterResponse
              </el-button>
              <el-button @click="
                eventClick(
                  'beforeSubmit',
                  '表单数据提交前事件，可对提交数据进行处理；也可为字符串，如opt=formatTest'
                )
                ">beforeSubmit
              </el-button>
              <el-button @click="eventClick('afterSubmit', '表单数据提交成功事件')">afterSubmit
              </el-button>
              <el-button @click="
                eventClick(
                  'change',
                  '表单组件值改变事件。当表单某值改变时，可修改其他组件的值；也可为字符串，如opt=formChange,字符串即为/utils/formChangeValue(name,model,key)中的key值'
                )
                ">表单组件改变事件change
              </el-button>
            </el-form-item>
          </template>
        </el-form>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script lang="ts" setup>
import { reactive, computed, toRefs, ref, watch, inject } from 'vue'
//import { useRoute } from 'vue-router'
import { useDesignFormStore } from '/@/stores/designForm'
import { useSysModel } from '/@/stores/sysModel';
import { formatNumber, deepClone, readWidgetOptions } from '../../utils/'
import { getFieldData, applyFilter } from '../../utils/applyFilter'
import { baseAttrItem, formAttrItem, otherAttrItem } from './controlBaseAttr'
import getOpationWidget from './widgets/opationWidget/getOpationWidget'
const props = withDefaults(
  defineProps<{
    formData: any
    formConfig: any
    formOtherData: any
  }>(),
  {
    formConfig: () => {
      return {}
    },
    formOtherData: () => {
      // 其他不需要保存在formData里的数据
      return {}
    }
  }
)
const emits = defineEmits<{
  (e: 'openDialog', data: any): void
  (e: 'update:formOtherData', data: any): void
  //(e: 'update:formData', data: any): void
  //(e: 'update:formConfig', data: any): void
}>()
const { formConfig, formData } = toRefs(props)
const store = useDesignFormStore() as any

const controlData = computed({
  get() {
    return store.controlAttr
  },
  set(newVal: any) {
    store.setControlAttr(newVal);
  }
})

const dataSourceOption = ref<any[]>()

const curWidget = (name: string) => {
  //写的时候，组件的起名一定要与dragList中的element名字一模一样，不然会映射不上
  return getOpationWidget[name]
}

const widgetOptions = computed(() => {
  return readWidgetOptions()
})

const widgetList = computed(() => {
  const temp = widgetOptions.value;
  return temp.filter((item: any) => {
    if (item.fieldType.includes('*')) {
      return true
    }
    else {
      return item.fieldType.includes(controlData.value.tType)
    }
  }).map((item: any) => ({
    name: item.description,
    value: item.name,
  })) as any[];
})

/**加载所选择的组件配置项 */
const widgetAttrList = computed(() => {
  const result = widgetOptions.value.find((item: { name: any; }) => item.name === controlData.value.widget);
  result?.options.forEach((item: any) => {
    // 获取dict
    if (item.dictPath) {
      item.dict = deepClone(controlData.value[item.dictPath])
    }
    // 获取值
    if (item.path) {
      item.value = getFieldData(controlData.value, item.path);
    } else if (item.key) {
      item.value = controlData.value.config[item.key];
    }
    // 当属性不存在对应key则赋予初始值
    if (!(item.key in controlData.value.config)) {
      if (item.type === 'bool') {
        controlData.value.config[item.key] = false;
      } else if (item.type === 'number') {
        controlData.value.config[item.key] = 0;
      }
      else {
        controlData.value.config[item.key] = undefined;
      }
      // 默认
      if (item.default) {
        item.value = item.default;
        controlData.value.config[item.key] = item.default;
      }
    }
  })

  // 隐藏
  if (result) {
    return result?.options.filter((item: { show: any[]; key: string | number; }) => {
      if (item.show) {
        const conditionsMet = item.show.every((condition: { [x: string]: any; }) => {
          return Object.keys(condition).every(key => {
            return controlData.value.config[key] === condition[key];
          });
        });

        if (!conditionsMet) {
          controlData.value.config[item.key] = undefined;
        }
        return conditionsMet
      }
      return true;
    });
  } else {
    return []
  }
})

const formAttr = computed(() => {
  let temp: any[] = [];
  formAttrItem.forEach((it: any) => {
    temp.push({
      ...it,
      value: getFieldData(formData.value, it.path) || null, // 增加新属性
    })
  })
  return temp;
})

const baseAttr = computed(() => {
  let temp: any[] = [];
  baseAttrItem.forEach((it: any) => {
    let isShow = true;
    if (it.show) {
      isShow = applyFilter(controlData.value, it.show)
    }
    if (isShow) {
      temp.push({
        ...it,
        value: getFieldData(controlData.value, it.path) || null, // 设定值
        dict: it.path === 'widget' ? widgetList.value : null // 设置小部件选项
      })
    }
  })
  return temp;
})

const otherAttr = computed(() => {
  const temp = otherAttrItem.map((item: any) => {
    return {
      ...item,
      value: getFieldData(controlData.value, item.path) || null, // 设定值
    };
  });
  return temp;
})

const attrList = computed(() => {
  const res: { title: string, children: any }[] = [];
  if (Object.keys(controlData.value).length) {
    res.push({ title: '通用属性', children: baseAttr.value });
    res.push({ title: '组件属性', children: widgetAttrList.value });
    res.push({ title: '其他属性', children: otherAttr.value });
  }
  return res
})

const designType = inject('formDesignType')
const state = reactive({
  dataSourceList: [],
  customRulesList: [
    {
      type: 'rules',
      label: '自定义正则'
    },
    {
      type: 'methods',
      label: '自定义方法'
    }
  ], // 自定义校验规则
  isSearch: designType === 'search',
  tooltip: {
    css: '当前表单应用页的样式，类似于.vue文件中的style scoped中的样式',
    dict: '数据字典，用于匹配多选组、下拉选择等，提供动态获取Options接口字典数据，一般不设置，从接口dict获取。json格式："sex":{"0":"男","1":"女"}',
    rules:
      "可参考UI组件表单校验，<a href='https://element-plus.gitee.io/zh-CN/component/form.html#%E8%A1%A8%E5%8D%95%E6%A0%A1%E9%AA%8C' target='_blank' style='color:red'>详情点击</a>",
    props: '可添加当前组件所有prop属性及事件方法'
  },
  tabsName: 'second'
})
watch(
  () => store.activeKey,
  (val: string) => {
    if (val) {
      // 有值时自动跳到第一项
      state.tabsName = 'first'
    }
  }
)
const controlChange = (obj: any, val: any) => {
  // select多选属性，
  switch (obj.format) {
    case 'json':
      break
  }
  if (obj.path) {
    const newVal = obj.isNum ? formatNumber(val) : val // 类型为数字时转整数
    obj.path && getPropByPath(controlData.value, obj.path, newVal)
  }
  if (obj.key) {
    obj.key && setPropByKey(controlData.value, obj, val)
  }
  getIsLayout()
}

const getIsLayout = () => {
  const result = widgetOptions.value.find((item: { name: any; }) => item.name === controlData.value.curWidget);
  controlData.value.isLayout = result?.isLayout ?? false;
}

const setPropByKey = (obj: any, opation: any, val: any) => {
  let tempObj = obj
  const key = opation.key;
  let newVal = val;
  if (opation.type === 'number') {
    newVal = formatNumber(val)
  }
  newVal = newVal === undefined ? null : newVal
  if (Array.isArray(controlData.value[key])) {
    tempObj['config'][key] = [];
    newVal.forEach((item: any) => {
      tempObj['config'][key].push(item);
    })
  } else {
    tempObj['config'][key] = newVal;
  }
}


// 修改指定路径下的值
const getPropByPath = (obj: any, path: string, val: any) => {
  let tempObj = obj
  const keyArr = path.split('.')
  let i = 0
  for (i; i < keyArr.length - 1; i++) {
    const key = keyArr[i]
    if (key in tempObj) {
      tempObj = tempObj[key]
    } else {
      throw new Error(`${key} is undefined`)
      // break
    }
  }
  const key = keyArr[i]
  const value = tempObj[keyArr[i]]
  // 检查最后一级是否存在
  /*if (!(key in tempObj)) {
  throw new Error(`${key} is undefined`)
}*/
  if (val !== undefined) {
    tempObj[key] = val
  }
  return {
    obj: tempObj,
    key: key,
    value: value
  }
}
// 属性设置相关结束
// 多选固定选项删除
const delSelectOption = (index: number, type?: string) => {
  if (type === 'tabs') {
    controlData.value.columns.splice(index, 1)
  } else {
    controlData.value.options.splice(index, 1)
  }
}
// 多选固定选项增加
const addSelectOption = (type: string) => {
  if (controlData.value.widget === 'cascader') {
    // 级联时打开弹窗口
    openAttrDialog('cascader')
  } else if (controlData.value.widget === 'treeSelect') {
    openAttrDialog('treeSelect', '编辑组件下拉选项数据')
  } else {
    if (type === 'tabs') {
      controlData.value.columns.push({
        label: '标签名称',
        list: []
      })
    } else {
      controlData.value.options.push({
        label: '',
        value: ''
      })
    }
  }
}
// 更多属性弹窗
const openAttrDialog = (type?: string, tooltip?: string) => {
  let editData = controlData.value.control
  if (controlData.value.widget === 'button') {
    // 按钮组件编辑属性
    editData = controlData.value.config
    type = 'button'
  }
  switch (type) {
    case 'treeSelect':
      editData = controlData.value.control.data
      break
    case 'cascader':
      editData = controlData.value.options
      break
    case 'optionsParams': // 选项请求附加参数
      editData = controlData.value.config.beforeRequest
      // params.codeType = 'json'
      break
    case 'optionsResult':
      editData = controlData.value.config.afterResponse
      break
  }
  const emitsParams = {
    content: editData,
    title: tooltip,
    type: type,
    direction: 'ltr',
    callback: (result: any) => {
      switch (type) {
        case 'treeSelect':
          controlData.value.control.data = result
          break
        case 'cascader':
          controlData.value.options = result
          break
        case 'optionsParams':
          controlData.value.config.beforeRequest = result
          break
        case 'optionsResult':
          controlData.value.config.afterResponse = result
          break
        case 'button':
          controlData.value.config = result
          break
        default:
          controlData.value.control = {}
          Object.assign(controlData.value.control, result)
      }
    }
  }
  emits('openDialog', emitsParams)
}

// 根据不同类型判断是否显示当前属性
const showHide = (type: string[], show?: boolean) => {
  // show=true 条件成立显示，false符合条件隐藏
  if (
    (type && type.length === 0) ||
    Object.keys(controlData.value).length === 0
  ) {
    return false
  }
  const index = type.indexOf(controlData.value.widget)
  return show ? index !== -1 : index === -1
}

const getDataSource = async () => {
  const res = useSysModel().sysModelList;
  dataSourceOption.value = res.map((it: any) => ({
    value: it.Id,
    label: it.Description + '[' + it.Name + ']'
  }))
}

// 表单属性修改
const formAttrChange = (obj: any, val?: any) => {
  if (obj && obj.value) {
    console.log(obj);
    formData.value[obj.path] = obj.value || val
  }
}

watch(
  () => formData.value,
  (v: any) => {
    v && store.setFormAttr(v);
  },
  {
    deep: true,
    immediate: true
  }
)

const eventClick = (type: string, tooltip?: string) => {
  emits('openDialog', { type: type, title: tooltip, direction: 'ltr' })
}
getDataSource()
</script>
