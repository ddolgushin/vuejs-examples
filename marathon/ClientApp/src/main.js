// Программная точка входа в Веб-приложение. Здесь инициализируется объект
// Vue, в котором указано, какой тэг разметки нужно использовать для встраивания
// основного компонента приложения "App".

// Кроме того, здесь импортируются необходимые для приложения модули.

import Vue from 'vue';
import BootstrapVue from 'bootstrap-vue';
import Router from './router.js';
import App from './app.vue';

// Подключаем стили Bootstrap.
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';

// Подключаем плагин для работы с Bootstrap (программная часть для использования
// тэгов соответствующих компонентов Bootstrap).
Vue.use(BootstrapVue);

new Vue({
	el: '#app',
	template: '<app/>',
	components: { App },
	// Подключаем модуль маршрутизации.
	router: Router
});
