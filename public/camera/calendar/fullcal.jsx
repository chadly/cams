var React = require("react");
require("fullcalendar");
require("fullcalendar/dist/fullcalendar.css");
var $ = require("jquery");

var CameraCalendar = React.createClass({
	propTypes: {
		dates: React.PropTypes.array.isRequired,
		onDateSelected: React.PropTypes.func.isRequired
	},
	componentDidMount: function() {
		$(this.refs.calContainer.getDOMNode()).fullCalendar({
			events: this.props.dates.map(function(date){
				return {
					title: date.videoCount + " videos",
					start: date.date,
					allDay: true
				};
			}),
			dayClick: function(date) {
				this.props.onDateSelected(date.format("YYYY-MM-DD"));
			}.bind(this)
		});
	},
	componentWillUnmount: function() {

	},
	render: function() {
		return <div ref="calContainer"></div>;
	}
});

module.exports = CameraCalendar;
