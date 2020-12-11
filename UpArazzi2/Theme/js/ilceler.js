$(function () {
    $(".iller").change(function () {
     
        var id = $(this).val();
        var Ekle = true          
        $.ajax({
            type: "get",
            data: { IlKeyId: id, Ekle: Ekle },
            url: "/Home/Ilceler",
            success: function (data) {
           
                $(".ilceler").html(data);
            }
        })
    })
})