$("#ChercherClient").autocomplete({
    source: function (request, response) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Stats/ChercherClient/',
            data: { ChercherClient: request.term },
            success: function (data) { response(data) }
        })
    }
})