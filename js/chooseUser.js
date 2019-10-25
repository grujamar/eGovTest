$(document).ready(function () {

    //console.log('Session storage is ' + sessionStorage["data.token"] + ' .Length ' + sessionStorage.length );
    if ( window.location.href.indexOf("UserInfo.html") > -1) {
        if (sessionStorage.length === 0) {
            window.location= 'http://localhost:8383/VETKS/login.html';
        }
    }
    
    if (window.location.href.indexOf("login.html") > -1){
        //console.log('clear session');
        sessionStorage.clear();
    }
    
    ///////////////////////////////////////////////////////////////////
    function GetURLParameter(sParam) {
        var urlCurrent = $(location).attr('href'); //get current url
        //console.log('Korak 1 + ' + urlCurrent);
        var decodedUrl = decodeURIComponent(urlCurrent);
        //console.log('Korak 2 + ' + decodedUrl);
        var substring = decodedUrl.substring(decodedUrl.indexOf('?') + 1);
        var sURLVariables = substring.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            //console.log('Korak 3 + ' + sParameterName);
            if (sParameterName[0] === sParam) {
                return sParameterName[1];
            }
        }
    }

    var personId = GetURLParameter('personid');
    //console.log(personId);
    callAjaxUserInfo(personId);

    function callAjaxUserInfo(personID) {
        $.ajax({
            url: "http://ucenickidomovi.pis.rs/VetWebService/api/person/" + personID,
            headers: {"Authorization": 'Bearer ' + sessionStorage["data.token"]},
            dataType: 'json',
            beforeSend: function () {
                //$('#user-info').html('<img src="https://i.gifer.com/7YQl.gif">');
                //$('#card-info').html('<img src="https://i.gifer.com/7YQl.gif">');
            }
        })
                .done(function (data) {
                    console.log(data);
                    /******************************PROFILE INFO***********************/
                    $('#user-img').empty();
                    $('#user-profile').empty();

                    $('#user-img').append('<figure><img src="https://api.adorable.io/avatars/150x150/abott@adorable.png" alt=""/></figure>');
                    $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Ime</label></div><div class="col-lg-8">' + data.Name + '</div></div><hr>');
                    $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Prezime</label></div><div class="col-lg-8">' + data.LastName + '</div></div><hr>');
                    $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Titula</label></div><div class="col-lg-8">' + data.Title + '</div></div><hr>');
                    $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Datum rodjenja</label></div><div class="col-lg-8">' + (data.DateOfBirth).substring(8, 10) + '-' + (data.DateOfBirth).substring(5, 7) + '-' + (data.DateOfBirth).substring(0, 4) + '</div></div><hr>');
                    if (data.Status === true)
                    {
                        $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Status</label></div><div class="col-lg-8 text-success">Aktivan</div></div><hr>');
                    } else {
                        $('#user-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Status</label></div><div class="col-lg-8 text-danger">Neaktivan</div></div><hr>');
                    }
                      
                    $('div.loading').remove(); 
                    
                    /******************************CARDS INFO***********************/
                    $('#card-img').empty();
                    $('#card-profile').empty();

                    if (data.Cards.length < 1) {
                        //console.log('Usao ovde ' + data.Cards.length);
                        $('#card-profile').html('<p class="error"><strong>Ne postoji kartica za izabranu osobu.</strong></p>');
                    } else {
                        //console.log('Usao ovde ' + data.Cards.length);
                        showCardInfoBasedOnLength(data.Cards[0].CardNumber, data.Cards[0].CardValidity, data.Cards[0].LastUpdateTime, data.Cards[0].Status);
                    }
                    
                    $('div.loading').remove();
                    /******************************Diploma INFO***********************/
                    var data1 = [];

                    for (var x in data.Certificates) {
                        data1.push([data.Certificates[x].Points,
                            (data.Certificates[x].GraduateDate).substring(8, 10) + '-' + (data.Certificates[x].GraduateDate).substring(5, 7) + '-' + (data.Certificates[x].GraduateDate).substring(0, 4),
                            data.Certificates[x].DepartmentId,
                            data.Certificates[x].StudyLevelId,
                            data.Certificates[x].TitleId,
                            data.Certificates[x].ProfessionalOrientationId
                        ]);
                    }
                    $('#exampleDiploma').DataTable({
                        data: data1,
                        scrollY: true,
                        scrollX: false,
                        scrollCollapse: true,
                        paging: false,
                        searching: false,
                        initComplete: function( settings, json ) {  
                            $('div.loading').remove(); 
                          }
                    });


                })
                .fail(function (jqXHR, statusText) {
                    $('#user-profile').text(jqXHR.status + '-' + jqXHR.statusText + '-' + statusText);
                    $('#card-profile').text(jqXHR.status + '-' + jqXHR.statusText + '-' + statusText);
                    $('#diploma-info').text(jqXHR.status + '-' + jqXHR.statusText + '-' + statusText);
                });

    }


    function showCardInfoBasedOnLength(cardNumber, cardValidity, lasrUpdateTime, cardStatus) {
        if (cardStatus === true) {
            $('#card-img').append('<span class="fa fa-id-card-o"></span>');
            $('#card-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Broj kartice</label></div><div class="col-lg-8">' + cardNumber + '</div></div><hr>');
            $('#card-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Rok važenja kartice</label></div><div class="col-lg-8">' + (cardValidity).substring(8, 10) + '-' + (cardValidity).substring(5, 7) + '-' + (cardValidity).substring(0, 4) + '</div></div><hr>');
            $('#card-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Poslednji put ažurirana</label></div><div class="col-lg-8">' + (lasrUpdateTime).substring(8, 10) + '-' + (lasrUpdateTime).substring(5, 7) + '-' + (lasrUpdateTime).substring(0, 4) + '</div></div><hr>');
            $('#card-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Status</label></div><div class="col-lg-8 text-success">Aktivna kartica</div></div><hr>');
        } else {
            $('#card-profile').append('<div class="row"><div class="col-lg-4"><label class="font-weight-bold">Status</label></div><div class="col-lg-8 text-danger">Neaktivna kartica</div></div><hr>');
        }
    }


    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
                .columns.adjust();
    });
});