$.validator.setDefaults({
    errorClass: "",
    validClass: "",
    highlight: function (element, errorClass, validClass) {
        //        $(element).addClass("is-invalid").removeClass("is-valid");
        $(element).addClass("is-invalid");
        $(element.form).find("[data-valmsg-for=" + element.id + "]").addClass("invalid-feedback");
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).removeClass("is-invalid");
        //        $(element).addClass("is-valid").removeClass("is-invalid");
        $(element.form).find("[data-valmsg-for=" + element.id + "]").removeClass("invalid-feedback");
    },
});