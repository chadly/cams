var React = require("react");
var api = require("./../../api");
var _ = require("lodash");
var moment = require("moment");

var FullCalendar = require("./fullcal");

var CameraCalendar = React.createClass({
	propTypes: {
		camera: React.PropTypes.string.isRequired
	},
	getInitialState: function() {
		return {
			dates: [],
			isLoading: true
		};
	},
	componentDidMount: function() {
		this.reloadDates();
	},
	componentWillReceiveProps: function() {
		this.reloadDates();
	},
	reloadDates: function() {
		api.getCameraDates(this.props.camera).done(function(response) {
			if (this.isMounted()) {
				this.setState({
					dates: response.body.map(function(date) {
						return _.extend({}, date, {
							date: moment(date.date)
						});
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
			<FullCalendar dates={this.state.dates} />
		);
	}
});

module.exports = CameraCalendar;
