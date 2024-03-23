

function addToCart(id, text) {
    let quantity;
    if (text == 'whithout quantity') {
        quantity = 1;
    }
    else {
        quantity = document.getElementById("qty").value;
    }
    let flag = false;

    let arritems = JSON.parse(localStorage.getItem("cart") || "[]");

    if (arritems.length == 0) {
        setItem("cart", [{ "bookID": id, "quantity": quantity }]);
        toster();
        toastr.success("The Book is added to the Cart Successfully.");
    }
    else {
        console.log("else")
        let newitems = [];
        arritems.forEach(obj => {
            if (obj.bookID == id) {
                flag = true;
                toster();
                if (text == 'whithout quantity') {
                    toastr.warning("The Book is already exist");
                }
                else {
                    obj.quantity = quantity;
                    toastr.success("The Book is already exist & update the quantity.");
                }
            }
            newitems.push(obj);
            if (newitems.length == arritems.length && flag == false) {
                newitems.push({ "bookID": id, "quantity": quantity });
                toster();
                toastr.success("The Book is added to the Cart Successfully.");
            }
            
        });
        setItem("cart", newitems);
    }
}
function toster() {
    toastr.options = {
        "closeButton": true,
        "newestOnTop": false,
        "progressBar": true,
        "preventDuplicates": false,
        "onclick": null,
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
    }
}


function setItem(key, valueObj) {

    localStorage.setItem(key, JSON.stringify(valueObj));
}