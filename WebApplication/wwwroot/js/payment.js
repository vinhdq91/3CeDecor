$(document).ready(function () {
    // Init Chosen
    $(".chosen-select:not(.disable_search)").chosen();

    $('input[name="PaymentMethod"]').off('click').on('click', function (e) {
        if ($(this).val() === 'NL') {
            $('#nganluongContent').show();
            $('#nganhangcontent').hide();
        }
        else if ($(this).val() === 'ATM_ONLINE') {
            $('#nganluongContent').hide();
            $('#nganhangcontent').show();
        }
        else {
            $('#nganluongContent').hide();
            $('#nganhangcontent').hide();
        }
    });

    // Bắt sự kiện nhập mã khuyến mại
    $("#inputDisCode").on("change keydown paste input", function () {
        $("#discount-mess").css("display", "none");
        $("#discount-check").css("display", "none");

        // Update lại totalprice về ban đầu
        $("#totalPrice").text($("#originPrice").val());
    });
});

var selectAction = {
    GenerateDistrict: function (provinceCode) {
        var jsonDistrict = null;
        if ($("body").data('GetListDistrict_' + provinceCode) !== null) {
            jsonDistrict = $("body").data('GetListDistrict_' + provinceCode);
        }
        if (jsonDistrict === undefined) {
            $.ajax({
                type: "Post",
                url: "check-out/gen-district",
                data: {
                    "provinceCode": provinceCode
                },
                success: function (dataDistrict) {
                    jsonDistrict = dataDistrict;
                    $("body").data('GetListDistrict_' + provinceCode, jsonDistrict);
                },
                complete: function () {
                    selectAction.BindingDistrict(0, jsonDistrict);
                },
                error: function (request, status, error) {
                    //alert(request.responseText);            
                }
            });
        }
        else {
            selectAction.BindingDistrict(0, jsonDistrict);
        }
    },
    BindingDistrict: function (districtCode, jsonDistrict) {
        $("#cboDistrict").find("option").remove();
        var strDistrict = $('#cboDistrict').html();
        if (jsonDistrict !== undefined)
            $.each(jsonDistrict, function (index, districtItem) {
                if (districtCode === districtItem.DistrictCode) {
                    strDistrict += '<option value="' + districtItem.DistrictCode + '" selected="selected">' + districtItem.Name + '</option>';
                }
                else {
                    strDistrict += '<option value="' + districtItem.DistrictCode + '">' + districtItem.Name + '</option>';
                }
            });
        $("#cboDistrict").html(strDistrict).trigger("chosen:updated");
    },
    GenerateWard: function (districtCode) {
        var jsonWard = null;
        if ($("body").data('GetListWard_' + districtCode) !== null) {
            jsonWard = $("body").data('GetListWard_' + districtCode);
        }
        if (jsonWard === undefined) {
            $.ajax({
                type: "Post",
                url: "check-out/gen-ward",
                data: {
                    "districtCode": districtCode
                },
                success: function (dataWard) {
                    jsonWard = dataWard;
                    $("body").data('GetListWard_' + districtCode, jsonWard);
                },
                complete: function () {
                    selectAction.BindingWard(0, jsonWard);
                },
                error: function (request, status, error) {
                    //alert(request.responseText);            
                }
            });
        }
        else {
            selectAction.BindingWard(0, jsonWard);
        }
    },
    BindingWard: function (wardCode, jsonWard) {
        $("#cboWard").find("option").remove();
        var strWard = $('#cboWard').html();
        if (jsonWard !== undefined)
            $.each(jsonWard, function (index, wardItem) {
                if (wardCode === wardItem.WardCode) {
                    strWard += '<option value="' + wardItem.WardCode + '" selected="selected">' + wardItem.Name + '</option>';
                }
                else {
                    strWard += '<option value="' + wardItem.WardCode + '">' + wardItem.Name + '</option>';
                }
            });
        $("#cboWard").html(strWard).trigger("chosen:updated");
    }
};

var discountCode = {
    checkDiscountCode: function () {
        var disCode = $("#inputDisCode").val();
        if (disCode === "") {
            $("#discount-mess").css("display", "inline-block");
            $("#discount-mess").text("Vui lòng nhập mã khuyến mại tại đây.");
            return false;
        }
        $.ajax({
            type: "Post",
            url: "check-out/checkDiscountCode",
            data: {
                "discountCode": disCode
            },
            success: function (res) {
                if (res.Error === true) {
                    $("#discount-mess").css("display", "inline-block");
                    $("#discount-mess").text(res.Title);
                    $("#discount-check").css("display", "none");
                }
                else {
                    $("#discount-check").css("display", "inline-block");
                    $("#discount-mess").css("display", "none");

                    discountCode.UpdateTotalPrice(res.Obj.DiscountRate);
                }
            }
        });
    },
    UpdateTotalPrice: function (discountRate) {
        var strPrice = $("#totalPrice").text().replace(".", "").replace("VND", "").trim();
        var totalPrice = parseFloat(strPrice);
        if (totalPrice > 0) {
            totalPrice = totalPrice - (totalPrice * parseInt(discountRate) / 100);
            $("#totalPrice").text(totalPrice).formatCurrency({ roundToDecimalPlace: -1, symbol: "" });
            $("#totalPrice").text($("#totalPrice").text().replace(",", ".") + " VND");
        }
    }
};