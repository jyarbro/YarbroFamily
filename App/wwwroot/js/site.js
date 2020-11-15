$('.clickable').on("click", (event) => {
    event.stopPropagation();
    event.preventDefault();

    let element = $(event.target);

    if (!element.hasClass(".clickable")) {
        element = element.closest(".clickable");
    }

    let anchor = element.find("a")[0];
    let url = anchor.getAttribute("href");

    window.location = url;
});