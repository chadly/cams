var React = require("react");
var api = require("./../../api");

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
			<div className="row">
				{this.state.videos.map(function(video) {
					return (
						<div key={video} className="col-xs-6 col-md-3">
							<div className="thumbnail">
								<div className="embed-responsive embed-responsive-16by9">
									<video class="embed-responsive-item" src={video} controls>
										Cool video, bro
									</video>
								</div>
							</div>
						</div>
					);
				})}
			</div>
		);
	}
});

module.exports = VideoList;
