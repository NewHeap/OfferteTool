// Write your JavaScript code.
function Dropdown(e) {
    if ($(e).hasClass("active")) {
        $(e).parent().children("ul").fadeOut('fast');
        $(e).css("background-color", "white");
        $(e).removeClass("active");
        $(e).css("font-weight", "normal");
        $(e).children("span").removeClass("glyphicon-menu-down");
        $(e).children("span").addClass("glyphicon-menu-right");
    } else {
        console.log(e);
        $(".Sub-Drop").removeClass("active");
        $(e).addClass("active");
        $(e).parent().children("ul").fadeIn('slow');
        $(e).css("background-color", "#EEEEEE");
        $(e).css("font-weight", "bold");
        $(e).children("span").removeClass("glyphicon-menu-right");
        $(e).children("span").addClass("glyphicon-menu-down");
    }
}