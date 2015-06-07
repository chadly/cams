var React = require("react");
var api = require("./../../api");
var _ = require("lodash");
var moment = require("moment");

var VideoPlayer = require("./player");

var VideoList = React.createClass({
	propTypes: {
		camera: React.PropTypes.string.isRequired,
		date: React.PropTypes.string.isRequired
	},
	getInitialState: function() {
		return {
			videos: [],
			isLoading: true,
			playingVideo: null
		};
	},
	componentDidMount: function() {
		api.getVideos(this.props.camera, this.props.date).done(function(response) {
			if (this.isMounted()) {
				this.setState({
					videos: response.body.map(function(vid) {
						return _.extend({}, vid, {
							time: moment(vid.time)
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
	playVideo: function(videoUrl) {
		this.setState({
			playingVideo: videoUrl
		});
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
					return <VideoPlayer
						key={video.url}
						video={video}
						isPlaying={video.url == this.state.playingVideo}
						onPlay={this.playVideo} />;
				}.bind(this))}
			</div>
		);
	}
});

module.exports = VideoList;
