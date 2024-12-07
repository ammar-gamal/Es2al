function toggleReact(obj) {
    // var clicked = action == 'Like' ? 'like' : 'dislike';//determine which button is clicked
    var clicked = $(obj).data('type');
    var count = parseInt($(obj).data('count'));//get the count of the clicked button 
    var answerId = $(obj).data('answer-id');
    $.ajax({
        url: '/react',
        data: {
            answerId: answerId,
            react: clicked
        },
        method: 'POST',
        success: function (response) {
            if (response == -1) {//redo 
                count--;
                $(obj).removeClass('active');
            }
            else if (response == 0) {//was clicked the opposite reaction before
                var opposite = clicked == 'like' ? 'dislike' : 'like';
                var oppositeElement = $('#answer-' + answerId + '-' + opposite)
                var oppositeCount = parseInt(oppositeElement.data('count'));
                oppositeCount--;
                oppositeElement.data('count', oppositeCount);
                $('#' + opposite + '-count-' + answerId).text(oppositeCount);
                count++;
                oppositeElement.removeClass('active');
                $(obj).addClass('active');
            }
            else {//normal case
                count++;
                $(obj).addClass('active');
            }
            $(obj).data('count', count);
            $('#' + clicked + '-count-' + answerId).text(count);
        },
        error: function (jqXHR) {
            showError(jqXHR);
        }
    })
}

function subQuestion(obj) {
    var questionId = $(obj).data('question-id');
    var receiverId = $(obj).data('receiver-id');
    var threadId = $(obj).data('thread-id');
    var element = $('#subQuestion_' + questionId);

    element.slideToggle();

    bindSubQuestionSubmission(questionId, threadId, receiverId);
}

function bindSubQuestionSubmission(questionId, threadId, receiverId) {
    var form = $('#questionForm_' + questionId);

    form.off('submit').on('submit', function (e) {
        e.preventDefault();

        var data = form.serializeArray();
        data.push(
            { name: "ParentQuestionId", value: questionId },
            { name: "ThreadId", value: threadId },
            { name: "ReceiverId", value: receiverId }
        );
        $.ajax({
            url: '/sub-question/',
            data: data,
            method: 'POST',
            dataType: 'HTML',
            success: function (response) {
                $('#subQuestion_' + questionId).html(response);
                bindSubQuestionSubmission(questionId, threadId, receiverId);

            },
            error: function (jqXHR) {
                showError(jqXHR);
            }
        });
    });
}
