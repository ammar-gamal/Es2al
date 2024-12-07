function answer(obj) {
    var questionId = $(obj).data('question-id');
    var element = $('#answer_' + questionId);
    element.slideToggle();
    bindAnswerSubmission(questionId);
}

function bindAnswerSubmission(questionId) {
    $('#answerForm_' + questionId).on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var data = form.serializeArray();
        data.push({ name: 'QuestionId', value: questionId });
        $.ajax({
            url: '/inbox-answer',
            data: data,
            method: 'POST',
            dataType: 'html',
            statusCode: {
                200: function () {
                    $('#question_' + questionId).fadeOut(function () {
                        $(this).remove();
                        if (typeof handleQuestionsScroll === "function")//to check if this function is exist or not  
                        {
                            handleQuestionsScroll();
                        }
                    });
                },
                206: function (response) {
                    $('#answer_' + questionId).html(response);
                    bindAnswerSubmission(questionId); // rebind to allow for subsequent submissions
                }
            },
            error: function (jqXHR) {
                showError(jqXHR);
            }
        });
    });
}