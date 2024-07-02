// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let rootURL = window.location.origin;

// form submission handling

function home_page_signup_form_submit() {
    let $form = $(`#home_page_signup_form`);
    $form.css({ opacity: 0.5 });
    
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: $form.attr('action'),
        data: $form.serialize(),
        error: function () {
            $('#form-error').show();
        },
        success: function (response) {
            if (response.result) {
                $form.hide();
                $('#signed-by-name').html(response.signedByName);
                $('#form-completed').show();
            }
            else {
                $('#form-error').show();
            }
        }
    });

    $form.css({ opacity: 1 });
    return false;
}

