/*===========================
		Navigation 
============================*/

/* Show & Hide White Navigation */
$(function () {
	
	// show/hide nav on page load
	showHideNav();
	
	$(window).scroll(function(){
		// show/hide nav on window's scroll
		showHideNav();
	})
	
	function showHideNav() {
		if($(window).scrollTop() > 50 ) {
		   // Show white nav
			$("nav").addClass("white-nav-top");
			
			// Show dark logo
			$(".navbar-brand img").attr("src","../images/Homepage/logo.png");
            
            // Show back to top button
			$("#back-to-top").fadeIn();	
			
		} 
        else {
		   // Hide white nav
			$("nav").removeClass("white-nav-top");
			
			// Show logo
			$(".navbar-brand img").attr("src","../images/Homepage/top-logo.png");
			
			// Hide back to top button
			$("#back-to-top").fadeOut();	
		}
	}
	
});


/*===========================
		Mobile Menu
============================*/
$(function () {

	// Show mobile nav
	$("#mobile-nav-open-btn").click(function() {	
		$("#mobile-nav").css("height", "100%");
	});
	
	// Hide mobile nav
	$("#mobile-nav-close-btn").click(function() {	
		$("#mobile-nav").css("height", "0%");
	});
});


/*=================================
		User Profile (Date Picker)
===================================*/
function setDatePicker(_this) { 
  
            /* Get the parent class name so we  
                can show date picker */ 
            let className = $(_this).parent() 
                .parent().parent().attr('class'); 
  
            // Remove space and add '.' 
            let removeSpace = className.replace(' ', '.'); 
  
            // jQuery class selector 
            $("." + removeSpace).datepicker({ 
                format: "dd/mm/yyyy", 
  
                // Positioning where the calendar is placed 
                orientation: "bottom auto", 
                // Calendar closes when cursor is  
                // clicked outside the calendar 
                autoclose: true, 
                showOnFocus: "false" 
            }); 
        }


/*=================================
	Password Hide & Show
===================================*/
$(".toggle-password").click(function () 
{
    
	var input = $($(this).attr("toggle"));
	if (input.attr("type") == "password") {
		input.attr("type", "text");
	} else {
		input.attr("type", "password");
	}
});


/*=================================
	FAQ Add Minus
===================================*/
$(document).ready(function () {
	$('.card .collapse').on('shown.bs.collapse', function () {
		$(this).parent().find('img').attr('src', '../images/FAQ/minus.png');		
		$(this).parent().find('.card-header').css('background-color', '#fff').css('border', '2px solid #d1d1d1').css('border-bottom', 'none').css('background-position', 'center');
		$(this).parent().find('.card-header p').css('font-weight', '600');
		$(this).parent().find('.card-body').css('border-top', 'none');

	});

	$('.card .collapse').on('hidden.bs.collapse', function () {
		$(this).parent().find('img').attr('src', '../images/FAQ/add.png');
		$(this).parent().find('.card-header').css('background-color', 'rgba(0,0,0,.03)').css('border', 'none');
		$(this).parent().find('.card-header p').css('font-weight', '400');
	});
});