<template>
  <div v-if='marathon'>
		<b-modal ref='waypointEditorModal'
						 title='Точка маршрута'
						 cancel-title='Отмена'
						 no-close-on-backdrop
						 no-close-on-esc
						 @ok='onWaypointEdited'
						 @cancel='onWaypointEdited'>
			<b-input-group>
				<b-form-input v-model='currentWaypoint.name'
											placeholder='Название'
											required />
			</b-input-group>
			<b-input-group>
				<b-form-textarea v-model='currentWaypoint.description'
												 placeholder='Описание'
												 rows='3'
												 required />
			</b-input-group>
			<b-input-group>
				<b-form-file v-model='currentWaypoint.fileInput'
										 accept='image/*'
										 placeholder='Фото места' />
			</b-input-group>
		</b-modal>

		<b-modal ref='waypointInfoModal'
						 :title='currentWaypoint.name'>
			<div v-if="currentWaypoint.description">
				<p>{{currentWaypoint.description}}</p>
			</div>
			<div v-if="currentWaypoint.picture">
				<b-img :src="currentWaypoint.picture" fluid />
			</div>
			<div slot="modal-footer" class="w-100">
				<p class="float-left"></p>
				<b-btn size="sm" class="float-right" variant="primary" @click="onWaypointInfoModalClose">
					Закрыть
				</b-btn>
			</div>
		</b-modal>

		<h1>{{marathon.title}}</h1>
		<h2>{{marathonDateLabel}}, {{marathonDistanceLabel}}</h2>
		<br/>
		<div v-if="marathon.description">
			<p>{{marathon.description}}</p>
			<br/>
		</div>

		<div role="tablist">
			<b-card no-body class="mb-1">
				<b-card-header header-tag="header" class="p-1" role="tab">
					<b-btn block href="#" v-b-toggle.accordionMap variant="info">Карта</b-btn>
				</b-card-header>
				<b-collapse id="accordionMap" visible accordion="marathonAccordion" role="tabpanel">
					<b-card-body>
						<!-- Начало содержимого секции -->

						<b-row>
							<b-col>
								<yandex-map
									:coords='[57.146689, 65.531581]'
									zoom='10'
									style='width: 640px; height: 480px'
									map-type='map'
									@map-was-initialized='onMapInitialized'
								/>
							</b-col>
							<b-col v-if='editable'>
								<b-btn v-b-toggle.collapseEditor class="p-1">Редактор</b-btn>
								<b-collapse id="collapseEditor">
									<b-card>
										<b-row>
											<div>
												<b-form-group>
													<b-form-input v-model="marathon.title" placeholder="Название" />
													<b-form-textarea v-model="marathon.description" placeholder="Описание" />
													<b-form-input v-model="dateInput"
														placeholder="Дата проведения"
														type="date"
														lazy-formatter />
												</b-form-group>
												<b-form-group>
													<b-form-radio-group buttons
														button-variant='secondary'
														size='sm'
														v-model='editMode'
														:options='editOptions' />
												</b-form-group>
												<b-form-group>
													<b-button variant='primary' size='sm' :disabled='!dirtyFlag' @click='onSave'>Сохранить</b-button>
													<b-button variant='primary' size='sm' :disabled='!dirtyFlag' @click='onCancel'>Отмена</b-button>
												</b-form-group>
											</div>
										</b-row>
									</b-card>
								</b-collapse>
							</b-col>
						</b-row>

						<!-- Конец содержимого секции -->
					</b-card-body>
				</b-collapse>
    	</b-card>
			<b-card v-if="assignmentAllowed" no-body class="mb-1">
				<b-card-header header-tag="header" class="p-1" role="tab">
					<b-btn block href="#" v-b-toggle.accordionParticipants variant="info">Участники</b-btn>
				</b-card-header>
				<b-collapse id="accordionParticipants" accordion="marathonAccordion" role="tabpanel">
					<b-card-body>
						<!-- Начало содержимого секции -->

						<b-row>
							<ul>
								<li v-for='item in participants'>{{item.name}}</li>
							</ul>
						</b-row>
						<b-row>
							<div v-if="sessionExists()">
								<b-button variant="success" size="sm" :disabled="currentUserParticipates" @click="onAssignClick">Участовать</b-button>
								<b-button variant="primary" size="sm" :disabled="!currentUserParticipates" @click="onDeclineClick">Отказаться</b-button>
							</div>
						</b-row>

						<!-- Конец содержимого секции -->
					</b-card-body>
				</b-collapse>
    	</b-card>
		</div>
  </div>
</template>

<script>
import Vue from 'vue';
import YmapPlugin from 'vue-yandex-maps';
import SessionAccessMixin from '../mixins/session-access-mixin.vue';
import MapMixin from '../mixins/map-mixin.vue';
import UtilMixin from '../mixins/util-mixin.vue';
import Proxy from '../proxy/marathon-proxy.js';

Vue.use(YmapPlugin);

export default {
	name: 'MarathonComponent',
	mixins: [ SessionAccessMixin, MapMixin, UtilMixin ],
	components: { YmapPlugin },
	props: [ 'id' ],
	data() {
		return {
			marathon: null,
			editOptions: [
				{ text: 'Нарисовать маршрут', value: 'createRoute', disabled: false },
				{ text: 'Описать точку', value: 'createWaypoint', disabled: false },
				{ text: 'Редактировать объекты', value: 'edit', disabled: false }
			],
			dirtyFlag: false,
			currentWaypoint: this.createWaypointInfo(),
			participants: [],
			dateInput: null
		};
	},
	watch: {
		'marathon.title': function (newValue) {
			this.dirtyFlag = true;
		},
		'marathon.description': function (newValue) {
			this.dirtyFlag = true;
		},
		dateInput: function (newValue) {
			this.marathon.date = new Date(newValue);
			this.dirtyFlag = true;
		}
	},
	computed: {
		currentUserParticipates() {
			let id = this.sessionGetUserId();

			if (!id)
				return false;

			return this.participants.find(o => o.id === id) !== undefined;
		},
		marathonDateLabel() {
			return this.formatDate(new Date(this.marathon.date));
		},
		marathonDistanceLabel() {
			return this.marathon.distance ? parseFloat(this.marathon.distance).toFixed(2) + ' км.' : '--';
		},
		assignmentAllowed() {
			return this.marathon.route !== undefined && this.marathon.route !== null;
		}
	},
	created() {
		let vm = this;

		// В контексте анонимной функции this будет указывать на другой объект, поэтому
		// передаём в него текущий this в виде переменной vm (т.н. 'замыкание')
		// и через неё обращаемся к модели компонента.
		Proxy.get(vm.id,
			data => {
				vm.marathon = data;
				vm.dateInput = this.formatDate(vm.marathon.date, '-', true);

				let userId = vm.sessionGetUserId();

				if (userId) {
					Proxy.ownedBy(vm.marathon.id, userId,
						data => {
							vm.editable = data === true;

							if (vm.marathon.route !== null && vm.marathon.route !== undefined)
								vm.disableRouteCreation();
						},
						err => vm.handleError(err));
				}

				vm.loadParticipants();
			},
			err => vm.handleError(err));
	},
	beforeDestroy() {
		this.assignMapEvents(false);
	},
	methods: {
		showWaypointEditor(waypoint) {
			this.clearCurrentWaypoint();
			this.fillWaypoint(waypoint, this.currentWaypoint);

			this.currentWaypoint.innerId = waypoint.mark.properties.get('innerId');

			this.$refs.waypointEditorModal.show();
		},

		showWaypointInfo(waypoint) {
			this.fillWaypoint(waypoint, this.currentWaypoint);
			this.$refs.waypointInfoModal.show();
		},

		disableRouteCreation(flag = true) {
			this.editOptions[0].disabled = flag;
		},

		createWaypointInfo() {
			return {
				id: null,
				innerId: null,
				name: null,
				description: null,
				picture: null,
				pictureType: null
			};
		},

		copyCurrentWaypoint() {
			return {
				id: this.currentWaypoint.id,
				innerId: this.currentWaypoint.innerId,
				name: this.currentWaypoint.name,
				description: this.currentWaypoint.description,
				picture: this.currentWaypoint.picture,
				pictureType: this.currentWaypoint.pictureType,
				location: this.currentWaypoint.location,
				mark: this.currentWaypoint.mark
			};
		},

		clearCurrentWaypoint() {
			this.currentWaypoint.id = 0;
			this.currentWaypoint.innerId = 0;
			this.currentWaypoint.name = null;
			this.currentWaypoint.description = null;
			this.currentWaypoint.picture = null;
			this.currentWaypoint.pictureType = null;
			this.currentWaypoint.location = {
				type: 'Feature',
				geometry: {
					type: 'Point',
					coordinates: []
				}
			};
			this.currentWaypoint.mark = null;
			this.currentWaypoint.fileInput = null;
		},

		fillWaypoint(source, target) {
			target.id = source.id;
			target.innerId = source.innerId;
			target.name = source.name;
			target.description = source.description;
			target.picture = source.picture;
			target.pictureType = source.pictureType;
			target.mark = source.mark;
		},

		loadParticipants() {
			let vm = this;

			Proxy.getParticipants(vm.marathon.id,
				data => {
					vm.participants.length = 0;

					data.forEach(item => vm.participants.push(item));
				},
				err => vm.handleError(err));
		},

		onWaypointEdited(e) {
			let vm = this;
			let addOrUpdateWaypointModel = function () {
				if (vm.currentWaypoint.innerId > 0)
					vm.fillWaypoint(
						vm.currentWaypoint,
						vm.getWaypointModel(vm.currentWaypoint.innerId)
					);
				else
					vm.marathon.waypointInfos.push(vm.copyCurrentWaypoint());
				
				vm.clearCurrentWaypoint();
			};

			if (vm.currentWaypoint.fileInput
					&& vm.currentWaypoint.fileInput.toString() === '[object File]') {
				let reader = new FileReader();

				reader.readAsDataURL(vm.currentWaypoint.fileInput);

				reader.onload = () => {
					vm.currentWaypoint.pictureType = vm.currentWaypoint.fileInput.type;
					vm.currentWaypoint.picture = reader.result;

					addOrUpdateWaypointModel();
				};
				reader.onerror = error => {
					console.log(error);
					alert('Ошибка чтения файла');
				};
			} else
				addOrUpdateWaypointModel();
		},

		onSave(e) {
			let vm = this;

			vm.disableFeatureEditors();
			vm.updateMarathonFromMap();
			vm.serializeMarathonGeometries();
			vm.assignMarksToWaypointInfos(false);

			let successCallback = data => {
				vm.deserializeMarathonGeometries();
				vm.assignMarksToWaypointInfos();

				vm.dirtyFlag = false;
				vm.editMode = null;
			};
			let failureCallback = err => {
				console.log(err);
				vm.deserializeMarathonGeometries();
				vm.assignMarksToWaypointInfos();
				vm.handleError(err);
			};

			if (vm.marathon.id)
				Proxy.update(
					vm.marathon,
					successCallback,
					failureCallback,
					vm.sessionGetAuthHeader()
				);
			else
				Proxy.create(
					vm.marathon,
					successCallback,
					failureCallback,
					vm.sessionGetAuthHeader()
				);
		},

		onCancel(e) {
			this.cancelEdit();
		},

		onWaypointInfoModalClose(e) {
			this.$refs.waypointInfoModal.hide();
			this.clearCurrentWaypoint();
		},

		onAssignClick() {
			let vm = this;

			Proxy.assignParticipant(
				vm.marathon.id,
				() => vm.loadParticipants(),
				err => vm.handleError(err),
				vm.sessionGetAuthHeader());
		},

		onDeclineClick() {
			let vm = this;

			Proxy.declineParticipant(
				vm.marathon.id,
				() => vm.loadParticipants(),
				err => vm.handleError(err),
				vm.sessionGetAuthHeader());
		}
	}
};
</script>
