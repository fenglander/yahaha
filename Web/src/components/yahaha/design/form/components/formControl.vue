<!-- Created by 337547038  -->
<template>
  <div class="components-list">
    <div v-for="(list, index) in controlList" :key="index">
      <div class="title">
        {{ list.title }}
      </div>
      <draggable :itemKey="'yahaha' + index" tag="ul" v-model="list.children"
        :group="{ name: 'form', pull: 'clone', put: false }" ghost-class="ghost" :sort="false" :clone="clone">
        <template #item="{ element }">
          <li :class="[element.type]">
            <i :class="`icon-${element.icon}`"></i>
            <span :title="element.label">{{ element.label }}</span>
          </li>
        </template>
      </draggable>
    </div>
    <use-template ref="useTemplateEl" @click="TemplateSelect" v-if="!isSearch" />
  </div>
</template>
<script lang="ts" setup>
import controlListData from './controlList'
import Draggable from 'vuedraggable-es'
import { computed, ref, watch, inject } from 'vue'
import { FormData, FormList } from '../../types'
import UseTemplate from './template.vue'
import { deepClone, readWidgetOptions, stringToObj } from '../../utils'
import { useSysModel } from '/@/stores/sysModel';
import { useVisualDev } from '/@/stores/visualDev';
const props = defineProps({
  formId: {
    type: Number,
    required: false,
  },
  modelId: {
    type: Number,
    required: false,
  },
});

const emits = defineEmits<{
  (e: 'fieldSel', value: FormList): void
  (e: 'templateSel', value: FormData): void
}>()

const designType = inject('formDesignType') as string
const formDataList = ref<any>([]);

const isSearch = computed(() => {
  return designType === 'search'
})
const LayoutComp = [
  'grid',
  'tabs',
  'card',
  'divider',
  'button',
  'divider',
  'title',
]
const modelFields = ref<any[]>([]);
const controlList = computed(() => {
  // 只返回布局字段
  const temp: any = []
  temp.push({ title: '主表字段', children: modelFields.value })
  controlListData.forEach((item: any) => {
    if (item.children) {
      const filter = item.children.filter((ch: any) => {
        return LayoutComp.includes(ch.type)
      })
      if (filter && filter.length) {
        temp.push({ title: item.title, children: filter })
      }
    }
  })
  return temp
})


const clone = (origin: any) => {
  return deepClone(origin)
}

watch(() => props.formId, (val) => {
  if (val && isSearch.value) {
    getFormField(val)
  }
});

watch(() => props.modelId, (val) => {
  if (val) {
    setModelFields(val)
  }
});


// 加载当前列表所属的表单，从表单中提取可用于搜索的字段
const getFormField = (formId: Number) => {
  var res = useVisualDev().getVisualDev(formId);
  const data = stringToObj(res.FormData);
  if (data && data.list) {
    forEachGetData(data.list)
  }
};

/**
 * 获取当前所选择的模型字段
 * @param model 表
 */
const getModelField = (model: any) => {
  const hideFields = ["", null, undefined] // 需要隐藏的字段名
  let temp: any[] = [];
  let res = useSysModel().getSysFieldsByModelId(model);
  res.forEach((item: any) => {
    if (item.SubFields) {
      item.SubFields = item.SubFields.map((subItem: any) => {
        if (!hideFields.includes(subItem.Name)) { return initField(subItem); }
      });
    }
    if (!hideFields.includes(item.Name)) { temp.push(initField(item)); }
  })
  return temp;
};


const initField = (vals: any) => {
  const widget = readWidgetOptions()
  const filteredItem: any = widget.find(item => {
    if (item.fieldType.includes('*')) {
      return true
    }
    else {
      return item.fieldType.includes(vals.tType)
    }
  });
  vals.type = filteredItem?.name ?? ''
  vals.label = vals.Description
  vals.fieldName = vals.Name
  vals.name = vals.Name
  vals.key = null
  vals.control = { modelValue: '' }
  vals.config = {}
  vals.list = []
  filteredItem?.options.forEach((item: any) => {
    if (!(item.key in vals.config)) {
      // 默认
      if (item.default) {
        item.value = item.default;
        vals.config[item.key] = item.default;
      }
    }
  })
  return vals;
}

const setModelFields = (val: any) => {
  modelFields.value = getModelField(val)
}
setModelFields(props.modelId)

const forEachGetData = (data: FormList[]) => {
  data.forEach((item: any) => {
    if (item.type === 'grid' || item.type === 'tabs') {
      item.columns.forEach((col: any) => {
        forEachGetData(col.list)
      })
    } else if (item.type === 'card') {
      forEachGetData(item.list)
    } else if (item.type !== 'button') {
      formDataList.value.push(item)
    }
  })
}
// 使用模板
const useTemplateEl = ref()
// const useTemplateClick = () => {
//   useTemplateEl.value.open()
// }
const TemplateSelect = (data: FormData) => {
  emits('templateSel', data)
}
</script>
