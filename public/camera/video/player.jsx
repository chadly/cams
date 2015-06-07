var React = require("react");
var NativeListener = require("react-native-listener");

var VideoPlayer = React.createClass({
	propTypes: {
		video: React.PropTypes.object.isRequired,
		isPlaying: React.PropTypes.bool.isRequired,
		onPlay: React.PropTypes.func.isRequired
	},
	playVideo: function(ev) {
		ev.preventDefault();
		ev.stopPropagation();

		this.props.onPlay(this.props.video.url);
	},
	render: function() {
		var content = (
			<NativeListener onClick={this.playVideo}>
				<a href={this.props.video.url}>
					<img src={this.props.video.thumbnail} />
				</a>
			</NativeListener>
		);

		if (this.props.isPlaying) {
			content = (
				<div className="embed-responsive embed-responsive-16by9">
					<video className="embed-responsive-item" src={this.props.video.url} controls autoPlay>
						Cool video, bro
					</video>
				</div>
			);
		}

		return (
			<div className="col-md-4">
				<div className="thumbnail">
					{content}

					<div className="caption text-center">
						<h4>{this.props.video.time.format("h:mm:ss a")}</h4>
					</div>
				</div>
			</div>
		);
	}
});

module.exports = VideoPlayer;
