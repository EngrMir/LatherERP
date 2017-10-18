

function checkEmail(email) {
    var reg = /^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;
    if (reg.test(email)) {
        return true;
    }
    else {
        return false;
    }
}

function commonTextboxValidation(textboxClass, fieldName) {
    $("." + textboxClass).each(function () {
        if ($(this).val() == "") {
            showMessage(fieldName + ' ' + 'is required.', 'warning', 'Warning!');
            return false;
        } else {
            return true;
        }
    });
}

function showMessage(message, priority, title) {
    $('#message').html("");
    $(document).trigger("add-alerts", [
        {
            'message': message,
            'priority': priority,
            'title': title
        }
    ]);
}