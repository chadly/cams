var React = require("react");

var CameraTabs = React.createClass({
	propTypes: {
		cameras: React.PropTypes.array.isRequired,
		selected: React.PropTypes.string
	},
	render: function() {
		return (
			<ul className="nav nav-tabs">
				{this.props.cameras.map(function(cam) {
					var isActive = this.props.selected == cam.id ? "active" : "";

					return (
						<li key={cam.id} className={isActive} role="presentation">
							<a href={'/' + cam.id + '/'}>{cam.name}</a>
						</li>
					);
				}.bind(this))}
			</ul>
		);
	}
});

module.exports = CameraTabs;
