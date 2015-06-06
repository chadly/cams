var React = require("react");

var VideoPlayer = React.createClass({
	propTypes: {
		video: React.PropTypes.object.isRequired,
		isPlaying: React.PropTypes.bool.isRequired,
		onPlay: React.PropTypes.func.isRequired
	},
	playVideo: function(ev) {
		ev.preventDefault();
		this.props.onPlay(this.props.video.url);
	},
	render: function() {
		if (this.props.isPlaying) {
			return (
				<div className="col-xs-6 col-md-3">
					<div className="embed-responsive embed-responsive-16by9">
						<video className="embed-responsive-item" src={this.props.video.url} controls autoPlay>
							Cool video, bro
						</video>
					</div>
				</div>
			);
		}

		return (
			<div className="col-xs-6 col-md-3">
				<div className="thumbnail">
					<a href={this.props.video.url} onClick={this.playVideo}>
						<img src={this.props.video.thumbnail} />
					</a>
				</div>
			</div>
		);
	}
});

module.exports = VideoPlayer;
