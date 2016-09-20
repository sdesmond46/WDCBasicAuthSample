window._api = new Api("http://sdesmond5:8080"); // ALWAYS use https in a production environment

(function(){
    var myConnector = tableau.makeConnector();

    myConnector.init = function (initCallback) {
        if (tableau.phase == tableau.phaseEnum.gatherDataPhase) {
            window._api.setCredentials(tableau.username, tableau.password);
            window._api.testCredentials(function (err) {
                if (err) {
                    tableau.abortForAuth(err.toString());
                } else {
                    initCallback();
                }
            });
        } else {
            initCallback();
        }
    };

    myConnector.getSchema = function(schemaCallback) {
        window._api.getSchema(schemaCallback);
    };

    myConnector.getData = function(table, doneCallback) {
        window._api.getData(function (data) {
            table.appendRows(data);
            doneCallback();
        });
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

    $("#username").val(tableau.username);
    $("#password").val(tableau.password);

	$('#login').submit(function( event ) {
        event.preventDefault();
        var username = $("#username").val();
        var password = $("#password").val();

        window._api.setCredentials(username, password);

        tableau.username = username;
        tableau.password = password;
        tableau.authType = tableau.authTypeEnum.basic;

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