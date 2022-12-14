$(document).ready(function () {
    var flag, dot_flag = false,
        prevX, prevY, currX, currY = 0,
        color = 'black', thickness = 2;
    var $canvas = $('#canvas');
    var ctx = $canvas[0].getContext('2d');

    $canvas.on('mousemove mousedown mouseup mouseout', function (e) {
        prevX = currX;
        prevY = currY;
        currX = e.clientX - $canvas.offset().left;
        currY = e.clientY - $canvas.offset().top;
        if (e.type == 'mousedown') {
            flag = true;
        }
        if (e.type == 'mouseup' || e.type == 'mouseout') {
            flag = false;
        }
        if (e.type == 'mousemove') {
            if (flag) {
                ctx.beginPath();
                ctx.moveTo(prevX, prevY);
                ctx.lineTo(currX, currY);
                ctx.strokeStyle = color;
                ctx.lineWidth = thickness;
                ctx.stroke();
                ctx.closePath();
            }
        }
    });

    $('.canvas-clear').on('click', function (e) {
        c_width = $canvas.width();
        c_height = $canvas.height();
        ctx.clearRect(0, 0, c_width, c_height);
        $('#canvasimg').hide();
    });
});