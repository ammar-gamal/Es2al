$(function () {
      bindSubmissionForm();//bind the form event when load the page
});
function display() {
  $('#question').slideToggle();
}
function bindSubmissionForm() {
    $('#questionForm').on('submit', (function (e) {
        e.preventDefault();
        var form = $(this);
        var data = form.serializeArray();
        const $profilePicture = $("#profilePicture");

        if ($profilePicture.length) {
            const receiverId = $profilePicture.data("userId")
            data.push({ name: "receiverId", value: receiverId });
        }
        $.ajax({
            url: '/ask/',
            type: 'post',
            dataType: 'HTML',
            data: data,
            success: function (response) {
                $('#question').html(response);
                bindSubmissionForm();
            },
            error: function (jqXHR) {
                showError(jqXHR);
            }
        });
    }));
}