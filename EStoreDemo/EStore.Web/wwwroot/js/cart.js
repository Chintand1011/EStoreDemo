$(document).ready(function () {
    // Fetch cart data from localStorage
    var cartData = JSON.parse(localStorage.getItem('cart')) || [];
    $("#cartItemCount").text(cartData.length);
    // Initialize DataTable
    var table = $('#cartDataTable').DataTable({
        data: cartData,
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        "bLengthChange": false,
        columns: [
            { data: null },
            {
                data: null, className: 'td-name', render: function (data, type, row) {
                    return `${row.name}<br><small>${row.description}</small>`;
                }
            },
            {
                data: null, className: 'td-number text-right', render: function (data, type, row) {
                    return `<small>$</small>${row.price}`
                }
            },
            {
                data: 'quantity', className: 'td-number', render: function (data, type, row) {
                    return `<div class="input-group input-group-sm">
                            <input type="text" class="form-control text-center quantity mx-1" style="max-width: 50px;" value="${data}" readonly>
                            <div class="btn-group btn-group-sm">
                                <button class="btn btn-round btn-dark" onclick="updateQuantity(${row.id}, -1)"> <i class="fa fa-minus"></i> </button>
                                <button class="btn btn-round btn-dark" onclick="updateQuantity(${row.id}, 1)"> <i class="fa fa-plus"></i> </button>
                            </div>`;
                }
            },
            {
                data: null, className: 'td-number', render: function (data, type, row) {
                    return `<small>$</small> ${(row.price * row.quantity).toFixed(2)}`;
                }
            },
            {
                data: null, className: 'td-actions', sorting: false, render: function (data, type, row) {
                    return `<button type="button" rel="tooltip" data-placement="left" title="" class="btn btn-danger" onclick="removeItem(${row.id})">
                             <i class="fa fa-times"></i>
                            </button>`;
                }
            }
        ],
        drawCallback: function (settings) {
            updateTotal();
        },
        rowCallback: function (row, data, index) {
            $('td:eq(0)', row).html(index + 1); // Adding serial number
        }
    });

    // Update localStorage function
    function updateLocalStorage(cartData) {
        localStorage.setItem('cart', JSON.stringify(cartData));
    }

    // Update quantity function
    window.updateQuantity = function (productId, change) {
        var rowIdx = table.rows().eq(0).filter(function (rowIdx) {
            return table.row(rowIdx).data().id === productId;
        });
        if (rowIdx.length) {
            var rowData = table.row(rowIdx).data();
            var newQuantity = rowData.quantity + change;
            if (newQuantity > 0 && newQuantity <= rowData.stock) {
                rowData.quantity = newQuantity;
                table.row(rowIdx).data(rowData).draw();
                updateTotal();
                cartData[rowIdx[0]] = rowData;
                updateLocalStorage(cartData);
            } else if (newQuantity == 0) {
                removeItem(rowData.id);
            }
        }
    }

    // Remove item function
    window.removeItem = function (productId) {
        var rowIdx = table.rows().eq(0).filter(function (rowIdx) {
            return table.row(rowIdx).data().id === productId;
        });
        if (rowIdx.length) {
            var rowData = table.row(rowIdx).data();
            table.row(rowIdx).remove().draw();
            cartData = cartData.filter(item => item.id !== productId);
            updateTotal();
            updateLocalStorage(cartData);
            $("#cartItemCount").text(cartData.length);
        }
    }

    // Update total function
    function updateTotal() {
        var total = 0;
        $('#cartDataTable').DataTable().rows().every(function (rowIdx, tableLoop, rowLoop) {
            var data = this.data();
            total += data.price * data.quantity; // Ensure field names match your data
        });
        $('#total-price').text(total.toLocaleString('en-US', { style: 'currency', currency: 'USD' }));
    }

    window.PlaceOrder = function () {
        var cartData = JSON.parse(localStorage.getItem('cart')) || [];
        if (cartData.length > 0) {
            var orderItems = [];
            $(cartData).each(function (rowData, data) {
                var item = {
                    ProductId: data.id,
                    Quantity: data.quantity
                }
                orderItems.push(item);
            });

            model = {
                OrderItems: orderItems
            };
			$(".loader").show();
            $.ajax({
                url: '/Order/CreateOrder', // Replace with your API URL
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(model),
                success: function (data) {
					$(".loader").hide();
                    if (data.isSuccess) {
                        toastr.success(data.message, "Success");
                        localStorage.removeItem('cart');
                        window.location = "/Order";

                    }
                    else {
                        toastr.error(data.message, "Error");
                    }
                }, error: function (xhr, res, status) {
					$(".loader").hide();
                    if (xhr.status == 401) {
                        window.location.href = xhr.responseJSON.redirectUrl;
                        return;
                    }
                }
            });

        } else {
            toastr.error("Cart is Empty", "Error");
        }
    }

    // Initial total calculation
    updateTotal();
});