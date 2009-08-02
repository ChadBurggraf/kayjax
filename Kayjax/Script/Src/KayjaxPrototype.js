//
// Prototype-specific Kayjax implementation.
//
Object.extend(Kayjax, {
    // Appends a querystring to a url.
    appendQuery: function(url, query) { 
        var q = (url.indexOf("?") >= 0) ? url.parseQuery() : {};
        Object.extend(q, (Object.isString(query) ? query.parseQuery() : {}));
        query = Object.toQueryString(q);
        return url.split("?")[0] + (query ? "?" + query : "");
    },
    
    // Makes a request.
    request: function(url, options) {
        if (Kayjax.requests.length >= Kayjax.maxCount) {
            setTimeout(Kayjax.request.curry(url, options), 100);
        } else {
            Kayjax.fireStartEvents();
            options = options || {};
            
            var queued, postBody, params, req;
            
            queued = {
                id: Kayjax.requests.length,
                callback: options.callback || null
            };
            
            Kayjax.requests.push(queued);
            
            postBody = options.parameters ? Object.toJSON(Kayjax.prepareParameters(options.parameters)) : "";
            
            params = {
                requestHeaders: ["X-Request-Id", queued.id],
                contentType: "application/json",
                postBody: postBody,
                asynchronous: (Object.isFunction(queued.callback)) ? true : false
            };
            
            if (params.asynchronous) {
                params.onComplete = function(transport) {
                    Kayjax.response(transport, function(json) {
                        return eval('(' + json.unfilterJSON() + ')');
                    });
                };
            }

            req = new Ajax.Request(Kayjax.appendQuery(Kayjax.resolveUrl(url), options.query), params);
            
            if (!params.asynchronous) {
                return Kayjax.response(new Ajax.Response(req));
            }
        }
    },
    
    // Makes an update request.
    update: function(element, url, options) {
        if (Kayjax.requests.length >= Kayjax.maxCount) {
            setTimeout(Kayjax.requests.curry(element, url, options), 100);
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
                requestHeaders: ["X-Request-Id", queued.id],
                asynchronous: true,
                postBody: options.postBody,
                onComplete: function(transport) {
                    Kayjax.updateResponse(transport, element);
                }
            };

            new Ajax.Request(Kayjax.appendQuery(Kayjax.resolveUrl(url), options.query), params);
        }
    }
});