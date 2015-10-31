var React = require("react");
var api = require("./../api");

var CameraTabs = React.createClass({
	propTypes: {
		cameras: React.PropTypes.array.isRequired,
		selected: React.PropTypes.string
	},
	getInitialState: function() {
		return {
			isProcessing: false
		};
	},
	process: function(ev) {
		ev.preventDefault();

		if (this.state.isProcessing) {
			return;
		}

		this.setState({
			isProcessing: true
		});

		api.process().then(doneProcessing.bind(this), doneProcessing.bind(this));

		function doneProcessing(output) {
			console.log(output.text);
			this.setState({
				isProcessing: false
			});
		}
	},
	render: function() {
		var items = this.props.cameras.map(function(cam) {
			var isActive = this.props.selected == cam.id ? "active" : "";

			return (
				<li key={cam.id} className={isActive} role="presentation">
					<a href={'/' + cam.id + '/'}>{cam.name}</a>
				</li>
			);
		}.bind(this));

		var spinner = this.state.isProcessing ? <i className="fa fa-refresh fa-spin"></i> : null;

		items.push(
			<li key="process_manually" role="presentation">
				<a href="#" onClick={this.process} className={this.state.isProcessing ? "disabled" : ""}>
					{spinner}
					Process Raw Footage
				</a>
			</li>
		);

		return (
			<ul className="nav nav-tabs">
				{items}
			</ul>
		);
	}
});

module.exports = CameraTabs;
