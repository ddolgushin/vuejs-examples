import axios from 'axios';

var axiosInstance = axios.create({
  baseURL: 'http://localhost:5678/api/',
  headers: {
    'Content-Type': 'application/json, text/plain, */*'
  }
});

export default {
  getAll(callback, failureCallback) {
    return axiosInstance
      .get('marathon')
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);
				
				if (failureCallback)
					failureCallback(e);
      });
  },

  get(id, callback, failureCallback) {
    return axiosInstance
      .get(`marathon/${id}`)
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);
				
				if (failureCallback)
					failureCallback(e);
      });
  },

  create(callback, failureCallback, header) {
		return axiosInstance
      .post('marathon', null, { headers: header })
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);

				if (failureCallback)
					failureCallback(e);
      });
	},

  update(marathon, callback, failureCallback, header) {
    return axiosInstance
      .put('marathon', marathon, { headers: header })
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);

				if (failureCallback)
					failureCallback(e);
      });
  },

  ownedBy(marathonId, userId, callback, failureCallback) {
    return axiosInstance
      .get(`marathon/${marathonId}/ownedby/${userId}`)
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);
				
				if (failureCallback)
					failureCallback(e);
			});
	},

  getParticipants(id, callback, failureCallback) {
    return axiosInstance
      .get(`marathon/${id}/participants`)
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);
				
				if (failureCallback)
					failureCallback(e);
      });
	},

	assignParticipant(marathonId, callback, failureCallback, header) {
    return axiosInstance
      .get(`marathon/assign/${marathonId}`, { headers: header })
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);
				
				if (failureCallback)
					failureCallback(e);
			});
	},

	declineParticipant(marathonId, callback, failureCallback, header) {
    return axiosInstance
			.get(`marathon/decline/${marathonId}`, { headers: header })
      .then(response => {
        if (callback) callback(response.data);
      })
      .catch(e => {
				console.error(e);

				if (failureCallback)
					failureCallback(e);
			});
  }
};
