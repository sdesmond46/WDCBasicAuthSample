window._api = new Api("http://localhost:30977"); // ALWAYS use https in a production environment

(function(){
    var myConnector = tableau.makeConnector();

    myConnector.init = function(initCallback) {

    };

    myConnector.getSchema = function(schemaCallback) {

    };

    myConnector.getData = function(table, doneCallback) {

    };

    tableau.registerConnector(myConnector);
})();

$(document).ready(function() {
    function changeStatusMessage(msg, color) {
        var control = $("#statusMessage");
        if (!msg) {
            control.css("display", "none");
        } else {
            control.css("display", "block");
            control.text(msg);
            control.css("color", color);
        }
    }

	$('#login').submit(function( event ) {
        event.preventDefault();
        var username = $("#username").val();
        var password = $("#password").val();

        window._api.setCredentials(username, password);

        tableau.username = username;
        tableau.password = password;

        changeStatusMessage("Testing credentials...", "green");
        window._api.testCredentials(function(err) {
            if (!err) {
				changeStatusMessage("Valid Credentials", "black");
                tableau.connectionName = "Basic Auth Sample for " + tableau.username;
                tableau.submit();
            } else {
                changeStatusMessage(err.toString(), "red");
            }
        });
    });
});