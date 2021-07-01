// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(document).ready(function () {
    SortProduct(); 
});

// ---Product Page js----//

function SortProduct() {
    $(".product-sort").on("change", function () {
        document.cookie = "SortProductId=" + $(this).val() + ";path=/;";
        var url = location.href;
        url = url.replace(new RegExp("/p([0-9]+)"), "");//bo phan trang di ve trang 1
        location.href = url;
        location.reload();
    });

    var sortCookie = readCookie("SortProductId");
    $(".product-sort option[value=" + sortCookie + "]").attr('selected', 'selected');
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

var boxSearchInHeader = {
    Init: function () {
        boxSearchInHeader.PostBoxSearchProduct();
    },
    PostBoxSearchProduct: function () {
        var categoryUrlName = $("#categoryId option:selected").data("url");
        if (location.href.indexOf("text-" + $("#textSearch").val() + "/p") !== -1) {
            location.href = "/product-search/" + categoryName + "/text-" + $("#textSearch").val();
        }
        $.ajax({
            type: "POST",
            url: "/Products/ProductSearch",
            data: {
                "categoryId": $("#categoryId").val(),
                "textSearch": $("#textSearch").val()
            },
            success: function () {
                if ($("#textSearch").val() !== "") {
                    location.href = "/product-search/" + categoryUrlName + "/text-" + $("#textSearch").val();
                }
                else {
                    location.href = "/product-search/" + categoryUrlName;
                }
                return;
            }
        });
    },
    GetCategoryNameFromId: function (categoryId) {
        
        var categoryName = "";
        switch (categoryId) {
            case 0:
                categoryName = "tat-ca";
                break;
            case 1:
                categoryName = "nha-cua";
                break;
            case 2:
                categoryName = "nha-hang-khach-san";
                break;
            case 3:
                categoryName = "do-choi-tre-em";
                break;
            default:
                categoryName = "tat-ca";
                break;
        }
        return categoryName;
    }
};

var productSearch = {
    SearchByPrice: function () {
        if (location.href.indexOf("/product-for-sale") !== -1) {
            location.href = location.href.replace("/product-for-sale", "/product-search/tat-ca") + "/price-" + $("#priceMin").val() + "-" + $("#priceMax").val();
        }
        if (location.href.indexOf("/strategy-") !== -1) {
            location.href = "/product-search/tat-ca" + "/price-" + $("#priceMin").val() + "-" + $("#priceMax").val();
        }
        $.ajax({
            type: "Post",
            url: "/Products/BoxFilterByPrice",
            data: {
                "PriceMin": $("#priceMin").val(),
                "PriceMax": $("#priceMax").val(),
                "CategoryId": $("#boxPrice_categoryId").val(),
                "TextSearch": $("#boxPrice_textSearch").val()
            },
            success: function () {
                location.href = location.href + "/price-" + $("#priceMin").val() + "-" + $("#priceMax").val(); 
            }
        });
    }
};

var shoppingCart = {
    ChangeQuantityOfProduct: function (productId) {
        var quantity = $("#quantity-" + productId).val();
        $.ajax({
            type: "POST",
            url: "CartItems/ChangeQuantity",
            data: {
                "productId": productId,
                "quantity": quantity
            },
            success: function (result) {
                location.reload();
            }
        });
    },
    AddToCart: function (productId) {
        var quantity = $("#quantityChange").val() <= 0 ? 1 : $("#quantityChange").val();
        $.ajax({
            type: "POST",
            url: "/CartItems/AddToCart",
            data: {
                "id": productId,
                "quantity": quantity
            },
            success: function (result) {
                location.reload();
            }
        });
    },
    DeleteCartItem: function (productId) {
        $.ajax({
            type: "POST",
            url: "CartItems/RemoveCartHeaderItem",
            data: {
                "id": productId
            },
            success: function (result) {
                location.reload();
            }
        });
    }
};

var articleSearch = {
    Init: function () {

    },
    SearchArticleByText: function () {
        if (location.href.indexOf("article-search") !== -1 || location.href.indexOf("article-blog-list/p") !== -1) {
            location.href = "/article-search/" + $("#textSearchArticle").val();
        }
        $.ajax({
            type: "POST",
            url: "ArticleBlogs/ArticleBlogSearch",
            data: {
                "textSearch": $("#textSearchArticle").val()
            }, 
            success: function () {
                if ($("#textSearchArticle").val() !== "" && $("#textSearchArticle").val() !== null) {
                    location.href = "/article-search/" + $("#textSearchArticle").val();
                    return;
                }
            }
        });
    }
};

// ---end Product Page js----//