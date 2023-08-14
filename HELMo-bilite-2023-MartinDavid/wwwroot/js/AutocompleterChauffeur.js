$("#ChercherChauffeur").autocomplete({
    source: function (request, response) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Stats/ChercherChauffeur/',
            data: { ChercherChauffeur: request.term },
            success: function (data) { response(data) }
        })
    }
})