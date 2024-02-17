export const EDITTYPE = 'javascript' // 弹出编辑器可输入类型 json/javascript
import jsBeautify from 'js-beautify'

export const aceEdit = (data: any, id?: string, type?: string | undefined) => {
  id = id || 'editJson'
  type = type || EDITTYPE
  // @ts-ignore
  const editor = ace.edit(id)
  editor.setOptions({
    enableBasicAutocompletion: true,
    enableSnippets: true,
    enableLiveAutocompletion: true
  })
  editor.setFontSize(14)
  editor.setShowPrintMargin(false)
  editor.session.setMode('ace/mode/' + type)
  editor.setTheme('ace/theme/tomorrow_night')
  editor.setValue(data)
  return editor
}

export function evil(fn: any) {
  const Fn = Function // 一个变量指向Function，防止有些前端编译工具报错
  return new Fn('return ' + fn)()
}

export const key = () => {
  const timestamp = new Date().getTime().toString(); // 获取当前时间戳
  const random = Math.random().toString(8).slice(2);
  return 'yahaha' + timestamp + random;
};

// 定义两个空方法，用于在编辑事件时作为默认值
export const beforeRequest =
  'opt=(data, route) => {\n' +
  '  // data经过处理后返回\n' +
  "  console.log('beforeRequest',data)\n" +
  '  return data\n' +
  '}'
export const afterResponse =
  'opt=(res) => {\n' +
  '  // res返回数据\n' +
  "  console.log('afterResponse',res)\n" +
  '  return res\n' +
  '}'

export const onChange =
  'opt=(key,model) => {\n' +
  '  // name当前改变组件的值,model表单的值\n' +
  "  console.log('onChange',key)\n" +
  '  return model\n' +
  '}'

export function debounce<T extends (...args: any[]) => void>(func: T, delay = 500, immediate?: boolean): T {
  let timerId: any

  return function (this: any, ...args: any[]) {
    if (timerId) {
      clearTimeout(timerId)
    }
    if (immediate) {
      const callNow = !timerId
      timerId = setTimeout(() => {
        timerId = null
      }, delay)
      if (callNow) {
        func.apply(this, args)
      }
    } else {
      timerId = setTimeout(() => {
        func.apply(this, args)
      }, delay)
    }
  } as T
}
// 时间格式化
export const dateFormatting = (time: any, cFormat?: string) => {
  const format = cFormat || '{y}-{m}-{d} {h}:{i}:{s}'
  // 字符串数字形式的时间戳要转换下
  let newTime = time
  if (/^\d+?$/.test(time)) {
    newTime = parseInt(time)
  }
  const date = typeof time === 'object' ? time : new Date(newTime)
  const formatObj: any = {
    y: date.getFullYear(),
    m: date.getMonth() + 1,
    d: date.getDate(),
    h: date.getHours(),
    i: date.getMinutes(),
    s: date.getSeconds(),
    w: date.getDay()
  }
  return format.replace(/{(y|m|d|h|i|s|w)+}/g, (result, key) => {
    let value = formatObj[key]
    if (key === 'w') {
      return ['日', '一', '二', '三', '四', '五', '六'][value]
    }
    if (result.length > 0 && value < 10) {
      value = '0' + value
    }
    return value || 0
  })
}
// 动态远程加载script脚本
export function loadScript(src: string) {
  return new Promise((resolve, reject) => {
    const script = document.createElement('script')
    script.type = 'text/javascript'
    script.onload = resolve
    script.onerror = reject
    script.src = src
    document.head.appendChild(script)
  })
}
// 随机数字符串
export const randomString = (len: number) => {
  len = len || 32
  const str = 'ABCDEFGHIJKMNOPQSTWXYZabcdefghijklmnopqrstwxyz1234567890'
  let n = ''
  for (let i = 0; i < len; i++) {
    n += str.charAt(Math.floor(Math.random() * str.length))
  }
  return n
}
/**对象转新对象 */
export const jsonParseStringify = (val: any) => {
  if (typeof val === 'object') {
    return JSON.parse(JSON.stringify(val))
  } else {
    return val
  }
}

/**给关联字段赋值 
 * @key 字段名
 * @data 待处理数据
 * @relateFieldList 关联列表
*/
export const TrigRelateFieldVals = (data: any, relateFieldList: any, key: string) => {
  const relate = relateFieldList.value.filter((item: any) => { return item.relatedKey === key });
  if (relate.length > 0) {
    relate.forEach((it: any) => {
      let relValue = data;
      for (const prop of it.Related.split('.')) {
        if (relValue && relValue.hasOwnProperty(prop)) {
          relValue = relValue[prop];
        } else {
          // 属性不存在时可以选择处理错误或提供默认值
          relValue = null;
          break;
        }
      }
      data[it.Name] = relValue;
    });
  }
}

export const hasKey = (obj: any, path: string) => {
  const keys = path.split('.');
  let currentObj = obj;

  for (const key of keys) {
    if (!currentObj || !currentObj.hasOwnProperty(key)) {
      return false;
    }
    currentObj = currentObj[key];
  }

  return true;
}

/****
 * 动态插入移除css
 * @param id 标签id
 * @param cssContent 要插入的css内容
 * @param append true插入false移除
 */
export const appendOrRemoveStyle = (
  id: string,
  cssContent: string,
  append?: boolean
) => {
  const styleId: any = document.getElementById(id)
  if (styleId && append) {
    // 存在时直接修改，不用多次插入
    styleId.innerText = cssContent
    return
  }
  if (cssContent && append) {
    const styleEl = document.createElement('style')
    styleEl.id = id
    styleEl.type = 'text/css'
    styleEl.appendChild(document.createTextNode(cssContent))
    document.head.appendChild(styleEl)
  }
  if (!append || !cssContent) {
    // 移除
    styleId && styleId.parentNode.removeChild(styleId)
  }
}

export const emptyToNull = (val: any) => {
  return val === "" ? null : val;
}

export const isEmptyRoNull = (val: any) => {
  if (typeof val === 'string' && (val !== null || val !== undefined)) {
    if (val.trim() !== '' || val.length !== 0) {
      return false
    }
  } else {
    return true
  }
}

/**对象转新对象(新的) */
export function deepClone(obj: any, cache: WeakMap<object, any> = new WeakMap()): any {
  if (obj === null || typeof obj !== 'object') return obj;
  if (obj instanceof Date) return new Date(obj);
  if (obj instanceof RegExp) return new RegExp(obj);

  if (cache.has(obj)) return cache.get(obj); // 如果出现循环引用，则返回缓存的对象，防止递归进入死循环
  const cloneObj = new obj.constructor(); // 使用对象所属的构造函数创建一个新对象
  cache.set(obj, cloneObj); // 缓存对象，用于循环引用的情况

  for (const key in obj) {
    if (obj.hasOwnProperty(key)) {
      cloneObj[key] = deepClone(obj[key], cache); // 递归拷贝
    }
  }
  return cloneObj;
}

export function keepOnlyId(obj: any) {
  // 检查是否为对象
  if (typeof obj !== 'object' || obj === null || obj === undefined) {
    return obj;
  }

  // 检查是否具有 Id 属性
  if (!('Id' in obj)) {
    return obj;
  }

  return { Id: formatNumber(obj.Id) };
}

export function obj2string(o: any) {
  let r: any = []
  if (o === null) {
    // 这里有个问题 因typeOf null=object,下面判断会报错
    return null
  }
  if (typeof o === 'string') {
    return (
      '"' +
      o
        .replace(/([\\'\\"\\])/g, '\\$1')
        .replace(/(\n)/g, '\\n')
        .replace(/(\r)/g, '\\r')
        .replace(/(\t)/g, '\\t') +
      '"'
    )
  }
  if (typeof o === 'object') {
    if (!o.sort) {
      for (const i in o) {
        if (o.hasOwnProperty(i)) {
          let iii = i
          if (i.indexOf('-') !== -1) {
            iii = `"${i}"`
          }
          r.push(iii + ':' + obj2string(o[i]))
        }
      }
      if (
        !!document.all &&
        !/^\n?function\s*toString\(\)\s*\{\n?\s*\[native code\]\n?\s*\}\n?\s*$/.test(
          o.toString
        )
      ) {
        r.push('toString:' + o.toString.toString())
      }
      r = '{' + r.join() + '}'
    } else {
      for (let i = 0; i < o.length; i++) {
        r.push(obj2string(o[i]))
      }
      r = '[' + r.join() + ']'
    }
    return r
  }
  return o && o.toString()
}


import type { widgetConfig } from '../types'

/**读取所有组件配置项 */
export const readWidgetOptions = () => {
  //const template = import.meta.globEager('./template/*.ts')
  const opation = import.meta.glob('../widgets/opations/*.ts', { eager: true })
  const temp: widgetConfig[] = [];
  Object.keys(opation).forEach((key: string) => {
    const file: any = opation[key]
    temp.push({
      name: file.name,
      description: file.description,
      fieldType: file.fieldType,
      isLayout: file.isLayout ? file.isLayout : false,
      options: file.options,
    })
  })
  return temp;
}

// 判断并添加属性
export const addKeyIfNotExists = (obj: any, key: string, val?: any) => {
  if (!(key in obj)) {
    obj[key] = val || null;
  }
}


// 将字符类数字转为数值类
export const formatNumber = (val: any) => {
  // 将字符类数字转为数值类
  if (typeof val === 'string' && /^\d+(\.\d+)?$/.test(val.toString())) {
    // 为数字
    return Number(val)
  } else {
    return val
  }
}
// 将{key:value}转[{label:'key',value:'value'}]
export const objectToArray = (obj: any) => {
  if (Object.prototype.toString.call(obj) === '[object Object]') {
    const temp: any = []
    for (const key in obj) {
      temp.push({
        label: obj[key],
        value: key
      })
    }
    return temp
  }
  return obj
}
export const parseStringToObject = (input: string) => {
  try {
    const keyValue = input.split(':');
    if (keyValue.length === 2) {
      const key = keyValue[0].trim();
      const value = keyValue[1].trim();
      return { [key]: value };
    }
    return null;
  } catch (error) {
    return null;
  }
}

/**
 * 设置或获取local session storage
 * @param key
 * @param data 有值时set，否则get
 * @param type local/session默认
 */
export const getSetStorage = (key: string, data?: string, type = 'session') => {
  //console.log(key, data)
  const winType = type === 'session' ? 'sessionStorage' : 'localStorage'
  if (data) {
    window[winType].setItem(key, data)
  } else {
    return window[winType].getItem(key)
  }
}


export function stringToObj(string: string) {
  if (EDITTYPE === 'javascript') {
    return evil(string)
  } else {
    return JSON.parse(string)
  }
}

export function string2json(string: string) {
  return JSON.parse(string || '{}')
}

export function json2string(obj: any, isBeautify?: boolean) {
  return isBeautify ? JSON.stringify(obj, null, 2) : JSON.stringify(obj)
}

export function objToStringify(obj: any, isBeautify?: boolean) {
  if (EDITTYPE === 'javascript') {
    if (isBeautify) {
      return jsBeautify('opt=' + obj2string(obj), {
        indent_size: 2,
        brace_style: 'expand'
      })
    } else {
      return obj2string(obj)
    }
  } else {
    return isBeautify ? JSON.stringify(obj, null, 2) : JSON.stringify(obj)
  }
}



// provide 方法定义的key
const prefix = 'yahaha'
export const constControlChange = prefix + 'ControlChange' // 表单组件改变事件
export const constSetFormOptions = prefix + 'SetFormOptions' // 使用setOptions设置下拉值
export const constGetControlByName = prefix + 'GetControlByName' // 根据name从formData.list查找数据
export const constFormBtnEvent = prefix + 'FormBtnEvent' // 按钮组件事件
export const constFormProps = prefix + 'FormProps' // 按钮组件事件
export const constblurEvent = prefix + 'BlurEvent' // 表单字段释放事件