var React = require("react");
var api = require("./api");
var page = require("page");

var CameraTabs = require("./camera/tabs");
var CameraCalendar = require("./camera/calendar");
var VideoList = require("./camera/video/list");

var Main = React.createClass({
	getInitialState: function() {
		return {
			cameras:[],
			selectedCamera: null,
			selectedDate: null
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

		page("/", function(ctx) {
			this.setState({
				selectedCamera: null,
				selectedDate: null
			});
		}.bind(this));

		page("/:camera", function(ctx) {
			this.setState({
				selectedCamera: ctx.params.camera,
				selectedDate: null
			});
		}.bind(this));

		page("/:camera/:date", function(ctx) {
			this.setState({
				selectedCamera: ctx.params.camera,
				selectedDate: ctx.params.date
			});
		}.bind(this));

		page.start();
	},
	render: function() {
		if (this.state.isErrored) {
			return <span>There was an error loading the cameras.</span>;
		}

		var calendar = !!this.state.selectedCamera ?
			<CameraCalendar camera={this.state.selectedCamera} onDateSelected={this.selectDate} /> : null;

		return (
			<div>
				<CameraTabs cameras={this.state.cameras} selected={this.state.selectedCamera} />
				{calendar}
			</div>
		);
	}
});

module.exports = Main;
