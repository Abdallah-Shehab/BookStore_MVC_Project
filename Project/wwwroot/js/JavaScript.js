

// for rating stars movement and hovering


var ratingsContainer;

var isAuthenticated = document.getElementById('authStatus').getAttribute('data-is-authenticated');
if (isAuthenticated) {
    document.addEventListener("DOMContentLoaded", function () {
        ratingsContainer = document.querySelector(".addRating");

        ratingsContainer.addEventListener("mousemove", function (event) {
            if (ratingsContainer.querySelector(".rateTxt").value == "0") {
                var newWidth = getRate(event);
                ratingsContainer.querySelector(".ratings-val").style.width = newWidth + "%";
            }
        });

        ratingsContainer.addEventListener("mouseleave", function () {
            if (ratingsContainer.querySelector(".rateTxt").value == "0") {
                ratingsContainer.querySelector(".ratings-val").style.width = "0%"; // Reset to default width
            }
        });


        ratingsContainer.addEventListener("click", function (event) {

            var newWidth = getRate(event);
            var newRate = Math.round(newWidth);
            // Send the new rate to the server or perform any other action
            ratingsContainer.querySelector(".ratings-val").style.width = newWidth + "%";
            document.getElementsByClassName("rateTxt")[0].value = newRate;
        });
    });

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ///// To add Comment and rate /////
    document.getElementById("newsletter-btn").addEventListener("click", function (e) {
        e.preventDefault(); //to prevent refreshing the page because of type submit

        var bookid = document.getElementById("bookId").value;
        var comment = document.getElementById("txtComment").value;
        var rate = document.getElementById("rateTxt").value;

        if (comment == "") {
            toastr.error("You must add Comment.");
        }
        else if (rate == 0) {
            toastr.error("You must select Rate.");
        }
        else {
            debugger;
            $.ajax({
                url: '/Home/addReview',
                type: 'POST',
                dataType: 'json',
                data: { bookID: bookid, comment: comment, rate: rate },
                success: function (response) {
                    // Handle success
                    console.log(response);
                    toastr.success("your Review is Added Successfully.");

                },
                error: function (xhr, status, error) {
                    // Handle error
                    console.error("error");
                    toastr.error("Something wrong, Try again!!");

                }
            });
        }
    });

}

function getRate(event) {
    var containerRect = ratingsContainer.getBoundingClientRect();
    var mouseX = event.clientX - containerRect.left;
    var containerWidth = containerRect.width;
    return (mouseX / containerWidth) * 100;
}


////////////////////////////////////////////////////////////////////////////////////////////////////
