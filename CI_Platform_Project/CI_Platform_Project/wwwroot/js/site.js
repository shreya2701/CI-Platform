﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('#datatable').DataTable();
    $('#example1').DataTable();


});

$(function () {
    var placeholderElement = $('#modal-placeholder');
    $('button[data-toggle="ajax-modal"]').off().click(function (event) {
        event.preventDefault();
        console.log("hii");
        var url = $(this).data('url');
        var decodeUrl = decodeURIComponent(url);
        $.get(decodeUrl).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });



    placeholderElement.on('click', '[data-save="modal"]', function (event) {

        var formData = new FormData($('#modalForm').get(0));
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        console.log(actionUrl);
        $.ajax({
            url: "/Customer/Home/" + actionUrl,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            dataType: "json",
            success: function (data) {
                placeholderElement.find('.modal').modal('hide');
            },
            error: function () {
                alert(Invalid);
            }
        });

        placeholderElement.find('.modal').modal('hide');
    });

    placeholderElement.on('click', '[data-dismiss="modal"]', function (e) {
        console.log("close");
        placeholderElement.find('.modal').modal('hide');
    });
});

//filterSelection("user")
//function filterSelection(c) {
//    var x, i;
//    x = document.getElementsByClassName("filterDiv");
//    if (c == "all") c = "";
//    // Add the "show" class (display:block) to the filtered elements, and remove the "show" class from the elements that are not selected
//    for (i = 0; i < x.length; i++) {
//        w3RemoveClass(x[i], "show");
//        if (x[i].className.indexOf(c) > -1) w3AddClass(x[i], "show");
//    }
//}

//function w3AddClass(element, name) {
//    var i, arr1, arr2;
//    arr1 = element.className.split(" ");
//    arr2 = name.split(" ");
//    for (i = 0; i < arr2.length; i++) {
//        if (arr1.indexOf(arr2[i]) == -1) {
//            element.className += " " + arr2[i];
//        }
//    }
//}

//// Hide elements that are not selected
//function w3RemoveClass(element, name) {
//    var i, arr1, arr2;
//    arr1 = element.className.split(" ");
//    arr2 = name.split(" ");
//    for (i = 0; i < arr2.length; i++) {
//        while (arr1.indexOf(arr2[i]) > -1) {
//            arr1.splice(arr1.indexOf(arr2[i]), 1);
//        }
//    }
//    element.className = arr1.join(" ");
//}

//// Add active class to the current control button (highlight it)
//var btnContainer = document.getElementsByClassName("nav-pills");
//var btns = btnContainer.getElementsByClassName("nav-link");
//for (var i = 0; i < btns.length; i++) {
//    btns[i].addEventListener("click", function () {
//        var current = document.getElementsByClassName("active xyz");
//        for (var j = 0; j < current.length; j++) {
//            current[j].className = current[j].className.replace(" border-bottom border-dark border-2 active xyz", " ");
//        }
//        this.className += " border-bottom border-dark border-2 active xyz";
//    });
//}

