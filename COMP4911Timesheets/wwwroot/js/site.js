// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var root;
var subWPCodeLength;
$('[data-toggle="collapse"]').on('click', function () {
    
    if (root == null) {
        root = $(this).attr('id').substring(0, 5);
        subWPCodeLength = $(this).attr('id').length + 1;
    }


    if (root != $(this).attr('id').substring(0, 5)) {
        jQuery('.collapse').collapse('hide');
    }

    if (((subWPCodeLength - $(this).attr('id').length) > 1)
        && (root == $(this).attr('id').substring(0, 5))) {
            jQuery('.collapse').collapse('hide');
    }

    root = $(this).attr('id').substring(0, 5);
    subWPCodeLength = $(this).attr('id').length + 1;
});