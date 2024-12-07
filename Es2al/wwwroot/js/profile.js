
$(function () {
  //  bindSubmissionForm();//bind the form event when load the page
});

function toggleFollow() {
    const followingId = $('#profilePicture').data('userId');
    const followButton = $('#followButton');
    const followersElement = $('#followersNumber');
    var followersNumber = followersElement.data("followers-count");
    const action = followButton.data("is-following") === 1 ? 'unfollow' : 'follow';

    $.ajax({
        type: 'POST',
        url: '/' + action + '/' + followingId,
        success: function () {
            if (action === 'follow') {
                followersNumber++;
                followButton.data('is-following', 1);
                followButton.removeClass('btn-danger').addClass('btn-secondary').text('Unfollow');
            } else {
                followersNumber--;
                followButton.data("is-following", 0)
                followButton.removeClass('btn-secondary').addClass('btn-danger').text('Follow');
            }
            followersElement.data('followers-count', followersNumber);
            followersElement.text(followersNumber);
        },
        error: function (jqXHR) {
            showError(jqXHR);
        }
    });
}

const followScrollState = {
    isLoadingfalse: false,
    pageIndex: null,
    totalPages: null,
    url: null
}
function handleFollowScrollState() {
    const scrollPosition = $('#listModel').scrollTop() + $('#listModel').height();
    const triggerPosition = $('#user-list').height();
    if (scrollPosition >= triggerPosition &&
        !followScrollState.isLoading &&
        followScrollState.pageIndex < followScrollState.totalPages) {
        fetchFollows();
    }
}

function displayList(obj) {
    resetScrollState();  
    const type = $(obj).data('type');
    const id = $("#profilePicture").data("userId");
    const url = '/' + type + '/' + id;

    followScrollState.url = url;
    $('#user-list').empty();
    $('#modelTitle').text(type);
    $('#listModel').modal('show');

    bindModalScrollEvent();
    handleFollowScrollState();  
}


function resetScrollState() {
    followScrollState.isLoading = false;
    followScrollState.pageIndex = 0;
    followScrollState.totalPages = 1;
    followScrollState.url = null;
}

function fetchFollows() {
    followScrollState.isLoading = true;
    $.ajax({
        url: followScrollState.url,
        method: 'GET',
        dataType: 'html',
        data: {
            pageIndex: followScrollState.pageIndex + 1
        },
        success: function (response) {
            $('#user-list').append(response);
            followScrollState.totalPages = $(response).data('total-pages');
            followScrollState.pageIndex++;
            followScrollState.isLoading = false;
            handleFollowScrollState();
        },
        error: function (jqXHR) {
            showError(jqXHR);
        }
    });
}

function bindModalScrollEvent() {
    var scrollTimeout;
    $('#listModel').off('scroll').on('scroll', function () {
        if (scrollTimeout) {
            clearTimeout(scrollTimeout);
        }
        scrollTimeout = setTimeout(handleFollowScrollState, 100);
    });
}
