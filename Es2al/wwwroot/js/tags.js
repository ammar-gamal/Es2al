$(function () {
    getTags();
    bindTagForm();
})

function getTags() {
    $.ajax({
        url: "/all-tags",
        method: 'GET',
        dataType: 'HTML',
        success: function (response) {
            $('#tags').html(response);
        },
        error: function (jqXHR) {
            showError(jqXHR);
        }
    })
}

function bindTagForm() {
    $('#tag-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var data = form.serializeArray();

        $.ajax({
            url: '/create-tag',
            method: 'post',
            data: data,
            success: function (response) {
                form[0].reset();
                $('#tags').html(response);
            },
            error: function (jqXHR) {
                if (jqXHR.status === 400) {
                    const errors = jqXHR.responseJSON.errors;
                    Object.keys(errors).forEach(key => {
                        $('#name-validation').text(errors[key]);
                    });
                } else {
                    showError(jqXHR);
                }
            }
        })
    })
}