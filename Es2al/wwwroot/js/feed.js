//const feedScrollState = {
//    IsLoading: false,
//    pageIndex: 1,
//    totalPages: 1

//};
//$(function () {


//    feedScrollState.totalPages = $('#pages-items').children().last().data('total-pages');
//    handleFeedScroll();
//    $('#question-filter').on('submit', function () {//remove empty inputs fields
//        $(this).find('input').each(function () {
//            var input = $(this);
//            if (!input.val()) {
//                input.remove();
//            }
//        });
//    });
//    var scrollTimeout;
//    $(window).on('scroll', function () {
//        if (scrollTimeout) {//exist time from the previous
//            clearTimeout(scrollTimeout);
//        }
//        scrollTimeout = setTimeout(() => { handleFeedScroll(); }, 100);
//    })
//})
//function handleFeedScroll() {
//    var scrollPosition = $(window).scrollTop() + $(window).height();
//    var triggerPosition = $(document).height() - 35;
//    if (!feedScrollState.IsLoading
//        && scrollPosition >= triggerPosition
//        && feedScrollState.pageIndex < feedScrollState.totalPages) {
//        fetchMoreFeed();
//    }

//}
//function fetchMoreFeed() {
//    feedScrollState.IsLoading = true;
//    $.ajax({
//        url: globalUrl,
//        data: { pageIndex: feedScrollState.pageIndex + 1 },
//        method: 'GET',
//        datetype: 'HTML',
//        success: function (response) {
//            $('#pages-items').append(response);
//            feedScrollState.pageIndex++;
//            feedScrollState.totalPages = $(response).data('total-pages');
//        },
//        error: function (error) {
//            alert(error);
//        },
//        complete: function () {
//            feedScrollState.IsLoading = false;
//            handleFeedScroll();
//        }

//    });
//}

