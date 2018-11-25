<template>
	<b-navbar-nav class="ml-auto">
		<b-modal id="authModal"
						 title="Авторизация"
						 cancel-title="Отмена"
						 :ok-disabled="!authDataValid"
						 no-close-on-backdrop
						 @shown="clearAuthData"
						 @ok="onLogin">
			<b-input-group>
				<b-form-input ref="authDataName"
											v-model="authData.name"
											placeholder="Имя"
											@keydown.native="onAuthKeyDown"
											required />
				</b-input-group>
			<b-input-group>
				<b-form-input type="password"
											v-model="authData.password"
											@keydown.native="onAuthKeyDown"
											placeholder="Пароль"
											required />
			</b-input-group>
		</b-modal>

		<b-modal id="regModal"
						 ref="regModal"
						 title="Регистрация"
						 cancel-title="Отмена"
						 hide-footer
						 no-close-on-backdrop
						 @shown="clearRegData">
			<b-form @submit.stop.prevent="onRegister" @reset="onRegisterReset">
				<b-form-group label="Имя:" label-for="regDataName">
					<b-form-input id="regDataName"
												ref="regDataName"
												type="text"
												placeholder="Имя"
												v-model="regData.name"
												required />
				</b-form-group>
				<b-form-group label="Email:" label-for="regDataEmail">
					<b-form-input id="regDataEmail"
												type="email"
												placeholder="Email"
												v-model="regData.email"
												required />
				</b-form-group>
				<b-form-group label="Пароль:" label-for="regDataPassword">
					<b-form-input id="regDataPassword"
												type="password"
												placeholder="Пароль"
												v-model="regData.password"
												required />
				</b-form-group>
				<b-form-group label="Подтверждение пароля:" label-for="regDataPasswordConfirm">
					<b-form-input id="regDataPasswordConfirm"
												type="password"
												placeholder="Подтверждение пароля"
												v-model="regData.passwordConfirm"
												required />
				</b-form-group>
				<b-form-group label="Фото:" label-for="regDataPicture">
					<b-form-file id="regDataPicture"
											 v-model="regData.picture"
											 accept="image/*"
											 placeholder="Фото для профиля" />
				</b-form-group>
				<b-button type="submit" variant="primary">Зарегистрироваться</b-button>
      	<b-button type="reset" variant="danger">Сбросить</b-button>
			</b-form>
		</b-modal>
		
		<b-nav-item-dropdown right>
			<template slot="button-content">
				<em>{{menuLabel}}</em>
			</template>
			<!--
				-- В тэгах ниже используется директива "v-if", обеспечивающая условный рендеринг.
				-- Например, если значение вычисляемого свойства "user" будет определено (не null и не undefined),
				-- то в разметке появится элемент выпадающего списка "Выйти".
				-->
			<b-dropdown-item v-if="!user" href="#" v-b-modal.authModal>Войти</b-dropdown-item>
			<b-dropdown-item v-if="!user" href="#" v-b-modal.regModal>Зарегистрироваться</b-dropdown-item>
			<b-dropdown-item v-if="user" href="#" @click="onLogout">Выйти</b-dropdown-item>
		</b-nav-item-dropdown>
	</b-navbar-nav>
</template>

<script>
import SessionAccessMixin from '../mixins/session-access-mixin.vue';
import AuthProxy from '../proxy/auth-proxy.js';
import UserProxy from '../proxy/user-proxy.js';

export default {
	name: 'SessionComponent',
	mixins: [ SessionAccessMixin ],
  data() {
    return {
      user: null,
      authData: {
        name: null,
        password: null
      },
      regData: {
        name: null,
        email: null,
        password: null,
				passwordConfirm: null,
				picture: null
      }
    };
	},
  // Вычисляемые свойства. Могут быть использованы в секции описания шаблона
  // для отображения нужных значений в разметке или определения доступности частей
  // разметки в зависимости от состояния модели.
  // В отличие от методов, результаты вычисляемых свойств кэшируются и обновляются
  // только при изменении свойств модели, от которых они зависят.
  computed: {
    menuLabel() {
      return this.user ? this.user.name : "Кабинет";
		},
		
		authDataValid() {
			return this.authData.name
				&& this.authData.password;
		}
	},
	created() {
		this.tryRestoreUser();
	},
  methods: {
		tryRestoreUser() {
			if (this.user)
				return;
			
			this.user = this.sessionGetUser();
		},

    clearAuthData() {
      this.authData.name = null;
			this.authData.password = null;
			
			this.$refs.authDataName.focus();
    },

    clearRegData() {
      this.regData.name = null;
      this.regData.email = null;
      this.regData.password = null;
			this.regData.passwordConfirm = null;
			this.regData.picture = null;

			this.$refs.regDataName.focus();
    },

    onLogin(e) {
      let vm = this;

      AuthProxy.login(
        vm.authData,
        data => {
          vm.user = data;
					localStorage.setItem('user', JSON.stringify(vm.user));
					window.location.reload();
        },
        err => {
          if (err.response.status === 401 || err.response.status == 404)
            alert('Неверное имя пользователя или пароль');
        }
      );
    },

		onAuthKeyDown(e) {
      if (e.which === 13)
        this.onLogin(e);
		},

    onRegister(e) {
			e.preventDefault();
			
			let vm = this;
			// Функция, вызываемая для отправки данных на сервер.
			let doRegistration = () =>
				UserProxy.create(
					vm.regData,
					data => {
          	vm.user = data;

						localStorage.setItem('user', JSON.stringify(vm.user));
						vm.$refs.regModal.hide();
					},
					err => {
						console.log(err);
						alert('Ошибка регистрации');
					});

			if (vm.regData.picture
					&& vm.regData.picture.toString() === '[object File]') {
				let reader = new FileReader();

				// С помощью объекта FileReader читаем данные файла в формате Base64.
				reader.readAsDataURL(vm.regData.picture);

				reader.onload = () => {
					// Если считывание завершилось успешно, дополняем объект и отсылаем его на сервер.
					vm.regData.pictureType = vm.regData.picture.type;
					vm.regData.picture = reader.result;

					doRegistration();
				};
				reader.onerror = error => {
					console.log(error);
					alert('Ошибка чтения файла');
				};
			}
			else
				doRegistration();
		},

		onRegisterReset(e) {
			e.preventDefault();
			this.clearRegData();
		},

    onLogout(e) {
			this.user = null;
			
			this.sessionRemoveUser();
			window.location.reload();
    }
  }
};
</script>
