function CallGlobalCustomAction() {
    var globalContext = Xrm.Utility.getGlobalContext();    
    var serverURL = globalContext.getClientUrl();
    var actionName = "seismic_MyGlobalAction";

    var InputParameterValue = globalContext.userSettings.userId; 
    var data = { };

    var req = new XMLHttpRequest(); 
    req.open("POST", serverURL + "/api/data/v9.0/" + actionName, true); 
    req.setRequestHeader("Accept", "application/json"); 
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8"); 
    req.setRequestHeader("OData-MaxVersion", "4.0"); 
    req.setRequestHeader("OData-Version", "4.0"); 
    req.onreadystatechange = function () { 
        if (this.readyState === 4) { 
            req.onreadystatechange = null; 

            if (this.status === 200 || this.status === 204)  { 
                alert("Action Executed Successfully called from WebResource...");
                var result = JSON.parse(this.response);
                alert(result.MyOutputParameter);
            } 

            else  { 
                var error = JSON.parse(this.response).error; 
                alert("Error in Action: "+error.message); 
            } 
        } 
    }; 

    req.send(window.JSON.stringify(data)); 
}
