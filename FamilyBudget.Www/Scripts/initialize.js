var AddAntiForgeryToken = function(data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

function updateSummaField(summaTitleField, accountSelect, summaInputField) {
    var currency = $('option:selected', accountSelect).attr('data-currency');
    if (currency) {
        $(summaTitleField).html(currency);
        $(summaInputField).removeAttr('disabled');
    } else {
        $(summaInputField).attr('disabled', 'disabled');
        $(summaTitleField).html('');
    }
}

function initializeDateTimePicker(selector, dateToSet) {
    if (!dateToSet) {
        dateToSet = new Date();
    }

    $(selector).datetimepicker({
        language: 'ru',
        pickTime: false
    });

    $(selector).data("DateTimePicker").setDate(Globalize.format(dateToSet));
}

function setDate(selector, dateToSet) {
    initializeDateTimePicker(selector, dateToSet);
    return false;
}

function InitGlobalize() {

    // Clone original methods we want to call into
    var originalMethods = {
        min: $.validator.methods.min,
        max: $.validator.methods.max,
        range: $.validator.methods.range
    };

    // Tell the validator that we want numbers parsed using Globalize

    $.validator.methods.number = function(value, element) {
        var val = Globalize.parseFloat(value);
        return this.optional(element) || ($.isNumeric(val));
    };

    // Tell the validator that we want dates parsed using Globalize

    $.validator.methods.date = function(value, element) {
        var val = Globalize.parseDate(value);
        return this.optional(element) || (val);
    };

    // Tell the validator that we want numbers parsed using Globalize,
    // then call into original implementation with parsed value

    $.validator.methods.min = function(value, element, param) {
        var val = Globalize.parseFloat(value);
        return originalMethods.min.call(this, val, element, param);
    };

    $.validator.methods.max = function(value, element, param) {
        var val = Globalize.parseFloat(value);
        return originalMethods.max.call(this, val, element, param);
    };

    $.validator.methods.range = function(value, element, param) {
        var val = Globalize.parseFloat(value);
        return originalMethods.range.call(this, val, element, param);
    };

}

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
$(function() {

    InitGlobalize();

    // Set Globalize to the current culture driven by the html lang property
    var currentCulture = $("html").prop("lang");
    if (currentCulture) {
        Globalize.culture(currentCulture);
    }

    $('#side-menu').metisMenu();

    $(window).bind("load resize", function() {
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.sidebar-collapse').addClass('collapse');
        } else {
            $('div.sidebar-collapse').removeClass('collapse');
        }
    });
});