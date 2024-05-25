function AjaxCallPost(Url, data) {
    var xhr = new XMLHttpRequest();

    xhr.open('POST', Url, false); // The third parameter indicates synchronous request
    xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    xhr.send(JSON.stringify(data));

    // Wait until the ready state is 4
    while (xhr.readyState !== 4) {
        // You might want to add a timeout here to prevent an infinite loop
    }

    if (xhr.status === 200) {
        return JSON.parse(xhr.responseText);
    } else {
        throw new Error(xhr.statusText);
    }
}

function AjaxCallPostNew(Url, data, successCallback, errorCallback, completeCallback) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        url: Url,
        beforeSend: function () {
            //$('#loadingmessage').show();
        },
        success: function (data) {
            if (successCallback) {
                successCallback(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (errorCallback) {
                errorCallback(jqXHR, textStatus, errorThrown);
            } else {
                alert(errorThrown + ' ' + textStatus + ' ' + jqXHR.responseText);
            }
        },
        complete: function () {
            //$('#loadingmessage').hide();
            if (completeCallback) {
                completeCallback();
            }
        }
    });
}
function AjaxCallGetNew(Url, successCallback, errorCallback, completeCallback) {
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        beforeSend: function () {
            //$('#loadingmessage').show();
        },
        success: function (data) {
            if (successCallback) {
                successCallback(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (errorCallback) {
                errorCallback(jqXHR, textStatus, errorThrown);
            } else {
                alert(errorThrown + ' ' + textStatus + ' ' + jqXHR.responseText);
            }
        },
        complete: function () {
            //$('#loadingmessage').hide();
            if (completeCallback) {
                completeCallback();
            }
        }
    });
}

function AjaxCallGet(Url) {
    var xhr = new XMLHttpRequest();

    xhr.open('GET', Url, false); // The third parameter indicates a synchronous request
    xhr.send();

    // Wait until the ready state is 4
    while (xhr.readyState !== 4) {
        // You might want to add a timeout here to prevent an infinite loop
    }

    if (xhr.status === 200) {
        return JSON.parse(xhr.responseText);
    } else {
        throw new Error(xhr.statusText);
    }
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}