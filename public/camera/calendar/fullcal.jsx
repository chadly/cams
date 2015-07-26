var React = require("react");
var _ = require("lodash");

var CameraCalendar = React.createClass({
	propTypes: {
		dates: React.PropTypes.array.isRequired
	},
	render: function() {
		return (
			<ul>
				{_(this.props.dates).reverse().map(function(date) {
					var formattedDate = date.date.format("YYYY-MM-DD");

					return (
						<li key={formattedDate}>
							<a href={formattedDate}>{date.date.format("ddd, MMM Do YYYY")} ({date.videoCount} videos)</a>
						</li>
					);
				}).value()}
			</ul>
		);
	}
});

module.exports = CameraCalendar;
