$(document).ready(function () {
    InitArrow();

    // Init clock
    GetClock();
    setInterval(GetClock, 1000);
});

// Collapse arrow
function InitArrow() {
    $('#nav-top').on('shown.bs.collapse', function () {
        $("#top-collapse.glyphicon").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
    });

    $('#nav-top').on('hidden.bs.collapse', function () {
        $("#top-collapse.glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
    });

    $('#top-alarm').on('shown.bs.collapse', function () {
        $("#alarm-collapse.glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
    });

    $('#top-alarm').on('hidden.bs.collapse', function () {
        $("#alarm-collapse.glyphicon").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
    });

    $('#top-event').on('shown.bs.collapse', function () {
        $("#event-collapse.glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
    });

    $('#top-event').on('hidden.bs.collapse', function () {
        $("#event-collapse.glyphicon").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
    });

    $(".arrow-up").hide();
    $(".option-heading").click(function () {
        $(this).find(".arrow-up, .arrow-down").toggle();
    });
}

function GetClock() {
    var d = new Date();
    var nmonth = d.getMonth(), ndate = d.getDate(), nyear = d.getYear();
    if (nyear < 1000) nyear += 1900;
    var nhour = d.getHours(), nmin = d.getMinutes(), nsec = d.getSeconds(), ap;
   
    if (nmin <= 9) nmin = "0" + nmin;
    if (nsec <= 9) nsec = "0" + nsec;

    //document.getElementById('clock-box').innerHTML = "" + ndate + "/" + (nmonth + 1) + "/" + nyear + " " + nhour + ":" + nmin + ":" + nsec;
}

