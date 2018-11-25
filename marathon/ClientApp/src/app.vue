<!--
  -- Секция описания шаблона компонента с помощью HTML, ссылок на стили CSS и элементов разметки Vue.js.
  -->

<template>
  <div>
		<div class="header">
			<nav-component></nav-component>
		</div>
		<div class="page">
			<router-view></router-view>
		</div>
  </div>
</template>

<!--
  -- Секция описания тела компонента в формате, определяемом Vue.js.
	-- Помимо компонента в этой секции осуществляется импорт нужных модулей
	-- и других компонентов, а также настройка плагинов для Vue.js.
  -->

<script>
// Импорт модуля (компонента Vue.js). Можно указать полное имя с расширением *.vue.
import NavComponent from './components/nav-component';

export default {
	// Название компонента. В разметке оно будет иметь тэг "<app/>".
	// Если название составное, например, "MarathonComponent", то его тэг будет иметь
	// вид "<marathon-component/>".
	name: 'App',
	// Используемые в данном компоненте другие компоненты.
	components: {
		NavComponent
	}
}
</script>

<!--
  -- Секция описания стилей для компонента и приложения в целом.
  -->

<style>

h1 {
	font-size: 1.5rem;
}

h2 {
	font-size: 1.3rem;
}

h3 {
	font-size: 1.1rem;
}

.header {
  width: 100%;
  padding: 20px 20px;
}

.page {
  width: 100%;
  padding: 20px 50px;
}

</style>
