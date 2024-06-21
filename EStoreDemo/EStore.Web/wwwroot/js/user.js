$(document).ready(function () {

    // Login form validation
    $('#loginForm').submit(function (event) {
        event.preventDefault();

        var username = $('#loginUsername').val().trim();
        var password = $('#loginPassword').val();

        if (username.length > 255) {
            toastr.error("Invalid Username", "Error");
            return;
        }

        if (password.length < 6) {
            toastr.error("Password must be at least 6 characters long", "Error");
            return;
        }
        UserLogin();
    });

    // Sign up form validation
    $('#signupForm').submit(function (event) {
        event.preventDefault();

        var username = $('#signupUsername').val().trim();
        var email = $('#signupEmail').val().trim();
        var password = $('#signupPassword').val();
        var confirmPassword = $('#signupConfirmPassword').val();

        if (username.length > 255) {
            toastr.error("Username length cannot exceed 255 characters.", "Error");
            return;
        }

        if (email.length > 255) {
            toastr.error("Email length cannot exceed 255 characters.", "Error");
            return;
        }

        if (password.length < 6) {
            toastr.error("Password must be at least 6 characters long.", "Error");
            return;
        }

        if (password !== confirmPassword) {
            toastr.error("Password and Confirm Password must match.", "Error");
            return;
        }
        UserRegister();
    });

});
function toggleForm() {
    var loginCard = $("#loginCard");
    var signupCard = $("#signupCard");
    if (loginCard.hasClass("d-none")) {
        loginCard.removeClass("d-none");
        signupCard.addClass("d-none");
    } else {
        signupCard.removeClass("d-none");
        loginCard.addClass("d-none");
    }
    clearForm();
}

function UserLogin() {
    var data = {
        Username: $("#loginUsername").val().trim(),
        Password: $("#loginPassword").val()
    }
	$(".loader").show();
    $.ajax({
        type: "POST",
        url: "/User/Login",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (result) {
            localStorage.removeItem('cart');
			$(".loader").hide();
            if (result.isSuccess) {
                toastr.success(result.message, "Success");
                window.location.href = "/Product";
            } else {
                toastr.error(result.message, "Error");
            }
        },
        error: function (result) {
			$(".loader").hide();
        }
    });
}
function UserRegister() {
    var data = {
        Username: $('#signupUsername').val().trim(),
        Email: $('#signupEmail').val().trim(),
        Password: $('#signupPassword').val()
    }
	$(".loader").show();
    $.ajax({
        type: "POST",
        url: "/User/Register",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (result) {
			$(".loader").hide();
            if (result.isSuccess) {
                toastr.success(result.message, "Success");
				clearForm();
                toggleForm();
            } else {
                toastr.error(result.message, "Error");
            }

        },
        error: function (result) {
			$(".loader").hide();
        }
    });
}
function clearForm() {
    $("#loginPassword").val("");
    $("#loginUsername").val("");
    $('#signupUsername').val("");
    $('#signupEmail').val("");
    $('#signupPassword').val("");
    $('#signupConfirmPassword').val("");
}