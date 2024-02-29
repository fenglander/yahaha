<template>
    <div style="width: 100%;height: 100%;">
        <listRenderer v-if="listconfig" :model-id="modelId" :listConfig="listconfig" :user-des-id="userDesId" />
    </div>
</template>
    
<script setup lang="ts" name="generalListView">
import { ref, reactive, onMounted, computed } from 'vue';
import listRenderer from '/@/components/yahaha/design/list/listRenderer.vue'
import { formatNumber, mergeObjects, stringToObj } from '/@/components/yahaha/design/utils/'
import { userListDesignScheme } from '/@/api/visualDev';
import { useVisualDev } from '/@/stores/visualDev';
import { useUserInfo } from '/@/stores/userInfo';
import { useSysModel } from '/@/stores/sysModel';
import { useRouter } from 'vue-router';
import { SysField } from '/@/api-services';
const router = useRouter();
const formDesignId = ref<number>();
const modelId = ref<number>();
const listDesignId = ref<number>();
const isKeepAlive = ref<any>();
const listconfig = ref<any>();
const userDesId = ref<number>();

const query = reactive(router.currentRoute.value.query);

const init = () => {
    console.log('页面参数：', router.currentRoute.value)
    isKeepAlive.value = router.currentRoute.value.meta.isKeepAlive;
    formDesignId.value = formatNumber(query.formDesignId);
    listDesignId.value = formatNumber(query.listDesignId);
    modelId.value = formatNumber(query.modelId);
}
init();

const fields = computed<SysField[]>(() => {
    if (modelId.value) {
        return useSysModel().getSysFieldsByModelId(modelId.value).filter((item: any) => item.Description !== null && item.Description.trim() !== "");
    } else {
        return []
    }
})

const getListConfig = async () => {
    const params = {
        sysModel: modelId.value,
        listDesign: listDesignId.value ?? 0,
        user: useUserInfo().userInfos.id,
        id: 0
    }
    const res = await userListDesignScheme(params);
    const userDesignData = stringToObj(res.data.result?.DesignData ?? "");
    const DesignData = useVisualDev().getlistDesgin(listDesignId.value) ?? {};
    listconfig.value = mergeObjects(DesignData, userDesignData);
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
        // 如何得知是否有新增字段？ 提高友好度
    } 
}

onMounted(async () => {
    await getListConfig()
});

</script>