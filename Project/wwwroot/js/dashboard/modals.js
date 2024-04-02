function BookInfo(bookId) {
    $.ajax({
        url: '/Dashboard/GetBookDeatils/' + bookId,
        type: 'GET',
        dataType: 'json',
        success: function (obj) {
            var imagePath = '/assets/images/books/' + obj.bookobj.image;
            document.getElementsByClassName("photo-main")[0].innerHTML = '<img src="' + imagePath + '">';

            document.getElementById("bookId").textContent = obj.bookobj.id;
            document.getElementById("bookRate").textContent = obj.bookobj.rate;
            document.getElementById("bookTitle").textContent = obj.bookobj.name;
            document.getElementById("bookPrice").textContent = obj.bookobj.price;
            document.getElementById("bookDesc").textContent = obj.bookobj.description;
            if (obj.bookobj.quantity == 0) {
                document.getElementById("bookQuant").style.color = "red";
                document.getElementById("bookQuant").textContent = obj.bookobj.quantity + " (Out Of Stock)";
            }
            else {
                document.getElementById("bookQuant").textContent = obj.bookobj.quantity;
                document.getElementById("bookQuant").style.color = "#757575";
            }
            document.getElementById("bookCategory").textContent = obj.category;
            document.getElementById("bookAuthor").textContent = obj.author;
            document.getElementById("bookDiscount").textContent = obj.discount;
            document.getElementById("bookAdmin").textContent = obj.admin;
        },
        error: function (xhr, status, error) {
            // Handle error
            console.log("noooooo")

        }
    });

}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementsByClassName("btnShowComments")[0].addEventListener("click", function () {
        var bookid = parseInt(document.getElementById("bookId").innerHTML);
        redirectToBookComments(bookid);
    });
    document.getElementsByClassName("btnDeleteBook")[0].addEventListener("click", function () {
        var bookid = parseInt(document.getElementById("bookId").innerHTML);
        redirectToDeleteBook(bookid);
    });
});



function redirectToBookComments(id) {
    // Construct the URL for the action
    let url = `/Dashboard/BookComments/${id}`;

    // Redirect the user to the URL
    window.location.href = url;
}
function redirectToDeleteBook(id) {
    Swal.fire({
        title: "Are You Sure to delete it?",
        showDenyButton: true,
        showCancelButton: true,
        showConfirmButton: false,
        denyButtonText: `Yes, delete it`
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isDenied) {
            Swal.fire("The Book Deleted Successfully", "", "OK");


            //    // Construct the URL for the action
            //    let url = `/Dashboard/BookComments/${id}`;

            //    // Redirect the user to the URL
            //    window.location.href = url;
        }
    });
}