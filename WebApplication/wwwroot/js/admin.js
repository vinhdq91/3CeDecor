$(document).ready(function () {
    productAdmin.Init();
    articleBlogAdmin.Init();
    strategyAdmin.Init();
});

var productAdmin = {
    Init: function () {
        productAdmin.ChangeImage();
    },
    ChangeImage: function () {
        $(".editor-img").change(function () {
            var image = this.files[0];
            var imgElement = document.createElement("img");
            imgElement.src = window.URL.createObjectURL(image);
            imgElement.style.height = "30px";
            imgElement.onload = function () {
                window.URL.revokeObjectURL(this.src);
            };

            // Replace image preview
            var imagePreview = $(this).prev();
            $(imagePreview).empty(); // clear existing content
            $(imagePreview).append(imgElement);
        });
    },
    ChangeStatus: function (productId) {
        $.ajax({
            type: "Post",
            url: "Admin/ProductAdmin/ChangeStatus",
            data: {
                "productId": productId
            },
            success: function (res) {
                $(document.body).load(location.href);
            }
        });
    },
    CheckAllProduct: function (event) {
        var inputCheckAll = event.target;
        if ($(inputCheckAll).is(":checked")) {
            // Xử lý check toàn bộ sản phẩm
            $(".select-product-item").each(function () {
                $(this).prop("checked", true);

                var productId = parseInt($(this).val());
                productAdmin.GetProductItemSelect(productId, this);
            });

            // enable button xóa
            $('.btn-remove').removeAttr('disabled');
        }
        else {
            // Bỏ check
            $(".select-product-item").each(function () {
                $(this).prop("checked", false);
                $(".list-product-item-selected").val("");
            });

            // disable button xóa
            $('.btn-remove').attr('disabled', 'disabled');
        }
    },
    GetProductItemSelect: function (productId, event) {
        var listValue = $(".list-product-item-selected").val();
        var inputSelected = event.target !== undefined ? event.target : event;
        if ($(inputSelected).is(":checked")) {
            if (productId > 0) {
                if (listValue === "") {
                    listValue = productId + "";
                }
                else {
                    listValue += "," + productId;
                }
                // enable button xóa
                $('.btn-remove').removeAttr('disabled');
            }
        }
        else {
            if (productId > 0) {
                if (listValue.indexOf("," + productId) !== -1) {
                    listValue = listValue.replace("," + productId, "");
                }
                else {
                    listValue = listValue.replace(productId, "");
                }

                // disable button xóa
                if (listValue === "") {
                    $('.btn-remove').attr('disabled', 'disabled');
                }
            }
        }

        // Trim bỏ dấu , ở đầu cuối
        listValue = listValue.replace(new RegExp('^,|,$', 'g'), '');
        $(".list-product-item-selected").val(listValue);
    },
    RemoveAllSelectedInList: function (topicId) {
        // Hàm xoá các product đã chọn trong danh sách sản phẩm
        if ($(".list-product-item-selected").val() !== undefined) {
            var listProductIds = $(".list-product-item-selected").val().split(',');
            if (listProductIds.length > 0) {
                $.ajax({
                    type: "Post",
                    url: "Admin/ProductAdmin/Delete",
                    data: {
                        "lstProductIds": listProductIds
                    },
                    success: function (res) {
                        $(document.body).load(location.href);
                    }
                });
            }
        }
    },
    RemoveAllSelectedInTopic: function (topicId) {
        // Hàm xoá các product đã chọn trong topic
        if ($(".list-product-item-selected").val() !== undefined) {
            var listProductIds = $(".list-product-item-selected").val().split(',');
            if (listProductIds.length > 0) {
                $.ajax({
                    type: "Post",
                    url: "RemoveFromTopic",
                    data: {
                        "lstProductIds": listProductIds,
                        "topicId": topicId
                    },
                    success: function (res) {
                        $(".pop-up .modal").css("display", "none");
                        $(document.body).load(location.href);
                    }
                });
            }
        }
    }
};

var popUpProductTopic = {
    OpenClosePopup: function () {
        if ($(".pop-up .modal").css("display") === "none") {
            $(".pop-up .modal").css("display", "block");
            popUpProductTopic.LoadProductList();
        }
        else {
            $(".pop-up .modal").css("display", "none");
        }
    },
    LoadProductList: function () {
        if (!$.trim($('.modal-body').html()).length) {
            $.ajax({
                type: "Post",
                url: "GetProductListNormalType",
                success: function (res) {
                    if (res !== null && res !== undefined) {
                        $(".modal-body").append(res);

                        // Check if Datatable has initialized or not
                        if (!$.fn.DataTable.isDataTable('#myTable-popup')) {
                            $('#myTable-popup').DataTable({
                                "paging": true,
                                "lengthChange": true,
                                "searching": true,
                                "ordering": true,
                                "info": true,
                                "autoWidth": false,
                                "responsive": true,
                                "stateSave": true
                            });
                        }
                    }
                }
            });
        }
    },
    GetProductSelect: function (productId) {
        if (productId > 0) {
            var listValue = $(".list-product-selected").val();
            if (listValue === "") {
                listValue = productId + "";
            }
            else {
                listValue += "," + productId;
            }
            $(".list-product-selected").val(listValue);
        }
    },
    SaveListProductToTopic: function (topicId) {
        var listProductIds = $(".list-product-selected").val().split(',');
        if (listProductIds.length > 0) {
            $.ajax({
                type: "Post",
                url: "UpdateTopicOfProductList",
                data: {
                    "lstProductIds": listProductIds,
                    "topicId": topicId
                },
                success: function (res) {
                    $(".pop-up .modal").css("display", "none");
                    $(document.body).load(location.href);
                }
            });
        }
    }
};

var articleBlogAdmin = {
    Init: function () {
        articleBlogAdmin.ChangeImage();
    },
    ChangeImage: function () {
        $(".editor-img").change(function () {
            var image = this.files[0];
            var imgElement = document.createElement("img");
            imgElement.src = window.URL.createObjectURL(image);
            imgElement.style.height = "30px";
            imgElement.onload = function () {
                window.URL.revokeObjectURL(this.src);
            };

            // Replace image preview
            var imagePreview = $(this).prev();
            $(imagePreview).empty(); // clear existing content
            $(imagePreview).append(imgElement);
        });
    }
};

var popUpProductInStrategyEdit = {
    OpenClosePopup: function () {
        if ($(".pop-up .modal").css("display") === "none") {
            $(".pop-up .modal").css("display", "block");
            popUpProductInStrategyEdit.LoadProductList();
        }
        else {
            $(".pop-up .modal").css("display", "none");
        }
    },
    LoadProductList: function () {
        if (!$.trim($('.modal-body').html()).length) {
            $.ajax({
                type: "Post",
                url: "GetProductListForPopUpStrategy",
                data: {
                    "strategyId": $("#strategyId").val()
                },
                success: function (res) {
                    if (res !== null && res !== undefined) {
                        if ($(".modal-body").length <= 1) {
                            $(".modal-body").append(res);
                        }

                        // Check if Datatable has initialized or not
                        if (!$.fn.DataTable.isDataTable('#myTable-popup')) {
                            $('#myTable-popup').DataTable({
                                "paging": true,
                                "lengthChange": true,
                                "searching": true,
                                "ordering": true,
                                "info": true,
                                "autoWidth": false,
                                "responsive": true,
                                "stateSave": true
                            });
                        }
                    }
                }
            });
        }
    },
    GetProductSelect: function (productId) {
        if (productId > 0) {
            var listValue = $(".list-product-selected").val();
            if (listValue === "") {
                listValue = productId + "";
            }
            else {
                listValue += "," + productId;
            }
            $(".list-product-selected").val(listValue);
        }
    },
    SaveListProductToStrategy: function (strategyId) {
        var listProductIds = $(".list-product-selected").val().split(',');
        if (listProductIds.length > 0) {
            $.ajax({
                type: "Post",
                url: "AddProductListToStrategy",
                data: {
                    "lstProductIds": listProductIds,
                    "strategyId": strategyId
                },
                success: function (res) {
                    $(".pop-up .modal").css("display", "none");
                    $(document.body).load(location.href);
                }
            });
        }
    },
    RemoveProductsFromStrategy: function (strategyId) {
        // Hàm xoá các product đã chọn trong danh sách sản phẩm
        if ($(".list-product-item-selected").val() !== undefined) {
            var listProductIds = $(".list-product-item-selected").val().split(',');
            if (listProductIds.length > 0) {
                $.ajax({
                    type: "Post",
                    url: "RemoveFromStrategy",
                    data: {
                        "lstProductIds": listProductIds,
                        "strategyId": strategyId
                    },
                    success: function (res) {
                        $(document.body).load(location.href);
                    }
                });
            }
        }
    }
};

var strategyAdmin = {
    Init: function () {
        strategyAdmin.ChangeImage();
    },
    ChangeImage: function () {
        $(".editor-img").change(function () {
            var image = this.files[0];
            var imgElement = document.createElement("img");
            imgElement.src = window.URL.createObjectURL(image);
            imgElement.style.height = "30px";
            imgElement.onload = function () {
                window.URL.revokeObjectURL(this.src);
            };

            // Replace image preview
            var imagePreview = $(this).prev();
            $(imagePreview).empty(); // clear existing content
            $(imagePreview).append(imgElement);
        });
    },
    ChangeStatus: function (strategyId) {
        $.ajax({
            type: "Post",
            url: "ChangeStatus",
            data: {
                "strategyId": strategyId
            },
            success: function (res) {
                $(document.body).load(location.href);
            }
        });
    },
    CheckAllStrategy: function (event) {
        var inputCheckAll = event.target;
        if ($(inputCheckAll).is(":checked")) {
            // Xử lý check toàn bộ chiến dịch
            $(".select-strategy-item").each(function () {
                $(this).prop("checked", true);

                var strategyId = parseInt($(this).val());
                strategyAdmin.GetStrategyItemSelect(strategyId, this);
            });

            // enable button xóa
            $('.btn-remove').removeAttr('disabled');
        }
        else {
            // Bỏ check
            $(".select-strategy-item").each(function () {
                $(this).prop("checked", false);
                $(".list-strategy-item-selected").val("");
            });

            // disable button xóa
            $('.btn-remove').attr('disabled', 'disabled');
        }
    },
    GetStrategyItemSelect: function (strategyId, event) {
        var listValue = $(".list-strategy-item-selected").val();
        var inputSelected = event.target !== undefined ? event.target : event;
        if ($(inputSelected).is(":checked")) {
            if (strategyId > 0) {
                if (listValue === "") {
                    listValue = strategyId + "";
                }
                else {
                    listValue += "," + strategyId;
                }
                // enable button xóa
                $('.btn-remove').removeAttr('disabled');
            }
        }
        else {
            if (strategyId > 0) {
                if (listValue.indexOf("," + strategyId) !== -1) {
                    listValue = listValue.replace("," + strategyId, "");
                }
                else {
                    listValue = listValue.replace(strategyId, "");
                }

                // disable button xóa
                if (listValue === "") {
                    $('.btn-remove').attr('disabled', 'disabled');
                }
            }
        }

        // Trim bỏ dấu , ở đầu cuối
        listValue = listValue.replace(new RegExp('^,|,$', 'g'), '');
        $(".list-strategy-item-selected").val(listValue);
    },
    RemoveAllSelectedInList: function () {
        // Hàm xoá các product đã chọn trong danh sách sản phẩm
        if ($(".list-strategy-item-selected").val() !== undefined) {
            var lstStrategyIds = $(".list-strategy-item-selected").val().split(',');
            if (lstStrategyIds.length > 0) {
                $.ajax({
                    type: "Post",
                    url: "DeleteStrategy",
                    data: {
                        "lstStrategyIds": lstStrategyIds
                    },
                    success: function (res) {
                        $(document.body).load(location.href);
                    }
                });
            }
        }
    }
};