var React = require("react");

var CameraCalendar = React.createClass({
	propTypes: {
		dates: React.PropTypes.array.isRequired
	},
	render: function() {
		return (
			<ul>
				{this.props.dates.map(function(date) {
					var formattedDate = date.date.format("YYYY-MM-DD");

					return (
						<li key={formattedDate}>
							<a href={formattedDate}>{date.date.format("ddd, MMM Do YYYY")} ({date.videoCount} videos)</a>
						</li>
					);
				})}
			</ul>
		);
	}
});

module.exports = CameraCalendar;
