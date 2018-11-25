import axios from "axios";

var axiosInstance = axios.create({
	baseURL: 'http://localhost:5678/api/',
	headers: {
		'Content-Type': 'application/json, text/plain, */*'
	}
});

export default {
	login(request, callback, failureCallback) {
		return axiosInstance.post('auth', request)
			.then(response => {
				if (callback)
					callback(response.data);
			})
			.catch(e => {
				console.error(e);

				if (failureCallback)
					failureCallback(e);
			});
	},

	logout(callback, failureCallback) {
		return axiosInstance.get('auth/logout')
			.then(response => {
				if (callback)
					callback(response.data);
			})
			.catch(e => {
				console.error(e);

				if (failureCallback)
					failureCallback(e);
			});
	}
}
