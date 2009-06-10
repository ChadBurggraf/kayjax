//
// Library-independent helper for interacting with the Kayson
// JSON framework.
//
Kayjax = {
    // Pending request.
    requests: [],
    
    // Maximum request count.
    maxCount: 10,
    
    // Global callbacks.
    onStop: function(){},
    onSend: function(){},
    onStart: function(){},
    onReceive: function(){},
    
    // When implemented, appends a querystring to a url.
    appendQuery: function(url, query) { },
    
    // Fires the Ajax request end events.
    fireEndEvents: function() {
        if (typeof Kayjax.onReceive === "function") {
            try {
                Kayjax.onReceive();
            } catch (e) { }
        }
        
        if (Kayjax.requests.length === 0 && typeof Kayjax.onStop === "function") {
            try {
                Kayjax.onStop();
            } catch (e) { }
        }
    },
    
    // Fires the Ajax request start events.
    fireStartEvents: function() {
        if (Kayjax.requests.length === 0 && typeof Kayjax.onStart === "function") {
            try {
                Kayjax.onStart();
            } catch (e) { }
        }
        
        if (typeof Kayjax.onSend === "function") {
            try { 
                Kayjax.onSend(); 
            } catch (e) { }
        }
    },
    
    // Gets a guess at a failure reason from a response code.
    getReasonFromResponseCode: function(code) {
        var reason = "OK";
        
        if (code >= 300) {
            if (code < 400) {
                reason = "The server attempted to redirect the request.";
            } else if (code < 500) {
                reason = "Invalid request or access denied.";
            } else if (code < 600) {
                reason = "Internal server error.";
            } else {
                reason = "An unspecified HTTP error occurred.";
            }
        }
        
        return reason;
    },
    
    // Resolves an ASP.NET application-relative URL.
    resolveUrl: function(url) {
        var rootExp = /^~\//;

        if (rootExp.test(url)) {
            if (typeof Kayjax.appPath === "string") {
                if (!/\/$/.test(Kayjax.appPath)) {
                    Kayjax.appPath = Kayjax.appPath + "/";
                }
            } else {
                Kayjax.appPath = "/";
            }
            
            url = url.replace(rootExp, Kayjax.appPath);
        }
        
        return url;
    },
    
    // When implemented, makes a request.
    request: function(url, options) { },
    
    // Process a response.
    response: function(transport, jsonEvaluator) {
        var obj, queued, newQueue, i, n, id, resp, text;
        
        newQueue = [];
        id = transport.getResponseHeader("X-Response-Id");
        resp = {success: true, reason: "", value: null};
        text = transport.responseText || "";
        
        try {
            obj = jsonEvaluator(text.replaceJSONDates());
            resp.success = obj.success;
            resp.reason = obj.reason;
            resp.value = obj.value; 
        } catch (e) {
            resp.success = false;
            resp.reason = "An invalid response was received from the server.";
        }
        
        // If we failed and we don't have a reason, try and glean
        // one from the status code.
        if (!resp.success && !resp.reason) {
            resp.reason = Kayjax.getReasonFromResponseCode(transport.status);
        }
        
        // Find the request item and remove it from the queue.
        for(i = 0, n = Kayjax.requests.length; i < n; i++) {
            if (Kayjax.requests[i].id.toString() === id) {
                queued = Kayjax.requests[i];
            } else {
                newQueue.push(Kayjax.requests[i]);
            }
        }
        
        // Update the queue.
        Kayjax.requests = newQueue;

        if (queued) {
            if (typeof queued.callback === "function") {
                queued.callback(resp);
            }
        } else {
            if (!resp.success) {
                alert(resp.reason);
            }
        }
        
        Kayjax.fireEndEvents();
        
        return resp;
    },
    
    // When implemented, makes an update request.
    update: function(element, url, options) { },
    
    // Processes an update response.
    updateResponse: function(transport, element) {
        var id, queued, newQueue, resp, i, n;
        
        id = transport.getResponseHeader("X-Response-Id");
        newQueue = [];
        resp = {success: true, reason: "", value: ""};
        
        if (transport.status !== 0 && !(transport.status >= 200 && transport.status < 300)) {
            resp.success = false;
            resp.reason = Kayjax.getReasonFromResponseCode(transport.status);
        } else {
            resp.value = transport.responseText || "";
        }
        
        // Find the request item and remove it from the queue.
        for(i = 0, n = Kayjax.requests.length; i < n; i++) {
            if (Kayjax.requests[i].id.toString() === id) {
                queued = Kayjax.requests[i];
            } else {
                newQueue.push(Kayjax.requests[i]);
            }
        }
        
        // Update the queue.
        Kayjax.requests = newQueue;
        
        // Update the element.
        if (typeof element === "string") {
            element = document.getElementById(element);
        }
        
        element.innerHTML = resp.value;
        
        if (queued) {
            if (typeof queued.callback === "function") {
                queued.callback(resp);
            }
        } else {
            if (!resp.success) {
                alert(resp.reason);
            }
        }
        
        Kayjax.fireEndEvents();

        return resp;
    }
};

/*
 * Date extensions.
 */

// Converts a date to an ASP.NET JSON string.
Date.prototype.toJson = function() {
    return '"\\\/Date(' + this.getTime() + ')\\\/"';
};

// Gets a short date string.
Date.prototype.toShortDateString = function(format, separator) {
    format = format || "ymd";
    separator = separator || "-";

	var year = this.getFullYear();
	var month = this.getMonth() + 1;
	var date = this.getDate();
	
	if (month < 10) month = "0" + month;
	if (date < 10) date = "0" + date;
	
	switch(format) {
	    case("mdy"):
	        return month + separator + date + separator + year;
	    case("dmy"):
            return date + separator + month + separator + year;
	    default:
	        return year + separator + month + separator + date;
	}
};

// Gets a short time string.
Date.prototype.toShortTimeString = function() {
    var hours = this.getHours();
    var minutes = this.getMinutes();
    var ampm = hours < 12 ? "AM" : "PM";
    
    if (hours == 0) {
        hours = 12;
    } else if (hours > 12) {
        hours -= 12;
    }
    
    if (minutes < 10) { 
        minutes = "0" + minutes;
    }
    
    return hours + ":" + minutes + " " + ampm;
};

// Creates a date from an ASP.NET JSON date representation.
Date.fromJson = function(json) {
    return typeof json === "string" && json.isJsonDate() ? 
        eval(json.replace(String.DATE_EXP, "new Date($1)")) : 
        NaN;
};

/*
 * String extensions.
 */
 
// Gets a camel-case version of the string.
String.prototype.camel = function() {
    var matches = /^([A-Z])/.exec(this);
    
    if (matches) {
        return matches[1].toLowerCase() + this.substr(1);
    } else {
        return this;
    }
};

// Gets the string's URL scheme, if the string is a URL.
String.prototype.getUrlScheme = function() {
    var scheme = "";
    var match = /^([^:]+)/.exec(this);
    
    if (match && match.length > 1) {
        scheme = match[1];
    }
    
    return scheme;
};

// Gets a value indicating whether this string is an ASP.NET JSON date.
String.prototype.isJsonDate = function() {
    return this.match(String.DATE_EXP);
};

// Replaces all of the ASP.NET JSON dates in the string
// with date strings that can be eval()'d.
String.prototype.replaceJSONDates = function() {
    return this.replace(/"\\\/Date\(([\d\-]+)\)\\\/"/g, "new Date($1)");
};

// Trims left and right whitespace from the string.
String.prototype.trim = function() {
    return this.replace(/^\s+/g, '').replace(/\s+$/g, '');
};

// A regular expression defining how ASP.NET JSON dates look.
String.DATE_EXP = (/^\/Date\(([\d-]+)\)\/$/);