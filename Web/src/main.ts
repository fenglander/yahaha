import { createApp } from 'vue';
import pinia from '/@/stores/index';
import App from '/@/App.vue';
import router from '/@/router';
import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';

import ElementPlus from 'element-plus';
import '/@/theme/index.scss';
import VueGridLayout from 'vue-grid-layout';

import VueSignaturePad from 'vue-signature-pad'; // 电子签名
import vue3TreeOrg from 'vue3-tree-org'; // 组织架构图
import 'vue3-tree-org/lib/vue3-tree-org.css'; // 组织架构图样式
import 'animate.css'; // 动画库
import '@icon-park/vue-next/styles/index.css';
import {IconPark} from '@icon-park/vue-next/es/all';

const app = createApp(App);

directive(app);
other.elSvg(app);

app.use(pinia).use(router).use(ElementPlus).component('IconPark', IconPark).use(i18n).use(VueGridLayout).use(VueSignaturePad).use(vue3TreeOrg).mount('#app');
