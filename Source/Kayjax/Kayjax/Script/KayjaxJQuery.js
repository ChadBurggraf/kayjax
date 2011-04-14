//
// JQuery-specific Kayjax implementation.
//
$.extend(Kayjax, {
    // Appends a querystring to a url.
    appendQuery: function(url, query) { 
        var q = (url.indexOf("?") >= 0) ? $.parseQuery(url.substr(url.indexOf("?") + 1)) : {};
        $.extend(q, (typeof query === "string" ? $.parseQuery(query) : {}));
        query = $.param(q);
        return url.split("?")[0] + (query ? "?" + query : "");
    },

    // Makes a request.
    request: function(url, options) {
        if (Kayjax.requests.length >= Kayjax.maxCount) {
            setTimeout(function() {
                Kayjax.request(url, options);
            }, 100);
        } else {
            Kayjax.fireStartEvents();
            options = options || {};
            
            var queued, postBody, params, req;
            
            queued = {
                id: Kayjax.requests.length,
                callback: options.callback || null
            };
            
            Kayjax.requests.push(queued);
            
            postBody = options.parameters ? JSON.stringify(Kayjax.prepareParameters(options.parameters)) : "";

            params = {
                contentType: "application/json",
                async: (typeof queued.callback === "function") ? true : false,
                data: postBody,
                processData: false,
                url: Kayjax.appendQuery(Kayjax.resolveUrl(url), options.query),
                type: "POST",
                beforeSend: function(req) {
                    req.setRequestHeader("X-Request-Id", queued.id);
                }
            };
            
            if (params.async) {
                params.complete = function(transport) {
                    Kayjax.response(transport, JSON.parse);
                };
            }
            
            req = $.ajax(params);
            
            if (!params.async) {
                return req;
            }
        }
    },
    
    // Makes an update request.
    update: function(element, url, options) {
        if (Kayjax.requests.length >= Kayjax.maxCount) {
            setTimeout(function() {
                Kayjax.update(element, url, options);
            }, 100);
        } else {
            Kayjax.fireStartEvents();
            options = options || {};
            
            var queued, params;
            
            queued = {
                id: Kayjax.requests.length,
                callback: options.callback || null
            };
            
            Kayjax.requests.push(queued);
            
            params = {
                async: (typeof queued.callback === "function") ? true : false,
                data: options.postBody,
                url: Kayjax.appendQuery(Kayjax.resolveUrl(url), options.query),
                type: "POST",
                beforeSend: function(req) {
                    req.setRequestHeader("X-Request-Id", queued.id);
                },
                complete: function(transport) {
                    Kayjax.updateResponse(transport, element);
                }
            };
            
            $.ajax(params);
        }
    }
});