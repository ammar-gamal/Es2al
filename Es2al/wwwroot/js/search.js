const usersScrollState = {
    pageIndex: null,
    totalPages: null,
    isLoading: false, 
    formData: null
};
$(function () {
    bindSearchForm();
})
function bindSearchForm() {
    var form = $('#user-search');
    form.off().on('submit', function (e) {
        e.preventDefault();
        $('#pages-items').empty();
        var data = form.serializeArray();
        usersScrollState.formData = data;
        usersScrollState.pageIndex = 0;
        usersScrollState.totalPages = 1;
        usersScrollState.isLoading = false;
        handleUserListScroll();
        bindScrollEvent();
    })
}
function handleUserListScroll() {
    const scrollPosition = $(window).scrollTop() + $(window).height();
    const triggerPosition = $(document).height() - 35;
    if (scrollPosition >= triggerPosition
        && usersScrollState.pageIndex < usersScrollState.totalPages
        && !usersScrollState.isLoading
    ) {
        fetchMoreUsers();
    }
}
function fetchMoreUsers() {
    usersScrollState.isLoading = true;
    $.ajax({
        url: '/users-search?pageIndex=' + (usersScrollState.pageIndex + 1),
        method: 'GET',
        dataType: "html",
        data: usersScrollState.formData,
        success: function (response) {
            $("#pages-items").append(response);
            usersScrollState.pageIndex++;
            usersScrollState.totalPages = $(response).data('total-pages');


        },
        error: function (jqXHR) {
            showError(jqXHR);
        },
        complete: function () {
            usersScrollState.isLoading = false;
            handleUserListScroll();//extra check 
        }
    });
}

function bindScrollEvent() {
    var scrollTimeout;
    $(window).on('scroll', function () {
        if (scrollTimeout) {
            //if this event happen again and there is was previous event watting for 100ms to be executed it will stop this event 
            clearTimeout(scrollTimeout);//this will prevent of multiple continous call of function within 100millesecond
        }
        scrollTimeout = setTimeout(() => { handleUserListScroll(); }, 100);//debouncing function called after a certial amount of time 

    });
}