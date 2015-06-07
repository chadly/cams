var React = require("react");
var api = require("./../api");
var _ = require("lodash");
var moment = require("moment");

var CameraCalendar = React.createClass({
	propTypes: {
		camera: React.PropTypes.string.isRequired
	},
	getInitialState: function() {
		return {
			dates: [],
			isLoading: true,
			selectedDate: null
		};
	},
	componentDidMount: function() {
		api.getCameraDates(this.props.camera).done(function(response) {
			if (this.isMounted()) {
				this.setState({
					dates: response.body.dates.map(function(date) {
						return moment(date);
					}),
					isLoading: false
				});
			}
		}.bind(this), function() {
			if (this.isMounted()) {
				this.setState({
					isErrored: true,
					isLoading: false
				});
			}
		}.bind(this));
	},
	render: function() {
		if (this.state.isLoading) {
			return <span>Loading...</span>;
		}

		if (this.state.isErrored) {
			return <span>Error loading dates.</span>;
		}

		return (
			<select>
				{this.state.dates.map(function(date) {
					return <option key={date.format("X")}>{date.format("ddd, MMM Do")}</option>;
				})}
			</select>
		);
	}
});

module.exports = CameraCalendar;
