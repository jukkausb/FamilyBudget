function loadWidgets(widgets) {
    for (var i = 0; i < widgets.length; i++) {
        var b = document.querySelector('#' + widgets[i].id + ' .ladda-button');
        var l = Ladda.create(b);
        var widgetContainer = $(document.getElementById(widgets[i].id)).find('.widget-content');
        var windgetContext = { widget: widgets[i], indicator: l, button: b, container: widgetContainer };
        $(widgetContainer).html('');
        $(b).show();
        l.start();
        $.ajax({
                type: "POST",
                url: widgets[i].url,
                context: windgetContext
            })
            .done(function(data) {
                this.indicator.stop();
                $(this.button).hide();
                $(this.container).append(data);
                if (this.widget.callback && typeof this.widget.callback === "function") {
                    window[this.widget.callback]();
                }
            });
    }
}