$(document).ready(function () {

    $("#btnRegisterSubmit").click(function () {

        //----------------------------------------------------------------------
        var log = log4javascript.getDefaultLogger();
        //----------------------------------------------------------------------
        //Function to register user
        log.info('Start register user');
        registerUser('UsernameTest2', 'PasswordTest2', log, 'http://ucenickidomovi.pis.rs/VetWebService/api/auth/register');
        log.info('End register user');
    });
    //}



    function registerUser(Username, Password, log, url) {

        var SendInfoRegister = { username: Username, password: Password };

        log.info('Parametri za upis - username: ' + Username + ' password: ' + Password);

        $.ajax({
            type: 'post',
            url: url,
            data: JSON.stringify(SendInfoRegister),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            beforeSend: function () {
                $('.user-login-inner').html('<div class="loading text-center mt-5"><h4>Please wait..</h4><img src="images/throbber.gif" class="animated-gif"></div>');
            },
            success: function (data, textStatus, xhr) {
                // successful request; 
                log.info('Uspešno - ' + xhr.status + ' ' + textStatus + ' ' + data);
                $('.user-login-inner').empty();
                $('.user-login-inner').html('<div class="row"><article id="user-profile" class="col-12"><p><strong>Successfully entered data.</strong></p><a href="ajax-json.html" class="btn btn-info px-4">Back</a></article></div>');
                successInsertUser();
            },
            error: function (data, textStatus, xhr) {
                // failed request;
                log.info('Neuspešno - ' + xhr.status + ' ' + textStatus + ' ' + data);
                $('.user-login-inner').html('<p class="error"><strong>Error!</strong> Please try later.</p>');
                ErrorInsertUser();
            }
        });
    }


    ////-------------------SWEETALERT-------------------------------
    /////////////////////////////////////////////////////////////////
    function SuccessSendingData() {
        swal({
            title: 'Successfully entered data.',
            text: '',
            type: 'OK'
        });
    }
    function ErrorSendingData() {
        swal({
            title: 'Error.',
            text: 'Please try later.',
            type: 'OK'
        });
    }
            /////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////
    
    
    
    
   
    
    
    
});

