

//let home = document.querySelector("#home");
//home.classList.remove("active");

let l = window.location.pathname;
let navLinks = document.querySelectorAll(".nav-link");
if (l.toString() == "/") {
    navLinks[4].classList.add("active");
}
else if (l.toString() == "/Product/GetAllProducts") {
    navLinks[5].classList.add("active");
}
else if (l.toString() == "/ShoppingCart") {
    navLinks[6].classList.add("active");
}
else if (l.toString() == "/Checkout") {
    navLinks[7].classList.add("active");
}
else if (l.toString() == "/Order/GetAllOrders") {
    navLinks[8].classList.add("active");
}
else if (l.toString() == "/Contact") {
    navLinks[9].classList.add("active");
}

if (l.toString() == "/Account/Login") {
    navLinks[6].classList.add("active");
}
else if (l.toString() == "/Account/Register") {
    navLinks[7].classList.add("active");
}
console.log(navLinks);


//function RemoveActive() {
//    navLinks.forEach(function (item) {
//        navLinks.forEach(function (item) {
//            item.classList.remove("active");
//        })
//    })
//}