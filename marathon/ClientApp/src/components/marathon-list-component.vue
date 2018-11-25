<template>
  <div>
		<h1>Список марафонов</h1>
		<ul>
			<!--
				-- Пример другой директивы ("v-for"): цикл, в котором элементы массива "marathons" перебираются
				-- и для каждого из них в разметку добавляется ссылка на соответствующий марафон.
				--
				-- В фигурных скобках ("{{item.title}}") подставляются значения соответствующих объектов
				-- или результаты вычисления выражений.
				--
				-- Директива :key нужна для привязки элементов li к конкретным объектам массива. Это позволяет
				-- осуществлять адресное обновление представления по модели (если в модели обновился один объект
				-- из десяти, то в представлении обновится только соответствующий ему элемент).
				-->
			<li v-for="item in marathons" :key="item.id">
				<b-link :href="'#/marathon/' + item.id">{{item.title}}</b-link> (состоится {{formatDate(item.date)}})
			</li>
		</ul>
		<b-button v-if="sessionExists()" variant="success" size="sm" @click="onCreateMarathon">Создать свой</b-button>
  </div>
</template>

<script>
import SessionAccessMixin from '../mixins/session-access-mixin.vue';
import UtilMixin from '../mixins/util-mixin.vue';
import Proxy from '../proxy/marathon-proxy.js';

export default {
	name: 'MarathonListComponent',
	mixins: [ SessionAccessMixin, UtilMixin ],
	// Свойство с данными, которые доступны всем методам данного компонента.
	data() {
		return {
			marathons: []
		}
	},
	// Стандартное для компонента Vue.js событие, срабатывающее при создании компонента.
	created() {
		let vm = this;

		// Вызываем функцию из модуля "Proxy" для получения списка марафонов.
		// Результат обрабатывается анонимной функцией, переданной в качестве параметра
		// функции "getMarathons" (можно заметить очевидное сходство с лямбда-выражениями).
		Proxy.getAll(
			data => data.forEach(o => vm.marathons.push(o)),
			err => vm.handleError(err)
		);
	},
	// Свойство, описывающее набор методов данного компонента.
	// Любой метод -- функция с параметрами или без, которая может быть вызвана
	// в других методах или в теле шаблона.
	methods: {
		onCreateMarathon(e) {
			let vm = this;

			Proxy.create(
				data => window.location = '#/marathon/' + data.id,
				err => vm.handleError(err),
				vm.sessionGetAuthHeader()
			);
		}
	}
}
</script>
