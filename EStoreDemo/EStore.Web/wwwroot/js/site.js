window.logout = function () {
    localStorage.removeItem("cart");
	$(".loader").show();
    $.ajax({
        url: '/User/Logout', // Replace with your API URL
        method: 'Get',
        success: function (data) {
			$(".loader").hide();
            toastr.success(data, "Success");
            window.location = "/";
        },
        error: function (xhr, status, error) {
			$(".loader").hide();
            toastr.error(error, "Error");
        }
    });
}