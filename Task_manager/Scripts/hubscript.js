﻿$(function () {
    // Reference the hub.
    var hubNotif = $.connection.tasksHub;

    // Start the connection.
    $.connection.hub.start().done(function () {
        getAll();
    });
    //hubNotif.сlient.updatedData()
    // Notify while anyChanges.
    hubNotif.client.updatedData = function () {
        getAll();
    };
});

function getAll() {
    var model = $('#dataTable');
    $.ajax({
        url: '/task/GetViewTasks',
        contentType: 'application/html ; charset:utf-8',
        type: 'GET',
        dataType: 'html'
    }).success(function (result) {
        model.empty().append(result);
    }).error(function (e) {
        alert(e);
    });
}