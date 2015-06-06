var React = require("react");
var api = require("./api");

var VideoList = React.createClass({
	propTypes: {
		camera: React.PropTypes.string.isRequired,
		date: React.PropTypes.string.isRequired
	},
	getInitialState: function() {
		return {
			videos: [],
			isLoading: true
		};
	},
	componentDidMount: function() {
		api.getVideos(this.props.camera, this.props.date).then(function(response) {
			if (this.isMounted()) {
				this.setState({
					videos: response.body[this.props.date],
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
			return <span>Error loading videos.</span>;
		}

		return (
			<ul className="nav nav-tabs">
				{this.props.cameras.map(function(cam) {
					return (
						<li key={cam.id} role="presentation">
							<a href={'#' + cam.id}>{cam.name}</a>
						</li>
					);
				})}
			</ul>
		);
	}
});

module.exports = CameraTabs;
