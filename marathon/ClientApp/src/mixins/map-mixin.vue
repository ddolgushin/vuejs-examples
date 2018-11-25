<script>
export default {
	name: 'MapMixin',
	data() {
		return {
			map: null,
			routeOptions: {
				strokeColor: '#0000ff88',
				strokeWidth: 5
			},
			editable: false,
			editMode: null,
			newMarkersIdSequence: -1,
			routeFirstPoint: null
		};
	},
	methods: {
		drawRoute() {
			let vm = this;

			vm.dirtyFlag = false;

			if (this.marathon && this.marathon.route)
				ymaps
					.geoQuery(vm.marathon.route)
					.addToMap(vm.map)
					.applyBoundsToMap(vm.map, {
						checkZoomRange: true
					});

			if (
				this.marathon &&
				this.marathon.waypointInfos &&
				this.marathon.waypointInfos.length
			) {
				vm.marathon.waypointInfos.forEach(item =>
					ymaps.geoQuery(item.location).addToMap(vm.map)
				);
				vm.assignMarksToWaypointInfos();
			}

			vm.assignMapEvents(false);
			vm.assignMapEvents();
		},

		assignMarksToWaypointInfos(on = true) {
			let vm = this;
			let waypointMapObjects = vm.getWaypointsMapObjects();

			if (on) {
				if (waypointMapObjects && waypointMapObjects.length)
					waypointMapObjects.forEach(o => {
						let modelWaypoint = vm.marathon.waypointInfos.find(
							p => p.innerId === o.properties.get('innerId')
						);

						if (modelWaypoint) modelWaypoint.mark = o;
					});
			} else vm.marathon.waypointInfos.forEach(o => (o.mark = null));
		},

		// Привязывает (или отменяет) обработчики событий карты
		// и объектов на ней.
		assignMapEvents(on = true) {
			let vm = this;

			if (on) vm.map.events.add('click', vm.onMapClick, vm);
			else vm.map.events.remove('click', vm.onMapClick, vm);

			vm.map.geoObjects.each(item => {
				if (on) item.events.add('click', vm.onFeatureClick, vm);
				else item.events.remove('click', vm.onFeatureClick, vm);

				if (on) item.events.add('dblclick', vm.onFeatureDoubleClick, vm);
				else item.events.remove('dblclick', vm.onFeatureDoubleClick, vm);

				if (item.geometry.getType() === 'LineString') {
					if (on) item.events.add('geometrychange', vm.onRouteGeometryChanged, vm);
					else item.events.remove('geometrychange', vm.onRouteGeometryChanged, vm);
				}
			});
		},

		// Готовит GeoJSON-объект к отображению на карте. В частности,
		// осуществляет разбор строковых представлений геометрии.
		// Также при необходимости метод меняет координаты местами (invertCoords).
		prepareGeometry(geoJsonStr, invertCoords = false) {
			if (!geoJsonStr || !geoJsonStr.length) return;

			let geoJson =
				typeof geoJsonStr === 'string' ? JSON.parse(geoJsonStr) : geoJson;

			if (invertCoords) {
				let invert = coord => {
					let temp = coord[0];

					coord[0] = coord[1];
					coord[1] = temp;
				};

				if (geoJson.hasOwnProperty('features'))
					geoJson.features.forEach(feature => {
						feature.geometry.coordinates.forEach(coord => invert(coord));
					});
				else if (geoJson.hasOwnProperty('geometry'))
					invert(geoJson.geometry.coordinates);
			}

			return geoJson;
		},

		// Формирует дополнительные поля для объекта, описывающего геометрию,
		// с целью дальнейшего отображения на карте.
		prepareWaypoints(waypointInfos) {
			if (!waypointInfos || !waypointInfos.length) return [];

			let vm = this;
			let res = [];

			waypointInfos.forEach(item => {
				let geoJson = vm.prepareGeometry(item.location);

				if (!geoJson) return;

				item.innerId = item.id;
				geoJson.properties = {
					hintContent: item.name,
					innerId: item.id
				};
				geoJson.options = {
					preset: 'islands#greenDotIcon'
				};

				item.location = geoJson;
			});

			return waypointInfos;
		},

		// Отключает все активные редакторы геометрии на карте.
		disableFeatureEditors() {
			this.map.geoObjects.each(item => item.editor.stopEditing());
		},

		// Обновление хранимого в модели компонента маршрута марафона по данным
		// карты.
		updateRouteFromMap(routeMapObject) {
			let vm = this;

			if (!routeMapObject)
				return;
			if (!vm.marathon.route)
				vm.marathon.route = {
					type: 'FeatureCollection',
					features: [{
						type: 'Feature',
						properties: { },
						geometry: {
							type: 'LineString',
							coordinates: []
						}
					}]
				};

			let routeGeometry = vm.marathon.route.features[0].geometry;

			routeGeometry.coordinates = [];

			let routeMapCoords = routeMapObject.geometry.getCoordinates();

			routeMapCoords.forEach(item => routeGeometry.coordinates.push([item[0], item[1]]));
		},

		// Обновление всего марафона по данным карты.
		updateMarathonFromMap() {
			let vm = this;
			let routeMapObject = vm.getRouteMapObject();

			vm.updateRouteFromMap(routeMapObject);

			// Обновление координат точек маршрута по данным карты.
			if (vm.marathon.waypointInfos && vm.marathon.waypointInfos.length)
				vm.marathon.waypointInfos.forEach(o => {
					if (o.mark) {
						let coords = o.mark.geometry.getCoordinates();

						o.location.geometry.coordinates = [coords[0], coords[1]];
					}
				});
		},

		getRouteMapObject() {
			let res = null;

			this.map.geoObjects.each(item => {
				if (item.geometry.getType() === 'LineString') res = item;
			});

			return res;
		},

		getWaypointsMapObjects() {
			let res = [];

			this.map.geoObjects.each(item => {
				if (item.geometry.getType() === 'Point') res.push(item);
			});

			return res;
		},

		// Сериализует объекты геометрии, хранимые в объекте "Марафон", в строки.
		serializeMarathonGeometries() {
			let vm = this;

			if (typeof vm.marathon.route === 'object')
				vm.marathon.route = JSON.stringify(vm.marathon.route);

			vm.marathon.waypointInfos.forEach(o => {
				if (typeof o.location === 'object')
					o.location = JSON.stringify(o.location);
			});
		},

		// Десериализует строковые описания геометрии в объекты.
		deserializeMarathonGeometries() {
			let vm = this;

			if (typeof vm.marathon.route === 'string')
				vm.marathon.route = JSON.parse(vm.marathon.route);

			vm.marathon.waypointInfos.forEach(o => {
				if (typeof o.location === 'string') o.location = JSON.parse(o.location);
			});
		},

		// Отменяет все изменения и перерисовывает маршрут по данным
		// из модели.
		cancelEdit() {
			this.map.geoObjects.removeAll();
			this.disableFeatureEditors();
			this.drawRoute();

			this.editMode = null;
		},

		clearRoute() {
			let routeMapObject = this.getRouteMapObject();

			this.map.geoObjects.remove(routeMapObject);
			this.marathon.route = null;
		},

		// Обрабатывает процесс создания описания точки маршрута.
		// Открывает соответствующий диалог.
		handleCreateWaypoint(e) {
			let coords = e.get('coords');
			let mark = new ymaps.Placemark(coords);

			mark.properties.set('innerId', this.newMarkersIdSequence--);
			this.map.geoObjects.add(mark);
			this.assignMapEvents(false);
			this.assignMapEvents();

			this.dirtyFlag = true;

			mark.editor.startEditing();

			if (this.showWaypointEditor)
				this.showWaypointEditor({ mark: mark });
		},

		// Обрабатывает процесс отрисовки маршрута.
		handleCreateRoute(e) {
			let vm = this;
			let coords = e.get('coords');

			if (!vm.routeFirstPoint) {
				let mark = new ymaps.Placemark(coords);

				vm.routeFirstPoint = mark;

				vm.map.geoObjects.add(mark);
			}
			else {
				let startingPoints = [
					vm.routeFirstPoint.geometry.getCoordinates(),
					coords
				];
				let routeMapObject = new ymaps.Polyline(startingPoints, { }, vm.routeOptions);

				vm.map.geoObjects.remove(vm.routeFirstPoint);
				vm.map.geoObjects.add(routeMapObject);
				vm.assignMapEvents(false);
				vm.assignMapEvents();
				routeMapObject.editor.stopFraming();
				routeMapObject.editor.startEditing();

				vm.routeFirstPoint = null;
				vm.editMode = null;
				vm.dirtyFlag = true;
				
				if (vm.disableRouteCreation)
					vm.disableRouteCreation();
			}
		},

		getWaypointModel(id) {
			if (!id)
				return null;

			return this.marathon.waypointInfos.find(
				o => o.innerId === id
			);
		},

		// Обработчик завершения инициализации карты.
		onMapInitialized(map) {
			let vm = this;

			vm.map = map;
			vm.marathon.route = vm.prepareGeometry(vm.marathon.route);
			vm.marathon.waypointInfos = vm.prepareWaypoints(
				vm.marathon.waypointInfos
			);

			if (vm.marathon.route && vm.marathon.route.features)
				vm.marathon.route.features[0].options = vm.routeOptions;

			vm.drawRoute();
		},

		// Обработчик щелчка мышью по области карты.
		onMapClick(e) {
			if (this.editMode === 'createWaypoint')
				this.handleCreateWaypoint(e);
			else if (this.editMode === 'createRoute')
				this.handleCreateRoute(e);
		},

		// Обработчик щелчка мышью по геометрическому объекту на карте.
		onFeatureClick(e) {
			let vm = this;
			let target = e.originalEvent.target;
			let id = target.properties.get('innerId');
			let isWaypoint = target.geometry.getType() === 'Point';

			if (vm.editMode === 'edit') {
				if (isWaypoint && id && target.editor.state.get('editing')) {
					if (vm.showWaypointEditor)
						vm.showWaypointEditor(vm.getWaypointModel(id));
				} else {
					vm.disableFeatureEditors();
					target.editor.stopFraming();
					target.editor.startEditing();
				}
				vm.dirtyFlag = true;
			} else {
				if (isWaypoint && id && vm.showWaypointInfo)
					vm.showWaypointInfo(vm.getWaypointModel(id));
			}
		},

		onFeatureDoubleClick(e) {
			console.log(e);
		},

		// Обновляет для модели текущую длину маршрута в километрах при каждом изменении
		// его геометрии.
		onRouteGeometryChanged(e) {
			this.marathon.distance = e.originalEvent.target.geometry.getDistance() / 1000;
		}
	}
};
</script>
