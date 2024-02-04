import { defineStore } from 'pinia';
import * as api from '/@/api/visualDev';
import { toRaw } from 'vue'
/**
 * 表单设计列表
 * @methods visualDevList 表单设计
 */
export const useVisualDev = defineStore('visualDev', {
	state: (): visualDevState => ({
		visualDevList: [],
	}),
	getters: {


	},
	actions: {
		async setVisualDevList() {
			const res = await api.getVisualDevList()
			this.visualDevList = res.data?.result ?? [];
			return this.visualDevList;
		},
		getVisualDev(id: any): any {
			const visualDevList = toRaw(this.visualDevList);
			let res;
			visualDevList.forEach((it: any) => {
				if (parseFloat(it.Id) === parseFloat(id)) { res = it; return; }
			})
			return res
		},
		getVisualDevList() {
			return this.visualDevList;
		},

	},
});
