
//This function is for reloading the main navigation links so that backbutton doesn't
//take to the back page
//Admin page has access to only certain people so refreshing facility or worker
//will have to go to home page instead of admin page

    var pathname = window.location.pathname;
    if (pathname.indexOf("Reports") > -1) {
        var path = 'Reports.aspx';
    }
    else if (pathname.indexOf("Admin") > -1)
    {
        var path = 'Admin.aspx';
    }
    else if (pathname.indexOf("Email") > -1)
    {
        var path = 'EmailTemplate.aspx';
    }
    else
    {
        var path = 'Default.aspx';
    }

    
    history.pushState(null, null, path + window.location.search);
    window.addEventListener('popstate', function (event) {
        history.pushState(null, null, path + window.location.search);
    });

  
