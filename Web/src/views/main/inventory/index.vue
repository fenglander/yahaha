<template>
  <div  style="width: 100%;height: 100%;">
    <!-- 使用 MyTable 组件 -->
    <yList model="SysModels" :create="true" :edit="true" :del="true" >
    </yList>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, reactive } from 'vue';
  import yList from '/@/components/yahaha/yList.vue';
  const internalValue = ref("");
  onMounted(() => {
    //console.log(123);
  });

  const dialog = reactive({
  visible: false,
  type: 1,
  editId: '',
  formData: {
    list: [
      {
        type: 'input',
        control: {
          modelValue: '',
          placeholder: '请输入字典名称'
        },
        config: {},
        name: 'name',
        item: {
          label: '字典名称'
        },
        customRules: [
          {
            type: 'required',
            message: '请输入字典名称',
            trigger: 'blur'
          }
        ]
      },
      {
        type: 'input',
        control: {
          modelValue: '',
          placeholder: '请输入字典标识'
        },
        config: {
          editDisabled: true
        },
        name: 'type',
        item: {
          label: '字典标识'
        },
        customRules: [
          {
            type: 'required',
            message: '请输入字典标识',
            trigger: 'blur'
          }
        ]
      },
      {
        type: 'radio',
        control: {
          modelValue: 1
        },
        options: [
          {
            label: '正常',
            value: 1
          },
          {
            label: '停用',
            value: 0
          }
        ],
        config: {
          optionsType: 0
        },
        name: 'status',
        item: {
          label: '状态'
        }
      },
      {
        type: 'textarea',
        control: {
          modelValue: ''
        },
        config: {},
        name: 'remark',
        item: {
          label: '说明描述'
        }
      },
      {
        type: 'button',
        control: {
          label: '保存',
          type: 'primary',
          key: 'submit'
        },
        config: {
          span: 0
        }
      },
      {
        type: 'button',
        control: {
          label: '取消',
          key: 'reset'
        },
        config: {
          span: 0
        }
      }
    ],
    form: {
      labelWidth: '',
      size: 'default'
    },
    config: {}
  }
});
</script>