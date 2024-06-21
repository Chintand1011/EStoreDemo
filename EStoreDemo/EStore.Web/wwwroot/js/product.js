$(document).ready(function () {
    var isAdmin = $("#isAdmin").val();
    var cart = JSON.parse(localStorage.getItem('cart')) || [];
    $("#cartItemCount").text(cart.length);
    var table = $('#productDataTable').DataTable({
        "bPaginate": true,
        "bLengthChange": false,
        "bInfo": isAdmin.toString() == "1" ? true : false,
        columns: [
            { data: 'name', className:'text-center' },
            { data: 'description' },
            {
                data: 'price', className: 'td-number', render: function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                }
            },
            { data: 'stock',className: 'td-number', visible: isAdmin.toString() == "1" ? true : false },
            {
                data: null, className:'td-actions td-right', sorting:false, render: function (data, type, row) {
                    var button = isAdmin.toString() == "1" ? '<button type="button" rel="tooltip" class="btn btn-success btn-just-icon btn-sm edit-product"><i class="fa fa-edit"></i></button> | <button type="button" rel="tooltip" class="btn btn-danger btn-just-icon btn-sm delete-product"><i class="fa fa-trash"></i></button>' : '<button class="btn btn-dark btn-just-icon btn-sm add-to-cart"><i class="fa fa-shopping-cart"></i></button>';
                    return button;
                }
            }
        ]
    });

    // Fetch data from API and populate the table
    function fetchData() {
		$(".loader").show();
        $.ajax({
            url: 'Product/GetProducts', // Replace with your API URL
            method: 'GET',
            success: function (data) {
				$(".loader").hide();
                table.clear().rows.add(data.data).draw();
                updateAddToCartButtons();
            },
            error: function (xhr, res, status) {
				$(".loader").hide();
                if (xhr.status == 401) {
                    window.location.href = xhr.responseJSON.redirectUrl;
                    return;
                }
            }
        });
    }

    // Add product form submit handler
    $('#productForm').submit(function (event) {
        event.preventDefault();

        
        var name = $('#addProductName').val();
        var description = $('#addProductDescription').val();
        var price = parseFloat($('#addProductPrice').val());
        var stock = parseInt($('#addProductStock').val());
        if (name.length == 0 || name == undefined) {
            toastr.error("Please Enter Product Name", "Error");
            return;
        }
        if (description.length == 0 || description == undefined) {
            toastr.error("Please Enter Product Description", "Error");
            return;
        }
        if (price == 0 || price == undefined || isNaN(price)) {
            toastr.error("Please Enter Price", "Error");
            return;
        }
        if (stock == 0 || stock == undefined || isNaN(stock)) {
            toastr.error("Please Enter Stock", "Error");
            return;
        }


        var product = {
            Name: name,
            Description: description,
            Price: price,
            Stock: stock
        }
$(".loader").show();
        $.ajax({
            url: '/Product/CreateProduct', // Replace with your API URL
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(product),
            success: function (data) {
				$(".loader").hide();
                if (data.isSuccess) {
                    $('#addProductName').val('');
                    $('#addProductDescription').val('');
                    $('#addProductPrice').val('');
                    $('#addProductStock').val('');
                    $('#productModal').modal('hide');
                    toastr.success(data.message, "Success");
                    fetchData();
                } else {
                    toastr.error(data.message, "Error");
                }
            },
            error: function (xhr, res, status) {
				$(".loader").hide();
                if (xhr.status == 401) {
                    window.location.href = xhr.responseJSON.redirectUrl;
                    return;
                }
            }
        });

    });

    // Add product form submit handler
    $('#productEditForm').submit(function (event) {
        event.preventDefault();

        // Validate form fields
        if (!this.checkValidity()) {
            event.stopPropagation();
            return;
        }

        var id = $("#editId").val();
        var name = $('#editProductName').val();
        var description = $('#editProductDescription').val();
        var price = parseFloat($('#editProductPrice').val());
        var stock = parseInt($('#editProductStock').val());

        if (name.length == 0 || name == undefined) {
            toastr.error("Please Enter Product Name", "Error");
            return;
        }
        if (description.length == 0 || description == undefined) {
            toastr.error("Please Enter Product Description", "Error");
            return;
        }
        if (price == 0 || price == undefined || isNaN(price)) {
            toastr.error("Please Enter Price", "Error");
            return;
        }
        if (stock == 0 || stock == undefined || isNaN(stock)) {
            toastr.error("Please Enter Stock", "Error");
            return;
        }
        var product = {
            Id: id,
            Name: name,
            Description: description,
            Price: price,
            Stock: stock
        }

$(".loader").show();
        $.ajax({
            url: '/Product/EditProduct', // Replace with your API URL
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(product),
            success: function (data) {
				$(".loader").hide();
                if (data.isSuccess) {
                    $('#editId').val('');
                    $('#editProductName').val('');
                    $('#editProductDescription').val('');
                    $('#editProductPrice').val('');
                    $('#editProductStock').val('');
                    $('#editProductModal').modal('hide');
                    toastr.success(data.message, "Success");
                    fetchData();
                }
                else {
                    toastr.error(data.message, "Error");
                }
            },
            error: function (xhr, res, status) {
				$(".loader").hide();
                if (xhr.status == 401) {
                    window.location.href = xhr.responseJSON.redirectUrl;
                    return;
                }
            }
        });

    });

    function updateAddToCartButtons() {
        $('.add-to-cart').off('click').on('click', function () {
            var row = $(this).closest('tr');
            var data = table.row(row).data();
            var product = {
                name: data.name,
                description: data.description,
                price: data.price,
                quantity: 1,
                stock: data.stock,
                id: data.id
            };
            if (data.stock > 0) {
                addToCart(product);
            } else {
                toastr.error("Product is out of stock.","Error");
            }
            
        });
        $('.edit-product').off('click').on('click', function () {
            var row = $(this).closest('tr');
            var data = table.row(row).data();
            $('#editId').val(data.id);
            $('#editProductName').val(data.name);
            $('#editProductDescription').val(data.description);
            $('#editProductPrice').val(data.price);
            $('#editProductStock').val(data.stock);
            $('#editProductModal').modal('show');
        });

        $('.delete-product').off('click').on('click', function () {
            var row = $(this).closest('tr');
            var data = table.row(row).data();

            var product = {
                Id: data.id
            }
$(".loader").show();
            $.ajax({
                url: '/Product/DeleteProduct', // Replace with your API URL
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(product),
                success: function (data) {
					$(".loader").hide();
                    if (data.isSuccess) {
                        toastr.success(data.message, "Success");
                        fetchData();
                    }
                    else {
                        toastr.error(data.message, "Error");
                    }
                },
                error: function (xhr, res, status) {
					$(".loader").hide();
                    if (xhr.status == 401) {
                        window.location.href = xhr.responseJSON.redirectUrl;
                        return;
                    }
                }
            });

        });
    }

    function addToCart(product) {
        var cart = JSON.parse(localStorage.getItem('cart')) || [];
        var found = false;

        for (var i = 0; i < cart.length; i++) {
            if (cart[i].id === product.id) {
                cart[i].quantity += 1;
                if (cart[i].quantity > product.stock) {
                    toastr.error("Maximum quantity of " + product.name + " has been added to your cart!", "Error");
                    return "";
                    break;
                }
                found = true;
                break;
            }
        }

        if (!found) {
            cart.push(product);
        }

        localStorage.setItem('cart', JSON.stringify(cart));
        $("#cartItemCount").text(cart.length);
        toastr.success(product.name + " has been added to your cart!", "Success");
    }
    fetchData();
});
