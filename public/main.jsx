var React = require("react");
var api = require("./api");

var CameraTabs = require("./camera/tabs");

var Main = React.createClass({
	getInitialState: function() {
		return {
			cameras:[]
		};
	},
	componentDidMount: function() {
		api.getCameras().then(function(response) {
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
	render: function() {
		if (this.state.isErrored) {
			return <span>There was an error loading the cameras.</span>;
		}

		return (
			<CameraTabs cameras={this.state.cameras} />
		);
	}
});

module.exports = Main;
