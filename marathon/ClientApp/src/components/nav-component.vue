<template>
  <div>
		<!-- Один из тэгов bootstrap-vue для отображения панели навигации. -->
		<b-navbar toggleable="md" type="light" variant="light">
			<b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
			<b-navbar-brand href="#/">
				<img src="dist/logo.png" alt="Главная" width="30">
			</b-navbar-brand>
			<b-collapse is-nav id="nav_collapse">
				<b-navbar-nav>
					<b-nav-item href="#/">Главная</b-nav-item>
					<b-nav-item href="#/marathon-list">Список марафонов</b-nav-item>
				</b-navbar-nav>
				<session-component />
			</b-collapse>
		</b-navbar>
  </div>
</template>

<script>
import SessionComponent from './session-component';

export default {
	name: 'NavComponent',
	components: { SessionComponent }
};
</script>
