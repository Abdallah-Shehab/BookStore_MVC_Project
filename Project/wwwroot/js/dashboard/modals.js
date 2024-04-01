function BookInfo(bookId) {
    $.ajax({
        url: '/Dashboard/GetBookDeatils/'+bookId,
        type: 'GET',
        dataType: 'json',
        success: function (obj) {
                '<div><span>rate:</span> <span>' + obj.bookobj.rate + '</span></div>' 

            document.getElementById("bookId").textContent = obj.bookobj.id;
            document.getElementById("bookTitle").textContent = obj.bookobj.name; 
            document.getElementById("bookPrice").textContent = obj.bookobj.price; 
            document.getElementById("bookDesc").textContent = obj.bookobj.description;
            if (obj.bookobj.quantity == 0) {
                document.getElementById("bookQuant").textContent = obj.bookobj.quantity + " (Out Of Stock)";
            }
            else {
                document.getElementById("bookQuant").textContent = obj.bookobj.quantity;
            }
            document.getElementById("bookCategory").textContent = obj.category; 
            document.getElementById("bookAuthor").textContent = obj.author; 
            document.getElementById("bookDiscount").textContent = obj.discount; 
            document.getElementById("bookAdmin").textContent = obj.admin; 
            //document.getElementsByClassName("photo-main")[0].innerHTML = '<img src="~/assets/images/books/' + obj.bookobj.image + '">';
        },
        error: function (xhr, status, error) {
            // Handle error
            console.log("noooooo")

        }
    });

}