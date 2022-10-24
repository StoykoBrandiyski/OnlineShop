$('#filterButton').click(function () {
    $(".filter-items").toggleClass("active");
});

$('#form-filter-price').change(function () {
    sendData("/Product/GetFilterByPrice");
});

$('#form-filter-brand').change(function () {
    sendData("/Product/GetFilterByBrand");
});

$('#form-filter').change(function () {
    sendData("/Product/GetFilter");
});

function sendData(destination) {
    let checked = $("input[type='radio']:checked").prevObject[0].activeElement;
    const categoryId = $('#categoryId').val();

    if (!checked.checked) {
        getDefaultProducts(categoryId);
        return;
    }

    if (typeof (checked.defaultValue) == "undefined") {
        console.log('Null');
        return;
    } else {
        getFilter(checked.name, checked.defaultValue,categoryId, destination);
    }
}

function getDefaultProducts(categoryId) {
    $.ajax({
        method: "Post",
        url: "/Product/GetProductsDefault",
        data: { Id: categoryId },
        dataType: 'json',
        success: function (data) {
            console.log("default");
            console.log(data);
            renderProduct(data);
        },
    });
}

function getFilter(inputType, inputValue,category, toUrl) {
    $.ajax({
        method: "Post",
        url: toUrl,
        data: { type: inputType, value: inputValue, categoryId: category },
        dataType: 'json',
        beforeSend: function () {
            $("#container-products").appendTo("Търси");
        },
        success: function (data) {
            renderProduct(data);
        },
    });
}

const categoryId = document.getElementById('categoryId').value;

function requestSort(inputValue) {
    $.ajax({
        method: "POST",
        url: "/Product/GetFilterProductsByOrder",
        data: { typeOrder: inputValue, id: categoryId },
        dataType: 'json',
        success: function (data) {
            renderProduct(data);
        },
    });
};

document.getElementById("sort-filter").addEventListener('change', function (event) {
    console.log("cj");
    requestSort(event.target.value);
});


function renderProduct(data) {
    const container = $("#container-products");
    container.empty();

    let response = "";
    for (let product of data) {
        
        response += `<div class='card m-3' style='width: 15rem;'><img class="card-img-top" src='${product.imageUrl}'>`
            + `<div class="card-body"><a class="card-text text-decoration-none text-dark fw-bold" href="/Product/Details/${product.id}">${product.name}</a>`
            + `<p class="border border-success border-2 px-1 mt-1">В наличност</p>`
            + `<p class="text-danger fw-bold">${product.price}лв.</p><a href="#"><i class="fa fa-heart-circle-plus"></i></a></div></div>`
    }
    $(response).appendTo(container);
}