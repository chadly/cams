var React = require("react");
var FluxComponent = require("flummox/component");
var FluxApp = require("./app");

var app = new FluxApp();

React.render((
	<FluxComponent flux={app}>
		Hello, world
	</FluxComponent>
), document.getElementById("react-app"));
