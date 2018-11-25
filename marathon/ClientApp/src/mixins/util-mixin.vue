<script>
export default {
	name: 'UtilMixin',
	methods: {
		formatDate(date, delim = '.', reverse = false) {
			if (typeof(date) !== 'object')
				date = new Date(date);

			let y = this.zeroPad(date.getYear() + 1900, 4);
			let m = this.zeroPad(date.getMonth() + 1, 2);
			let d = this.zeroPad(date.getDate(), 2);

			return reverse
				? `${y}${delim}${m}${delim}${d}`
				: `${d}${delim}${m}${delim}${y}`;
		},

    zeroPad(number, width) {
      width -= number.toString().length;

      if (width > 0)
        return new Array( width + (/\./.test( number ) ? 2 : 1) ).join( '0' ) + number;

      return number + "";
		},

		handleError(err) {
			console.log(err);

			if (err.response.data)
				alert(err.response.data);
		}
	}
};
</script>
