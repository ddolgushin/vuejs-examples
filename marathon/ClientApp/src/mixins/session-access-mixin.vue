<script>
export default {
	name: 'SessionAccessMixin',
	methods: {
		sessionExists() {
			return this.sessionGetUser() !== null;
		},

		sessionGetUser() {
			let userInStorage = localStorage.getItem('user');

			if (userInStorage)
				return JSON.parse(userInStorage);
			
			return null;
		},

		sessionRemoveUser() {
			localStorage.removeItem('user');
		},

		sessionGetUserId() {
			let user = this.sessionGetUser();

			return user ? user.id : null;
		},

		sessionGetAuthHeader() {
			let user = this.sessionGetUser();

			return user ?  { Authorization: 'Bearer ' + user.jwt } : null;
		}
	}
};
</script>
