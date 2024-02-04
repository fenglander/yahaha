import { defineStore } from 'pinia';
import * as api from '/@/api/model';
import { toRaw } from 'vue'
/**
 * 缓存系统模型数据
 * @methods SysModel 系统模型数据0
 */
export const useSysModel = defineStore('sysmodel', {
	state: (): sysModelState => ({
		sysModelList: [],
		sysFieldList: [],
		sysActionList: [],
	}),
	getters: {
	},
	actions: {
		async setSysModels(): Promise<any[]> {
			const res = await api.getModelList()
			this.sysModelList = res.data?.result ?? [];
			return this.sysModelList;
		},
		async setSysFiedls(): Promise<any[]> {
			const res = await api.getFieldList()
			this.sysFieldList = res.data?.result ?? [];
			return this.sysFieldList;
		},
		async setSysActions(): Promise<any[]> {
			const res = await api.getActionList()
			this.sysActionList = res.data?.result ?? [];
			return this.sysActionList;
		},
		getSysModels(name: any): any {
			const sysModelList = toRaw(this.sysModelList);
			let res;
			sysModelList.forEach((it: any) => {
				if (it.Name === name) { res = it; return; }
			})
			return res
		},
		getSysActionById(id: any): any {
			const sysActionList = toRaw(this.sysActionList);
			return sysActionList.filter((it: any) => it.BindingModel.Id === id)
		},
		getSysModelsById(Id: any): any {
			Id = Number(Id);
			const sysModelList = toRaw(this.sysModelList);
			let res;
			sysModelList.forEach((it: any) => {
				if (it.Id === Id) { res = it; return; }
			})
			return res
		},

		getSysModelLables(name: any): any {
			const Default = ["Id", "Code", "Name"]
			let DisplayFields: string[] = [];
			const res = this.getSysFields(name);
			res.forEach((it: any) => {
				if (it.Display && !DisplayFields.includes(it.Name)) {
					DisplayFields.push(it.Name);
				}
			});
			if (DisplayFields.length === 0) { DisplayFields = Default; }
			return DisplayFields;
		},
		getTitleByrForm(id: any, data: any[]): any {
			//let lable: string = ""; //默认指端
			const model = this.getSysModelsById(id);
			const lables = this.getSysModelLables(model.Name);
			const excludedValues = [undefined, null, "", 0];
			for (let i = lables.length - 1; i >= 0; i--) {
				const lable = lables[i];
				const matchingElement = !excludedValues.includes(data[lable]);
				if (matchingElement) {
					return data[lable];
				}
			}
			return "";
		},
		getSysFieldsByModelId(Id: any): any {
			const sysFieldList = toRaw(this.sysFieldList);
			const res = sysFieldList.filter((it: any) => it.ModelId == Id)
			return res
		},
		getSysFields(name: any): any {
			const model = this.getSysModels(name)
			const sysFieldList = toRaw(this.sysFieldList);
			return sysFieldList.filter((it: any) => it.ModelId == model.Id)
		},
		getFieldInfo(Id: any): any {
			const sysFieldList = toRaw(this.sysFieldList);
			let res;
			sysFieldList.forEach((it: any) => {
				if (it.Id === Id) { res = it; return; }
			})
			return res
		},

		getRelateFieldList(Id: any): any {
			const row = this.getSysFieldsByModelId(Id).filter((item: any) => item.Relate);
			const res = row.map((item: any) => {
				return {
					...item, // 保留原始属性
					relatedKey: item.Related.split('.')[0], // 增加新属性
				};
			});
			return res;
		}
	}
});
