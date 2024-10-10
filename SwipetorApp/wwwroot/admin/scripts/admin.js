document.addEventListener('DOMContentLoaded', function () {
    var topLogoLink = document.getElementById('topLogoLink');
    var sideNav = document.getElementById('sidenav');

    topLogoLink.addEventListener('click', function (event) {
        event.preventDefault(); // Prevent default action of the link
        sideNav.classList.toggle('sidenav-active'); // Toggle the class to show/hide the sidebar
    });
});

