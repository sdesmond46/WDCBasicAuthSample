function Api(baseUrl) {
    this.baseUrl = baseUrl;
}

Api.prototype.setCredentials = function(username, password) {
    this.username = username;
    this.password = password;
}

Api.prototype.testCredentials = function(cb) {
	var url = this.baseUrl + "/api/auth/TestAuth";
	$.ajax({
	  url: url,
	  beforeSend: this._setRequestHeaders.bind(this)
	}).done(function(data, textStatus, jqXHR) {
		cb();
	}).fail(function(jqXHR, textStatus, errorThrown) {
		cb(jqXHR.responseText);
	});
}

Api.prototype._setRequestHeaders = function(xhr) {
	var combinedField = this.username + ":" + this.password;
	var val = "Basic " + btoa(combinedField);
	xhr.setRequestHeader("Authorization", val);
	// return {"Authorization", val};
}