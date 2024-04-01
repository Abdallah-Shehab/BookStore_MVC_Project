function BookInfo(bookId) {
    $.ajax({
        url: '/Dashboard/GetBookDeatils/'+bookId,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            var block = '<div><span>id:</span> <span>' + response.ID + '</span></div>' +
                '<div><span>image:</span> <span>' + response.Image + '</span></div>' +
                '<div><span>title:</span> <span>' + response.Name + '</span></div> ' +
                '<div><span>ddesciption:</span> <span>' + response.Description + '</span></div> ' +
                '<div><span>rate:</span> <span>' + response.Rate + '</span></div>' +
                '<div><span>price:</span> <span>' + response.Price + '</span></div> ' +
                //'<div><span>discount:</span> <span>' + response.Discount.Percantage + '</span></div> ' +
                '<div><span>quantity:</span> <span>' + response.Quantity + '</span></div> ' +
                '<div><span>author:</span> <span>' + response.Author.Name + '</span></div> ' +
                '<div><span>category:</span> <span>' + response.Category.Name + '</span></div> ' +
                '<div<span>>added by:</span> <span>' + response.Admin.FirstName + '</span> </div>';

            var descDiv = document.createElement("div");
            document.getElementsByClassName("bookInfo")[0].innerHTML = block;
        },
        error: function (xhr, status, error) {
            // Handle error
            console.log("noooooo")

        }
    });

}