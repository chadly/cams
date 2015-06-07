var React = require("react");

var CameraTabs = React.createClass({
	propTypes: {
		cameras: React.PropTypes.array.isRequired
	},
	render: function() {
		return (
			<ul className="nav nav-tabs">
				{this.props.cameras.map(function(cam) {
					return (
						<li key={cam.id} role="presentation">
							<a href={'/' + cam.id}>{cam.name}</a>
						</li>
					);
				})}
			</ul>
		);
	}
});

module.exports = CameraTabs;
