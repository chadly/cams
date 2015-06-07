var React = require("react");
var api = require("./api");

var CameraTabs = require("./camera/tabs");
var CameraCalendar = require("./camera/calendar");
var VideoList = require("./camera/video/list");

var Main = React.createClass({
	getInitialState: function() {
		return {
			cameras:[]
		};
	},
	componentDidMount: function() {
		api.getCameras().done(function(response) {
			if (this.isMounted()) {
				this.setState({
					cameras: response.body
				});
			}
		}.bind(this), function() {
			if (this.isMounted()) {
				this.setState({
					isErrored: true
				});
			}
		}.bind(this));
	},
	selectDate: function(date) {
		console.log(date);
	},
	render: function() {
		if (this.state.isErrored) {
			return <span>There was an error loading the cameras.</span>;
		}

		return (
			<div>
				<CameraTabs cameras={this.state.cameras} />
				<CameraCalendar camera="front-door" onDateSelected={this.selectDate} />
			</div>
		);
	}
});

module.exports = Main;
