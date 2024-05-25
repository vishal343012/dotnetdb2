$(document).ready(function () {
    $(function () {
        $('.onlyChars').keydown(function (e) {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                e.preventDefault();
            }
        });
        $('.onlyNumbers').keydown(function (e) {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 48 && key <= 57) || (key >= 96 && key <= 105))) {
                e.preventDefault();
            }
        });
        $('.required').keyup(function (e) {
            validateOnChange(this);
        });
        $('.freeText').keyup(function (e) {
            validateOnChange(this);
        });
    });
});

function IsNumber(e) {
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    specialKeys.push(9); //Tab
    specialKeys.push(46); //Delete
    specialKeys.push(36); //Home
    specialKeys.push(35); //End
    specialKeys.push(37); //Left
    specialKeys.push(39); //Right
    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    var ret = ((keyCode > 48 && keyCode <= 57) || (specialKeys.indexOf(e.keyCode) != -1 && e.charCode != e.keyCode));
    // document.getElementById("error").style.display = ret ? "none" : "inline";
    return ret;
}


function IsOnlyNumericWoSpcialChar(e) {
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    specialKeys.push(9); //Tab
    specialKeys.push(46); //Delete
    specialKeys.push(36); //Home
    specialKeys.push(35); //End
    specialKeys.push(37); //Left
    specialKeys.push(39); //Right
    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    var ret = ((keyCode >= 48 && keyCode <= 57) || (specialKeys.indexOf(e.keyCode) != -1 && e.charCode != e.keyCode));
    // document.getElementById("error").style.display = ret ? "none" : "inline";
    return ret;
}

function validateEmail(emailField) {
    var emailReg = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!emailReg.test(emailField.trim())) {
        return false;
    }
    return true;
}

function validateOnChange(source) {
    if ($(source).val() == '') {
        $(source).parents('div .input-group').addClass('has-error form-group-focus');
    } else {
        $(source).parents('div .input-group').removeClass('has-error form-group-focus');
    }
}

function RequiredFields(ControlToValidate) {
    var requiredErrorCount = 0;
    $('#' + ControlToValidate + ' .required').each(function () {
        if ($(this).val() == '') {
            $(this).parents('div .input-group').addClass('has-error form-group-focus');
            if (requiredErrorCount == 0) requiredErrorCount = 1;
        } else {
            $(this).parents('div .input-group').removeClass('has-error form-group-focus');
        }
    });
    return requiredErrorCount;
}

function EmailFields(ControlToValidate) {
    var emailErrorCount = 0;
    $('#' + ControlToValidate + ' .requiredEmail').each(function () {
        if ($(this).val() == '') {
            $(this).parents('div .input-group').addClass('has-error form-group-focus');
            if (emailErrorCount == 0) emailErrorCount = 1;
        } else if (validateEmail($(this).val()) == false) {
            $(this).parents('div .input-group').addClass('has-error form-group-focus');
            if (emailErrorCount == 0) emailErrorCount = 1;
        } else {
            $(this).parents('div .input-group').removeClass('has-error form-group-focus');
        }
    });
    return emailErrorCount;
}

function DropDownFields(ControlToValidate) {

    var selectErrorCount = 0;
    $('#' + ControlToValidate + ' .requiredSelect').each(function () {
        if ($(this).val() == 0) {
            $(this).parents('div .form-group').addClass('has-error form-group-focus');
            if (selectErrorCount == 0) selectErrorCount = 1;
        } else {
            $(this).parents('div .form-group').removeClass('has-error form-group-focus');
        }
    });
    return selectErrorCount;
}

function TermsValidations() {
    var termCount = 0;
    $('.popup-text').each(function () {
        if ($(this).parents('div iCheck-helper').hasClass('checked')) {
            termCount = 1;
        }
    });
    return termCount;
}

function validateControls(ControlToValidate) {
    var errorMsg = "";
    var errorEmailMsg = "";
    var requiredErrorCount = 0,
        emailErrorCount = 0,
        dropDownErrorCount = 0,
        termCount = 0;
    requiredErrorCount = RequiredFields(ControlToValidate);
    emailErrorCount = EmailFields(ControlToValidate);
    dropDownErrorCount = DropDownFields(ControlToValidate);
    if (requiredErrorCount == 1) errorMsg = "Please Enter Required Fields.";
    else if (emailErrorCount == 1) errorMsg = "Please Enter Valid Email Id.";
    else if (termCount == 1) errorMsg = "Please accept terms and conditions.";
    else if (dropDownErrorCount == 1) errorMsg = "Please select valid data.";
    return errorMsg;
}

function alertModel(message) {
    alert(message);
}



function erroralertModel(message) {
    alert(message);

}