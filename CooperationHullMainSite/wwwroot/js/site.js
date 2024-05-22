// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let rootURL = window.location.origin;

document.addEventListener('DOMContentLoaded', () => { pageLoadActions(); });

function pageLoadActions() {
    setNavItemActive();
    $('#homepage-formError').html();
    $('#home_page_signup_form').show();
    $('#form-completed').hide();
}

function setNavItemActive() {
    let pageURL = window.location.href.replace(rootURL, '');
    $(`#topNavBar li > a[href="${pageURL}"]`).parent().addClass('active')
}


//form submission handling

function home_page_signup_form_submit() {

    let $form = $(`#home_page_signup_form`);

    // TODO(alexis baker): Add a if error or if success as it varies.
    // Make the header bigger and different
    let header = document.querySelector('.heading-md')
    let body = document.querySelector('#callout-block-body-text');
    let block = document.querySelector('#call-out-block-text');
    let signupWrapper = document.getElementById('signup-wrapper');

    // Set new classes - Currently the same - needs style changing
    header.classList.add('bigger-font')
    block.classList.add('call-out-block-text-after-submit');
    signupWrapper.classList.add('call-out-block-wrapper-after-submit');

    // If success
    header.textContent = 'Thanks!';
    body.textContent = "We'll be in touch soon.";

    // If error
    header.textContent = "Oops, that didn't work";
    body.textContent = "Please try again later"

    $.ajax({
        type: "POST",
        dataType: 'json',
        url: $form.attr('action'),
        data: $form.serialize(),
        error: function (xhr, status, error) {
            $('#homepage-form-to-complete #homepage-formError').html("Something went wrong.  Please try again later");
        },
        success: function (response) {

            if (response.result) {
                $('#signedbyName').html(response.signedByName);
                $('#home_page_signup_form').hide();
                $('#form-completed').show();
            }
            else {
                $('#homepage-formError').html(response.error);
            }
        }
    });

    // Remove the submitted form
    $('#home_page_signup_form').remove();

    return false;

}

