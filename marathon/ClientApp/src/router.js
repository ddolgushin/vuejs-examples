// Модуль маршрутизации. Описывает маршруты (подстроки адреса URL приложения),
// поддерживаемые приложением. При наборе адреса в строке адреса браузера
// приложение включит соответствующий компонент в месте расположения тэга
// <router-view></router-view> (компонент "app.vue").

import Vue from 'vue';
import VueRouter from 'vue-router';
import HomeComponent from './components/home-component.vue';
import MarathonListComponent from './components/marathon-list-component.vue';
import MarathonComponent from './components/marathon-component.vue';

Vue.use(VueRouter);

export default new VueRouter({
	routes: [
		{ path: '/', name: 'home', component: HomeComponent },
		{ path: '/marathon-list', name: 'marathon-list', component: MarathonListComponent },
		// :id -- параметр, извлекаемый из URL и передаваемый одноимённому свойству (элементу "props") компонента.
		{ path: '/marathon/:id', name: 'marathon', component: MarathonComponent, props: true }
	]
});
