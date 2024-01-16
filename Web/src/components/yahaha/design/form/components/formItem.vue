<!-- Created by 337547038 on 2021/9/27. -->
<template>
  <template v-if="data.isLayout">
    <component :is="curWidget(data.type)" :widgetConfig="data" v-model="value"></component>
  </template>
  <el-form-item v-else v-bind="data.item" :prop="tProp || data.name" :class="config.className" :rules="itemRules"
    :label="getLabel(data.item as FormItem)">
    <template #label>
      <el-tooltip :disabled="isEmptyRoNull(config.help)" :content="config.help" placement="top">
        <el-text class="form-item-lable" tag="b">{{ getLabel(data.item) }}</el-text>
      </el-tooltip>
    </template>

    <div class="form-value" v-if="type === 4" v-html="value"></div>
    <template v-else>
      <el-upload class="upload-style" v-if="data.type === 'upload'" :action="uploadUrl" v-bind="control"
        :name="control.file || 'file'" :disabled="editDisabled" :file-list="fileList" :class="{
          limit: control.limit <= fileList.length
        }" :on-error="uploadError" :on-success="uploadSuccess" :on-remove="uploadRemove">
        <el-button type="primary" v-if="config?.btnText">{{ config?.btnText }}
        </el-button>
        <i class="icon-plus" v-else></i>
        <template #tip v-if="config?.tip">
          <div class="el-upload__tip">
            {{ config?.tip }}
          </div>
        </template>
      </el-upload>
      <template v-if="data.type === 'tinymce'">
        <!--  设计模式时拖动会出现异常，设计模式暂用图片代替-->
        <tinymce-edit v-bind="control" :config="config" :disabled="editDisabled" v-model="value"
          v-if="[1, 2, 3].includes(type as number)" />
        <img alt="" src="./tinymce.png" v-if="type === 5" style="max-width: 100%" />
      </template>

      <template v-else>
        <!--  设计模式时拖动会出现异常，设计模式暂用图片代替-->
        <component :is="curWidget(data.type)" :widgetConfig="data" v-model="value" @blur="InvokeTriggeredEvent">
        </component>
      </template>

    </template>
  </el-form-item>
</template>

<script lang="ts" setup>
import {
  inject,
  onMounted,
  computed,
  watch,
  ref,
  onUnmounted,
} from 'vue'
import md5 from 'md5'
import { ElMessage } from 'element-plus'
import TinymceEdit from './tinymce.vue'
import { FormItem, FormList } from '../../types'
import validate from './validate'
import getWidget from './widgets/getWidget'
import {
  constControlChange,
  constSetFormOptions,
  constFormProps,
  constTriggeredEvent,
  objectToArray, isEmptyRoNull, debounce
} from '../../utils/'
import { uploadUrl } from '/@/api/model/';
import { useRoute } from 'vue-router'
import formatResult from '../../utils/formatResult'
import { getRequest } from '/@/api/model/';

const props = withDefaults(
  defineProps<{
    data: FormList
    modelValue?: any // 子表和弹性布局时时有传
    tProp?: string // 子表时的form-item的prop值，用于子表校验用
  }>(),
  {}
)
const emits = defineEmits<{
  (e: 'update:modelValue', val: any): void
}>()
const route = useRoute()
const formProps = inject(constFormProps, {}) as any
const type = computed(() => {
  return formProps.value.type
})

const config = computed(() => {
  return props.data.config || {}
})
// const control = ref(props.data.control)
const control = computed(() => {
  return props.data.control
})
const options = ref(props.data.options)
const changeEvent = inject(constControlChange, '') as any
const updateModel = (val: any) => {
  changeEvent &&
    changeEvent({
      key: props.data.name,
      value: val,
      data: props.data,
      tProp: props.tProp
    })
}
const triggeredEvent = inject(constTriggeredEvent, '') as any
const InvokeTriggeredEvent = () => {
  triggeredEvent &&
    triggeredEvent(
      props.data.name
    )
}

const value = computed({
  get() {
    if (props.tProp) {
      // 表格和弹性布局
      return props.modelValue
    } else {
      return formProps.value.model[props.data.name]
    }
  },
  set(newVal: any) {
    if (props.tProp) {
      emits('update:modelValue', newVal)
    }
    updateModel(newVal)
  }
})


const curWidget = (name: string) => {
  //写的时候，组件的起名一定要与dragList中的element名字一模一样，不然会映射不上
  return getWidget[name]
}

// 当通用修改属性功能添加新字段时，数组更新但toRefs没更新

const sourceFunKey = computed(() => {
  const iReg = new RegExp('(?<=\\${)(.*?)(?=})', 'g')
  //const iReg = new RegExp('\\${.*?}', 'g') // 结果会包含开头和结尾=>${name}
  const apiUrl = config.value.optionsFun
  const replace = apiUrl?.match(iReg)
  return replace && replace[0]
})
const getLabel = (ele: FormItem | undefined) => {
  const showColon = formProps.value.showColon ? ':' : ''
  if (ele) {
    return ele.showLabel ? '' : ele.label + showColon
  } else {
    return ''
  }
}
// 控制编辑模式下是否可用
const editDisabled = computed(() => {
  if (type.value === 3) {
    return true // 查看模式，为不可编辑状态
  }
  if (type.value === 1 && config.value.addDisabled) {
    return true
  }
  if (type.value === 2 && config.value.editDisabled) {
    return true // 编辑模式
  }
  return control.value.disabled
})
// 返回当前item项的校验规则
const itemRules = computed(() => {
  let temp
  const itemR: any = props.data.item?.rules || []
  const customR = formatCustomRules()
  // 如果三个都没有设置，则返回undefined
  if (itemR?.length || customR?.length) {
    temp = [...customR, ...itemR]
  }
  return temp
})
// data 根据条件搜索，select远程搜索里data有值
const getAxiosOptions = debounce((data?: any) => {
  const {
    optionsType,
    optionsFun,
    method = 'post',
    afterResponse,
    beforeRequest,
    label,
    value,
    debug // =true可用于调试，不存sessionStorage
  } = config.value
  if (optionsType !== 0) {
    let sourceFun = optionsFun
    // 接口数据源
    if (optionsType === 1 && sourceFun) {
      // 当前控件为动态获取数据，防多次加载，先从本地取。data=true时直接请求
      const key = 'getOptions_' + props.data.name + md5(sourceFun + data)
      const storage = window.sessionStorage.getItem(key)
      if (storage && !data && !debug) {
        const val = JSON.parse(storage)
        if (props.data.type === 'treeSelect') {
          control.value.data = val
        } else {
          options.value = val
        }
      } else {
        // 从url里提取一个动态值,${name}形式提取name
        if (sourceFunKey.value) {
          const val = formProps.value.model[sourceFunKey.value]
          const string = '${' + sourceFunKey.value + '}'
          sourceFun = sourceFun.replace(string, val)
        }
        // 处理请求前的数据
        //let newData = Object.assign({}, data || {}, queryParams)
        let newData = data || {}
        if (typeof beforeRequest === 'function') {
          newData =
            beforeRequest(newData, route, formProps.value.model) ?? data
        }
        if (newData === false) {
          return
        }
        if (method === 'get') {
          newData = { params: newData }
        }
        getRequest(sourceFun, newData, { method: method })
          .then((res: any) => {
            const result = res.data.list || res.data
            let formatRes: any = result
            // 这里做数据转换，很多时候后端并不能提供完全符合且一样的数据
            if (typeof afterResponse === 'string' && afterResponse) {
              formatRes = formatResult(result, afterResponse)
            } else if (typeof afterResponse === 'function') {
              // 没有return时，使用原来的，相当于没处理
              formatRes = afterResponse(result) ?? result
            } else if (label || value) {
              // 没有设置afterResponse时，这里将数据转换为[{label:'',value:''}]形式。只处理一级
              formatRes = []
              result.forEach((item: any) => {
                formatRes.push({
                  label: item[label] || item.label,
                  value: item[value] || item.value
                })
              })
            }
            if (formatRes === false) {
              return
            }
            // console.log('formatRes', formatRes)
            if (props.data.type === 'treeSelect') {
              control.value.data = formatRes
            } else {
              options.value = formatRes
            }
            if (typeof formatRes === 'object') {
              window.sessionStorage.setItem(key, JSON.stringify(formatRes)) //缓存，例如子表添加时不用每添加一行就请求一次
            }
          })
          .catch((res: any) => {
            if (props.data.type === 'treeSelect') {
              control.value.data = []
            } else {
              options.value = []
            }
            console.log(res)
          })
      }
    }
    setFormDict(formProps.value.dict) // 表格里新增时行时需要重新设一次
  }
})
watch(
  () => formProps.value.model[sourceFunKey.value],
  () => {
    getAxiosOptions()
  }
)
// 处理自定义校验规则，将customRules转换后追加到rules里
const formatCustomRules = () => {
  const rulesReg: any = {}
  validate &&
    validate.forEach(item => {
      rulesReg[item.type] = item.regExp
    })

  // 获取校验方法 父级使用provide方法注入
  const temp: any = []
  props.data.customRules?.forEach((item: any) => {
    if (!item.message && item.type !== 'methods') {
      return // 方法时允许提示信息为空
    }
    let obj = {}
    if (item.type === 'required') {
      obj = { required: true }
    } else if (item.type === 'rules') {
      // 自定义表达式
      obj = { pattern: item.rules }
    } else if (item.type === 'methods') {
      // 方法时
      const methods: any = item.methods
      if (methods) {
        obj = { validator: inject(methods, {}) }
      }
    } else if (item.type) {
      obj = { pattern: rulesReg[item.type as string] }
    }
    // 这里判断下防某些条件下重复push的可能或存重复校验类型
    let message: any = { message: item.message }
    if (!item.message) {
      // 当使用validator校验时，如果存在message字段则不能使用 callback(new Error('x'));的提示
      message = {}
    }
    temp.push(
      Object.assign(
        {
          trigger: item.trigger || 'blur'
        },
        obj,
        message
      )
    )
  })
  return temp
}
// 从数据接口获取数据设置options，在表单添加或编辑时数据加载完成
const setFormDict = (val: any) => {
  if (val && config.value.optionsType === 2) {
    const opt = val[config.value.optionsFun] || val[props.data.name] // 不填写默认为当前字段名
    if (opt !== undefined) {
      options.value = objectToArray(opt)
    }
  }
}
// 从接口返回的dict会在这里触发
// watch(
//   () => formProps.value.dict,
//   (val: any) => {
//     setFormDict(val)
//   },
//   {
//     /*deep: true*/
//   }
// )
// 对单选多选select设置options
const formOptions = inject(constSetFormOptions, {}) as any
watch(
  () => formOptions.value,
  (val: any) => {
    const opt = val[props.data.name]
    // 子表内的需要注意下，只有在子表有记录时才生效
    if (val && opt !== undefined) {
      if (props.data.type === 'treeSelect') {
        // 树结构的参数为data
        control.value.data = objectToArray(opt)
      } else {
        options.value = objectToArray(opt)
      }
    }
  }
)
// ------------图片上传处理-----------
const fileList = computed<any>(() => {
  const imgVal = formProps.value.model[props.data.name]
  if (imgVal && typeof imgVal === 'string') {
    const temp: any = []
    imgVal.split(',').forEach((item: string) => {
      temp.push({
        name: item,
        url: item
      })
    })
    return temp
  }
  return imgVal || [] // 这样可支持默认值为array([name:'',url:''这种形式])
})
// 上传成功时
const uploadSuccess = (response: any, uploadFile: any, uploadFiles: any) => {
  const oldList = []
  fileList.value.forEach((item: any) => {
    oldList.push(item.url)
  })
  oldList.push(response.path)
  updateModel(oldList.join(','))
  control.value.onSuccess &&
    control.value.onSuccess(response, uploadFile, uploadFiles)
}
// 从列表移除
const uploadRemove = (uploadFile: any, uploadFiles: any) => {
  const oldList: any = []
  fileList.value.forEach((item: any) => {
    if (item.url !== uploadFile.url) {
      oldList.push(item.url)
    }
  })
  updateModel(oldList.join(','))
  control.value.onRemove && control.value.onRemove(uploadFile, uploadFiles)
  // todo 需从服务端删除已上传图片时，这里需要发删除请求接口
}
// 上传错误
const uploadError = (err: any, file: any, fileList: any) => {
  // console.log('uploadError')
  ElMessage.error(file.name + '上传失败')
  control.value.onError && control.value.onError(err, file, fileList)
}
// -------------图片上传结束----------------

// treeSelect
// const filterMethod = (val: string) => {
//   if (props.data.type === 'treeSelect') {
//     // 请求参数名，可使用config.queryName传进来
//     const queryName = config.value.queryName || 'name'
//     control.value.filterMethod && control.value.filterMethod(val)
//     getAxiosOptions({ [queryName]: val })
//   }
// }
onMounted(() => {
  getAxiosOptions()
})
onUnmounted(() => { })
</script>
<style scoped>
.form-item-lable {
  color: var(--el-text-color-primary);
}
</style>