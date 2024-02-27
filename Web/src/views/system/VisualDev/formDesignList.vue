<template>
  <div style="width: 100%;height: 100%;">
    <!-- 使用 MyTable 组件 -->
    
    <listRenderer v-if="listconfig" :model-id="modelId" :listConfig="listconfig" :user-des-id="userDesId"
      :form-comp="formDes" />
  </div>
</template>
  
<script setup lang="ts" name="visualDev">
import { onMounted, ref, computed } from 'vue';
import listRenderer from '/@/components/yahaha/design/list/listRenderer.vue'
import { stringToObj } from '/@/components/yahaha/design/utils/'
import formDes from './component/formDes.vue';
import { useSysModel } from '/@/stores/sysModel';
import { userListDesignScheme } from '/@/api/visualDev';

const listconfig = ref();
const userDesId = ref();
const modelId = ref(useSysModel().getSysModels('FormDesign').Id);

const fields = computed(() => {
  if (modelId.value) {
    return useSysModel().getSysFieldsByModelId(modelId.value).filter((item: any) => item.Description !== null && item.Description.trim() !== "");
  } else {
    return []
  }
})

const getListConfig = async () => {
  const params = {
    sysModel: modelId.value,
    listDesign: 0,
    id: 0
  }
  const res = await userListDesignScheme(params);
  const userDesignData = stringToObj(res.data.result?.DesignData ?? "{}");
  listconfig.value = userDesignData;
  if (Object.keys(listconfig.value).length > 0 && listconfig.value.columns.length > 0) {
    userDesId.value = res.data.result.Id;
    let temp: any[] = [];
    listconfig.value.columns.forEach((it: any) => {
      const field = fields.value.find((item: any) => item.Name === it.Name);
      // 只处理能匹配上的
      if (field) {
        temp.push(Object.assign(it, { ...field, ...it }));
      }
    })
    listconfig.value.columns = temp;
  }
}


onMounted(async () => {
  await getListConfig();
});

</script>