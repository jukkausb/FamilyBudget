var AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

function updateSummaField(summaTitleField, accountSelect, summaInputField) {
    var currency = $('option:selected', accountSelect).attr('data-currency');
    if (currency) {
        $(summaTitleField).html('');
        var valutaImg = document.createElement("img");
        valutaImg.setAttribute('src', '/Content/flags/' + currency + '.png');
        valutaImg.setAttribute('width', '20px');

        $(summaTitleField).append(valutaImg);
        $(summaInputField).removeAttr('disabled');
    } else {
        $(summaInputField).attr('disabled', 'disabled');
        $(summaTitleField).html('');
    }
}

function initializeDateTimePicker(selector, dateToSet) {
    if (!dateToSet) {
        $(selector).datetimepicker({
            locale: 'ru',
            format: 'L'
        });
    }

    $(selector).datetimepicker({
        locale: 'ru',
        format: 'L',
        defaultDate: moment(dateToSet).format('L')
    });
}

function setDate(selector, dateToSet) {
    initializeDateTimePicker(selector, dateToSet);
    return false;
}

function isDecimalInput(txt, evt) {
    var charCode = evt.which ? evt.which : evt.keyCode;
    if (charCode === 46) {
        //Check if the text already contains the . character
        if (txt.value.indexOf('.') === -1) {
            return true;
        } else {
            return false;
        }
    } else {
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
    }
    return true;
}

$(function () {

    $.validator.methods.date = function (value, element) {
        var val = Globalize.parseDate(value);
        return this.optional(element) || val;
    };

    // Set Globalize to the current culture driven by the html lang property
    var currentCulture = $("html").prop("lang");
    if (currentCulture) {
        $.when(
            $.get("/Scripts/cldr/supplemental/likelySubtags.json"),
            $.get("/Scripts/cldr/main/" + currentCulture + "/numbers.json"),
            $.get("/Scripts/cldr/supplemental/numberingSystems.json"),
            $.get("/Scripts/cldr/main/" + currentCulture + "/ca-gregorian.json"),
            $.get("/Scripts/cldr/main/" + currentCulture + "/timeZoneNames.json"),
            $.get("/Scripts/cldr/supplemental/timeData.json"),
            $.get("/Scripts/cldr/supplemental/weekData.json")
        ).then(function () {
            // Normalize $.get results, we only need the JSON, not the request statuses.
            return [].slice.apply(arguments, [0]).map(function (result) {
                return result[0];
            });
        }).then(Globalize.load).then(function () {
            Globalize.locale(currentCulture);
        });
    }
});