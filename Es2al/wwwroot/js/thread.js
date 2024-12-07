$(function () {
    var $element = $("#question_" + focusQuestionId);

    if ($element.length) {
        $element[0].scrollIntoView({ behavior: "smooth", block: "center" });

        var $header = $element.find(".accordion-header");
        if ($header.length) {
            $header.addClass("focus-highlight");

            setTimeout(function () {
                $header.removeClass("focus-highlight");
            }, 3000);
        }
    }
});