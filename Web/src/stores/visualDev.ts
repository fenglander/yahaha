import { defineStore } from 'pinia';
import * as api from '/@/api/visualDev';
import { toRaw } from 'vue'
/**
 * 表单设计列表
 * @methods formDesginList 表单设计
 */
export const useVisualDev = defineStore('visualDev', {
	state: (): visualDevState => ({
		formDesginList: [],
		listDesginList: [],
	}),
	getters: {


	},
	actions: {
		async setFormDesginList() {
			const res = await api.formDesginList()
			this.formDesginList = res.data?.result ?? [];
			return this.formDesginList;
		},
		async setListDesginList() {
			const res = await api.listDesginList()
			this.listDesginList = res.data?.result ?? [];
			return this.listDesginList;
		},
		getFormDesgin(id: any): any {
			const list = toRaw(this.formDesginList);
			let res;
			list.forEach((it: any) => {
				if (parseFloat(it.Id) === parseFloat(id)) { res = it; return; }
			})
			return res
		},
		getlistDesgin(id: any): any {
			const list = toRaw(this.listDesginList);
			let res;
			list.forEach((it: any) => {
				if (parseFloat(it.Id) === parseFloat(id)) { res = it; return; }
			})
			return res
		},

	},
});
