const questionsScrollState = {
    pageIndex: 1,
    totalPages: 1,
    isLoading: false
};
$(function () {
    questionsScrollState.totalPages = $('#pages-items').children().last().data('total-pages');
    $('#question-filter').on('submit', function () {
        $(this).find('input').each(function () {
            var input = $(this);
            if (!input.val()) {
                input.remove();
            }
        });
    });
    handleQuestionsScroll();
    //if there is multiple scroll is done continous it prevent from executing

    var scrollTimeout;
    $(window).on('scroll', function () {
        if (scrollTimeout) {
            //if this event happen again and there is was previous event watting for 100ms to be executed it will stop this event 
            clearTimeout(scrollTimeout);//this will prevent of multiple continous call of function within 100millesecond
        }
        scrollTimeout = setTimeout(() => { handleQuestionsScroll(); }, 100);//debouncing function called after a certial amount of time 

    });

});
function handleQuestionsScroll() {

    const scrollPosition = $(window).scrollTop() + $(window).height();
    const triggerPosition = $(document).height() - 35;
    if (scrollPosition >= triggerPosition
        && questionsScrollState.pageIndex < questionsScrollState.totalPages
        && !questionsScrollState.isLoading
    ) {

        fetchMoreQuestions();
    }
}
function fetchMoreQuestions() {
    questionsScrollState.isLoading = true;
    $.ajax({
        url: globalUrl,
        method: 'GET',
        dataType: "html",
        data: {
            pageIndex: (questionsScrollState.pageIndex + 1)
        },
        success: function (response) {
            $("#pages-items").append(response);
            questionsScrollState.pageIndex++;
            questionsScrollState.totalPages = $(response).data('total-pages');
        },
        error: function () {
            alert("Error loading more questions.");
        },
        complete: function () {
            questionsScrollState.isLoading = false;
            handleQuestionsScroll();//extra check 
        }
    });
}
