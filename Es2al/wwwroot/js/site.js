// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showError(jqXHR) {
    const statusCode = jqXHR.status;
    const errorMessage = jqXHR.responseText || 'An error occurred. Please try again.';
    alert(`Error ${statusCode}: ${errorMessage}`);
}
function deleteQuestion(obj) {
    const questionId = $(obj).data('question-id');
    $.ajax({
        url: '/questions/delete/' + questionId,
        success: function () {
            $('#question_' + questionId).fadeOut(function () {
                $(this).remove();
                if (typeof handleQuestionsScroll === "function")//to check if this function is exist or not  
                {
                    handleQuestionsScroll();
                }
            });
        },
         error: function(jqXHR) {
            showError(jqXHR);
        }
    })
}